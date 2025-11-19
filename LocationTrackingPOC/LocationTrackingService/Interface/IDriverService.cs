using LocationTrackingCommon.Models;

namespace LocationTrackingService.Interface
{
    public interface IDriverService
    {
        Task<Driver> GetDriverByIdAsync(long id);
        Task<long> RegisterDriverAsync(Driver driver);
        Task UpdateDriverStatusAsync(long driverId, int status);
        Task<int?> GetDriverStatusAsync(long driverId);
    }
}