using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using LocationTrackingService.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace LocationTrackingService.Implementation
{
    public class LocationTrackingService : ILocationTrackingService
    {
        private readonly IDistributedCache _cache;
        private readonly IDatabase _redisDb;
        private readonly GeometryFactory _geometryFactory;
        private readonly ILogger<LocationTrackingService> _logger;
        private readonly IAddressRepository _addressRepository;
        private readonly IDriverLocationRepository _driverLocationRepository;

        public LocationTrackingService(
            IDistributedCache cache,
            IDatabase redisDb,
            GeometryFactory geometryFactory,
            ILogger<LocationTrackingService> logger,
            IAddressRepository addressRepository,
            IDriverLocationRepository driverLocationRepository)
        {
            _cache = cache;
            _redisDb = redisDb;
            _geometryFactory = geometryFactory;
            _logger = logger;
            _addressRepository = addressRepository;
            _driverLocationRepository = driverLocationRepository;
        }

        public async Task UpdateDriverLocationAsync(DriverLocationUpdateDto locationUpdate)
        {
            _logger.LogInformation("Updating location for driver {DriverId}: Lat={Latitude}, Lon={Longitude}, Status={Status}",
                locationUpdate.DriverId, locationUpdate.Latitude, locationUpdate.Longitude, locationUpdate.Status);

            // Get previous location to detect status changes
            var previousLocation = await GetDriverCurrentLocationAsync(locationUpdate.DriverId);

            // Store in Redis with key format: driver:{driverId}:location
            var cacheKey = $"driver:{locationUpdate.DriverId}:location";
            var locationData = JsonSerializer.Serialize(locationUpdate);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10) // Expire after 10 hours
            };

            await _cache.SetStringAsync(cacheKey, locationData, cacheOptions);

            _logger.LogDebug("Location stored in cache for driver {DriverId}", locationUpdate.DriverId);

            // Manage active driver set based on status
            if (locationUpdate.Status == 1)
            {
                await _redisDb.SetAddAsync("driver:active:set", locationUpdate.DriverId.ToString());
                _logger.LogInformation("Driver {DriverId} added to active set", locationUpdate.DriverId);
            }
            else if (locationUpdate.Status ==2 )
            {
                await _redisDb.SetRemoveAsync("driver:active:set", locationUpdate.DriverId.ToString());
                _logger.LogInformation("Driver {DriverId} removed from active set", locationUpdate.DriverId);
            }

            // Check for status change and persist immediately if transitioning to/from "online"
            bool statusChanged = previousLocation == null || previousLocation.Status != locationUpdate.Status;
            if (statusChanged && (locationUpdate.Status == (int)DriverStatus.Online || previousLocation?.Status == (int)DriverStatus.Online))
            {
                var dbLocation = new DriverLocationDb
                {
                    DriverId = locationUpdate.DriverId,
                    Location = _geometryFactory.CreatePoint(new Coordinate(locationUpdate.Longitude, locationUpdate.Latitude)),
                    Status = locationUpdate.Status,
                    Timestamp = locationUpdate.Timestamp,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = locationUpdate.DriverId
                };
                await _driverLocationRepository.AddDriverLocationAsync(dbLocation);
                _logger.LogInformation("Persisted location for driver {DriverId} due to status change to {Status}", locationUpdate.DriverId, locationUpdate.Status);
            }

            // Persist transition records for online/offline status changes
            if (statusChanged)
            {
                if (locationUpdate.Status == (int)DriverStatus.Offline)
                {
                    // Query the last persisted location for offline_exit transition
                    var lastPersisted = await _driverLocationRepository.GetLastDriverLocationAsync(locationUpdate.DriverId);
                    if (lastPersisted != null)
                    {
                        var transitionRecord = new DriverLocationDb
                        {
                            DriverId = locationUpdate.DriverId,
                            Location = _geometryFactory.CreatePoint(new Coordinate(lastPersisted.Location.X, lastPersisted.Location.Y)),
                            Status = (int)DriverStatus.OfflineExit,
                            Timestamp = DateTime.UtcNow,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = locationUpdate.DriverId
                        };
                        await _driverLocationRepository.AddDriverLocationAsync(transitionRecord);
                        _logger.LogInformation("Persisted offline_exit transition for driver {DriverId}", locationUpdate.DriverId);
                    }
                }
                else if (locationUpdate.Status == (int)DriverStatus.Online)
                {
                    // Persist online_entry transition with current location data
                    var transitionRecord = new DriverLocationDb
                    {
                        DriverId = locationUpdate.DriverId,
                        Location = _geometryFactory.CreatePoint(new Coordinate(locationUpdate.Longitude, locationUpdate.Latitude)),
                        Status = (int)DriverStatus.OnlineEntry,
                        Timestamp = DateTime.UtcNow,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = locationUpdate.DriverId
                    };
                    await _driverLocationRepository.AddDriverLocationAsync(transitionRecord);
                    _logger.LogInformation("Persisted online_entry transition for driver {DriverId}", locationUpdate.DriverId);

                    // If last offline location fields are provided, persist offline_exit immediately
                    if (locationUpdate.LastOfflineLatitude.HasValue && locationUpdate.LastOfflineLongitude.HasValue && locationUpdate.LastOfflineTimestamp.HasValue)
                    {
                        var offlineExitRecord = new DriverLocationDb
                        {
                            DriverId = locationUpdate.DriverId,
                            Location = _geometryFactory.CreatePoint(new Coordinate(locationUpdate.LastOfflineLongitude.Value, locationUpdate.LastOfflineLatitude.Value)),
                            Status = (int)DriverStatus.OfflineExit,
                            Timestamp = locationUpdate.LastOfflineTimestamp.Value,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = locationUpdate.DriverId
                        };
                        await _driverLocationRepository.AddDriverLocationAsync(offlineExitRecord);
                        _logger.LogInformation("Persisted offline_exit transition for driver {DriverId} using last offline location", locationUpdate.DriverId);
                    }
                }
            }

            // Periodic persistence to database is handled by LocationPersistenceService background service (every 5 minutes)
        }

        public async Task<DriverLocationUpdateDto?> GetDriverCurrentLocationAsync(long driverId)
        {
            var cacheKey = $"driver:{driverId}:location";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            return JsonSerializer.Deserialize<DriverLocationUpdateDto>(cachedData);
        }

        public async Task PingDriverAsync(long driverId)
        {
            var cacheKey = $"driver:{driverId}:location";
            var cachedData = await _cache.GetStringAsync(cacheKey);

            DriverLocationUpdateDto locationUpdate;
            if (string.IsNullOrEmpty(cachedData))
            {
                // Create a minimal location entry for ping
                locationUpdate = new DriverLocationUpdateDto
                {
                    DriverId = driverId,
                    Latitude = 0,
                    Longitude = 0,
                    Status = (int)DriverStatus.Offline, // Default to offline if no previous location
                    Timestamp = DateTime.UtcNow,
                    LastPing = DateTime.UtcNow
                };
            }
            else
            {
                locationUpdate = JsonSerializer.Deserialize<DriverLocationUpdateDto>(cachedData)!;
                locationUpdate.LastPing = DateTime.UtcNow;
            }

            var locationData = JsonSerializer.Serialize(locationUpdate);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10) // Keep same expiration
            };

            await _cache.SetStringAsync(cacheKey, locationData, cacheOptions);
        }

        public async Task<bool> CheckGeofenceAsync(GeofenceCheckDto geofenceCheck)
        {
            _logger.LogInformation("Checking geofence for driver {DriverId}, type {CheckType}, target {TargetLocationId}, buffer {BufferMeters}m",
                geofenceCheck.DriverId, geofenceCheck.CheckType, geofenceCheck.TargetLocationId, geofenceCheck.BufferMeters);

            // Get driver's current location from Redis
            var driverLocation = await GetDriverCurrentLocationAsync(geofenceCheck.DriverId);
            if (driverLocation == null)
            {
                _logger.LogWarning("Driver {DriverId} location not found in cache", geofenceCheck.DriverId);
                return false;
            }

            _logger.LogDebug("Driver {DriverId} current location: Lat={Latitude}, Lon={Longitude}",
                geofenceCheck.DriverId, driverLocation.Longitude, driverLocation.Latitude);

            bool isWithin;
            if (geofenceCheck.CheckType.ToLower() == "pickup")
            {
                // Check against user's saved location using repository
                isWithin = await _addressRepository.CheckGeofencePickupAsync(
                    driverLocation.Longitude, driverLocation.Latitude,
                    geofenceCheck.TargetLocationId, geofenceCheck.BufferMeters);
                _logger.LogDebug("Executed pickup geofence check via repository");
            }
            else if (geofenceCheck.CheckType.ToLower() == "dropoff")
            {
                // TODO: Implement facilities repository for dropoff checks
                _logger.LogWarning("Dropoff geofence check not implemented - facilities repository needed");
                isWithin = false;
            }
            else
            {
                _logger.LogError("Invalid check type: {CheckType}", geofenceCheck.CheckType);
                throw new ArgumentException("Invalid check type. Must be 'pickup' or 'dropoff'");
            }

            _logger.LogInformation("Geofence check result for driver {DriverId}: {IsWithin}",
                geofenceCheck.DriverId, isWithin);

            return isWithin;
        }
    }
}