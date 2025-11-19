

using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Interface
{
    public interface IAddressRepository
    {
        Task<AddressDb> CreateAsync(AddressDb address);
        Task<AddressDb?> GetAddressByIdAsync(long id);
        Task<IEnumerable<AddressDb>> GetAddressesByUserIdAsync(long userId);
        Task<AddressDb> UpdateAsync(AddressDb address);
        Task<bool> DeleteAsync(long id);
        Task<bool> CheckGeofencePickupAsync(double driverLat, double driverLon, long targetLocationId, double bufferMeters);
        Task<bool> CheckGeofenceDropoffAsync(double driverLat, double driverLon, long targetLocationId, double bufferMeters);
    }
}
