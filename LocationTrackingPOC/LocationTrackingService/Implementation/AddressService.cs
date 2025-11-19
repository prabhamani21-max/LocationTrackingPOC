using NetTopologySuite.Geometries;
using LocationTrackingService.Interface;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using System.Collections.Generic;


namespace LocationTrackingService.Implementation
{
    public class AddressService: IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly GeometryFactory _geometryFactory;
        private readonly ICurrentUser _currentUser;
        public AddressService(IAddressRepository addressService, GeometryFactory geometryFactory, ICurrentUser currentUser)
        {
            _addressRepository = addressService;
            _geometryFactory = geometryFactory;
            _currentUser = currentUser;
        }
        public async Task<Address> SaveNewAddressAsync(Address address)
        {
            // 1. Business Logic: Create the Point
            // IMPORTANT: (Longitude, Latitude) order for (X, Y)
            var locationPoint = _geometryFactory.CreatePoint(new Coordinate(address.Longitude, address.Latitude));

            // 2. Business Logic: Map DTO to the database model
            var newAddressDb = new AddressDb
            {
                UserId = address.UserId, // Use the secure UserId from the token
                Label = address.Label,
                FullAddress = address.FullAddress,
                CreatedBy = _currentUser.UserId,
                CreatedDate = DateTime.UtcNow,
                Location = locationPoint
            };

            // 3. Call Repository to save
            var savedAddress = await _addressRepository.CreateAsync(newAddressDb);

            // 4. Map the result to a response DTO
            return MapToAddressDto(savedAddress);
        }

        public async Task<Address?> GetAddressByIdAsync(long id)
        {
            var addressDb = await _addressRepository.GetAddressByIdAsync(id);
            return addressDb == null ? null : MapToAddressDto(addressDb);
        }

        public async Task<IEnumerable<Address>> GetAddressesByUserIdAsync(long userId)
        {
            var addressesDb = await _addressRepository.GetAddressesByUserIdAsync(userId);
            return addressesDb.Select(MapToAddressDto);
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            // Create the Point
            var locationPoint = _geometryFactory.CreatePoint(new Coordinate(address.Longitude, address.Latitude));

            var addressDb = new AddressDb
            {
                Id = address.Id,
                UserId = address.UserId,
                Label = address.Label,
                FullAddress = address.FullAddress,
                Location = locationPoint,
                UpdatedDate = DateTime.UtcNow,
                UpdatedBy = _currentUser.UserId
            };

            var updatedAddress = await _addressRepository.UpdateAsync(addressDb);
            return MapToAddressDto(updatedAddress);
        }

        public async Task<bool> DeleteAddressAsync(long id)
        {
            return await _addressRepository.DeleteAsync(id);
        }

        // Private helper method for mapping
        private Address MapToAddressDto(AddressDb address)
        {
            return new Address
            {
                Id = address.Id,
                UserId = address.UserId,
                Label = address.Label,
                FullAddress = address.FullAddress,
                // IMPORTANT: Point.X is Longitude, Point.Y is Latitude
                Latitude = address.Location.Y,
                Longitude = address.Location.X
            };
        }
    }
}
