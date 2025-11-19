using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Data;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Implementation
{
    public class DriverLocationRepository : IDriverLocationRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DriverLocationRepository> _logger;

        public DriverLocationRepository(AppDbContext context, ILogger<DriverLocationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DriverLocationDb?> GetDriverLocationByIdAsync(long id)
        {
            return await _context.DriverLocations
                .Include(dl => dl.Driver)
                .FirstOrDefaultAsync(dl => dl.Id == id);
        }

        public async Task<IEnumerable<DriverLocationDb>> GetDriverLocationsByDriverIdAsync(long driverId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.DriverLocations
                .Where(dl => dl.DriverId == driverId);

            if (fromDate.HasValue)
                query = query.Where(dl => dl.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(dl => dl.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(dl => dl.Timestamp)
                .ToListAsync();
        }

        public async Task<long> AddDriverLocationAsync(DriverLocationDb driverLocation)
        {
            _context.DriverLocations.Add(driverLocation);
            await _context.SaveChangesAsync();
            return driverLocation.Id;
        }

        public async Task AddDriverLocationsBulkAsync(IEnumerable<DriverLocationDb> driverLocations)
        {
            // Use EF Core's database facade for bulk operations
            // For high-performance bulk inserts, consider using a library like EFCore.BulkExtensions
            // For now, we'll use individual inserts with transaction for consistency

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;
                var systemUserId = 1; // System user

                foreach (var location in driverLocations)
                {
                    // Set audit fields
                    location.CreatedDate = now;
                    location.CreatedBy = systemUserId;

                    _context.DriverLocations.Add(location);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Bulk inserted {Count} driver locations", driverLocations.Count());
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error in bulk insert of driver locations");
                throw;
            }
        }

        public async Task UpdateDriverLocationAsync(DriverLocationDb driverLocation)
        {
            _context.DriverLocations.Update(driverLocation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDriverLocationAsync(long id)
        {
            var driverLocation = await GetDriverLocationByIdAsync(id);
            if (driverLocation != null)
            {
                _context.DriverLocations.Remove(driverLocation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DriverLocationDb>> GetAllDriverLocationsAsync()
        {
            return await _context.DriverLocations
                .Include(dl => dl.Driver)
                .OrderByDescending(dl => dl.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverLocationDb>> GetDriverLocationsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.DriverLocations
                .Include(dl => dl.Driver)
                .Where(dl => dl.Timestamp >= fromDate && dl.Timestamp <= toDate)
                .OrderByDescending(dl => dl.Timestamp)
                .ToListAsync();
        }

        public async Task<DriverLocationDb?> GetLatestDriverLocationAsync(long driverId)
        {
            return await _context.DriverLocations
                .Where(dl => dl.DriverId == driverId)
                .OrderByDescending(dl => dl.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<DriverLocationDb?> GetLastDriverLocationAsync(long driverId)
        {
            return await GetLatestDriverLocationAsync(driverId);
        }

        public async Task<Dictionary<long, DriverLocationDb?>> GetLatestDriverLocationsAsync(IEnumerable<long> driverIds)
        {
            // Convert to list to allow multiple enumeration
            var driverIdList = driverIds.ToList();
            if (!driverIdList.Any())
            {
                return new Dictionary<long, DriverLocationDb?>();
            }

            // Get the latest location per driver in a single query
            // Use a subquery to find the max timestamp for each driver, then join back
            var latestLocations = await _context.DriverLocations
                .Where(dl => driverIdList.Contains(dl.DriverId))
                .GroupBy(dl => dl.DriverId)
                .Select(g => g.OrderByDescending(dl => dl.Timestamp).FirstOrDefault())
                .ToListAsync();

            // Build dictionary: driverId -> latest location (or null if no location exists)
            var result = new Dictionary<long, DriverLocationDb?>();
            foreach (var driverId in driverIdList)
            {
                var latestLocation = latestLocations.FirstOrDefault(dl => dl?.DriverId == driverId);
                result[driverId] = latestLocation;
            }

            return result;
        }

        public async Task PersistDriverLocationsAsync(IEnumerable<(long DriverId, DriverLocationUpdateDto Location)> locations)
        {
            // Use EF Core transaction for consistency
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;
                var systemUserId = 1; // System user

                var locationsList = locations.ToList();
                if (!locationsList.Any())
                {
                    _logger.LogInformation("No driver locations to persist");
                    await transaction.CommitAsync();
                    return;
                }

                // Collect all distinct driver IDs upfront
                var distinctDriverIds = locationsList.Select(loc => loc.DriverId).Distinct();

                // Query database once for latest locations per driver
                var latestLocationsDict = await GetLatestDriverLocationsAsync(distinctDriverIds);

                // Build list of new location entities without per-iteration awaits
                var locationEntities = new List<DriverLocationDb>();

                foreach (var loc in locationsList)
                {
                    // Check against dictionary instead of database query per iteration
                    var lastLocation = latestLocationsDict.TryGetValue(loc.DriverId, out var cached) ? cached : null;
                    if (lastLocation == null || loc.Location.Timestamp > lastLocation.Timestamp)
                    {
                        locationEntities.Add(new DriverLocationDb
                        {
                            DriverId = loc.DriverId,
                            Location = new Point(loc.Location.Longitude, loc.Location.Latitude),
                            Status = loc.Location.Status,
                            Timestamp = loc.Location.Timestamp,
                            CreatedDate = now,
                            CreatedBy = systemUserId
                        });
                    }
                }

                if (locationEntities.Any())
                {
                    await _context.DriverLocations.AddRangeAsync(locationEntities);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully persisted {Count} driver locations", locationEntities.Count);
                }
                else
                {
                    _logger.LogInformation("No new driver locations to persist");
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error persisting driver locations");
                throw;
            }
        }
    }
}