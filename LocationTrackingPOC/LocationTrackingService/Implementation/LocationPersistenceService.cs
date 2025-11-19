using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace LocationTrackingService.Implementation
{
    public class LocationPersistenceService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LocationPersistenceService> _logger;
        private readonly StackExchange.Redis.IDatabase _redisDb;
        private readonly TimeSpan _persistenceInterval = TimeSpan.FromMinutes(5);
        private Timer? _timer;
        private CancellationTokenSource? _cancellationTokenSource;

        public LocationPersistenceService(
            IServiceProvider serviceProvider,
            ILogger<LocationPersistenceService> logger,
            StackExchange.Redis.IDatabase redisDb)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _redisDb = redisDb;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Location Persistence Service starting");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _timer = new Timer(async _ => await PersistDriverLocationsAsync(), null, TimeSpan.Zero, _persistenceInterval);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Location Persistence Service stopping");
            _timer?.Dispose();
            _cancellationTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        public async Task PersistDriverLocationsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            var repository = scope.ServiceProvider.GetRequiredService<IDriverLocationRepository>();

            // In a production environment, you would maintain a set of active driver IDs
            // For this implementation, we'll scan for driver location keys in Redis
            // Note: This is not the most efficient approach for large-scale deployments

            var activeDriverIds = await GetActiveDriverIdsFromRedisAsync(cache);

            // Check for offline drivers based on ping timeout
            await CheckAndMarkOfflineDriversAsync(cache);

            if (!activeDriverIds.Any())
            {
                _logger.LogInformation("No active drivers found for persistence");
                return;
            }

            // Collect all driver locations first
            var locationsToPersist = new List<(long DriverId, DriverLocationUpdateDto Location)>();

            // For high performance, batch fetch all locations in one Redis operation
            if (cache is StackExchange.Redis.IDatabase redisDb)
            {
                // Use Redis MGET for batch retrieval
                var keys = activeDriverIds.Select(id => (RedisKey)$"driver:{id}:location").ToArray();
                var values = await redisDb.StringGetAsync(keys);

                for (int i = 0; i < activeDriverIds.Count; i++)
                {
                    var driverId = activeDriverIds[i];
                    var value = values[i];

                    if (!value.IsNullOrEmpty)
                    {
                        try
                        {
                            var location = JsonSerializer.Deserialize<DriverLocationUpdateDto>(value.ToString());
                            if (location != null)
                            {
                                locationsToPersist.Add((driverId, location));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"Failed to deserialize location for driver {driverId}");
                        }
                    }
                }
            }
            else
            {
                // Fallback to individual gets for other cache implementations
                foreach (var driverId in activeDriverIds)
                {
                    var location = await GetDriverLocationFromRedisAsync(cache, driverId);
                    if (location != null)
                    {
                        locationsToPersist.Add((driverId, location));
                    }
                }
            }

            if (!locationsToPersist.Any())
            {
                _logger.LogInformation("No valid locations found for persistence");
                return;
            }

            try
            {
                await repository.PersistDriverLocationsAsync(locationsToPersist);
                _logger.LogInformation($"Persisted locations for {locationsToPersist.Count} drivers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error persisting driver locations");
                throw;
            }
        }

        private async Task<List<long>> GetActiveDriverIdsFromRedisAsync(IDistributedCache cache)
        {
            // Use Redis SET to maintain active driver IDs for high performance
            // When drivers come online/go offline, use SADD/SREM on "driver:active:set"

            try
            {
                var activeDriverIds = await _redisDb.SetMembersAsync("driver:active:set");
                return activeDriverIds
                    .Select(member => (long)member)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active driver IDs from Redis SET");
                return new List<long>();
            }
        }

        private async Task<DriverLocationUpdateDto?> GetDriverLocationFromRedisAsync(IDistributedCache cache, long driverId)
        {
            var cacheKey = $"driver:{driverId}:location";
            var cachedData = await cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedData))
                return null;

            return JsonSerializer.Deserialize<DriverLocationUpdateDto>(cachedData);
        }

        private async Task CheckAndMarkOfflineDriversAsync(IDistributedCache cache)
        {
            var activeDriverIds = await GetActiveDriverIdsFromRedisAsync(cache);
            var offlineTimeout = TimeSpan.FromMinutes(5); // Configurable timeout

            foreach (var driverId in activeDriverIds)
            {
                var location = await GetDriverLocationFromRedisAsync(cache, driverId);
                if (location != null && DateTime.UtcNow - location.LastPing > offlineTimeout)
                {
                    // Mark as offline
                    location.Status = (int)DriverStatus.Offline;
                    location.LastPing = DateTime.UtcNow; // Update ping to prevent repeated processing

                    var cacheKey = $"driver:{driverId}:location";
                    var locationData = JsonSerializer.Serialize(location);
                    var cacheOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(10)
                    };
                    await cache.SetStringAsync(cacheKey, locationData, cacheOptions);

                    // Remove from active set
                    await _redisDb.SetRemoveAsync("driver:active:set", driverId.ToString());

                    _logger.LogInformation("Marked driver {DriverId} as offline due to ping timeout", driverId);
                }
            }
        }
    }
}