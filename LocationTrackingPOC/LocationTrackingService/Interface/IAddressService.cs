using LocationTrackingCommon.Models;

namespace LocationTrackingService.Interface
{
    public interface IAddressService
    {
        Task<Address> SaveNewAddressAsync(Address address);
        Task<Address?> GetAddressByIdAsync(long id);
        Task<IEnumerable<Address>> GetAddressesByUserIdAsync(long userId);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(long id);
    }
}
