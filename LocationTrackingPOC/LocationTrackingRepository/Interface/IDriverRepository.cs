using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Interface
{
    public interface IDriverRepository
    {
        Task<DriverDb?> GetDriverByIdAsync(long id);
        Task<long> AddDriverAsync(DriverDb driver);
        Task UpdateDriverAsync(DriverDb driver);
        Task DeleteDriverAsync(long id);
        Task<IEnumerable<DriverDb>> GetAllDriversAsync();
        Task<IEnumerable<DriverDb>> GetDriversByStatusAsync(int statusId);
    }
}