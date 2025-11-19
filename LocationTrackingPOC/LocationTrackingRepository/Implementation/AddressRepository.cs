using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Data;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using System.Linq;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Implementation
{
    public class AddressRepository: IAddressRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddressRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AddressDb> CreateAsync(AddressDb address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<AddressDb?> GetAddressByIdAsync(long id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task<IEnumerable<AddressDb>> GetAddressesByUserIdAsync(long userId)
        {
            return await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        public async Task<AddressDb> UpdateAsync(AddressDb address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return false;
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckGeofencePickupAsync(double driverLat, double driverLon, long targetLocationId, double bufferMeters)
        {
            var driverLocation = new Point(driverLon, driverLat) { SRID = 4326 };
            return await _context.Addresses
                .AnyAsync(a => a.Id == targetLocationId && a.Location.Distance(driverLocation) <= bufferMeters);
        }

        public async Task<bool> CheckGeofenceDropoffAsync(double driverLat, double driverLon, long targetLocationId, double bufferMeters)
        {
            var driverLocation = new Point(driverLon, driverLat) { SRID = 4326 };
            return await _context.Addresses
                .AnyAsync(a => a.Id == targetLocationId && a.Location.Distance(driverLocation) <= bufferMeters);
        }
    }
}
