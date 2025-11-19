using LocationTrackingCommon.Models;
using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Interface
{
    public interface IDriverLocationRepository
    {
        Task<DriverLocationDb?> GetDriverLocationByIdAsync(long id);
        Task<IEnumerable<DriverLocationDb>> GetDriverLocationsByDriverIdAsync(long driverId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<long> AddDriverLocationAsync(DriverLocationDb driverLocation);
        Task AddDriverLocationsBulkAsync(IEnumerable<DriverLocationDb> driverLocations);
        Task UpdateDriverLocationAsync(DriverLocationDb driverLocation);
        Task DeleteDriverLocationAsync(long id);
        Task<IEnumerable<DriverLocationDb>> GetAllDriverLocationsAsync();
        Task<IEnumerable<DriverLocationDb>> GetDriverLocationsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<DriverLocationDb?> GetLatestDriverLocationAsync(long driverId);
        Task<DriverLocationDb?> GetLastDriverLocationAsync(long driverId);
        Task<Dictionary<long, DriverLocationDb?>> GetLatestDriverLocationsAsync(IEnumerable<long> driverIds);
        Task PersistDriverLocationsAsync(IEnumerable<(long DriverId, DriverLocationUpdateDto Location)> locations);
    }
}