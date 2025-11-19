using Microsoft.EntityFrameworkCore;
using LocationTrackingRepository.Data;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Implementation
{
    public class DriverRepository : IDriverRepository
    {
        private readonly AppDbContext _context;

        public DriverRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DriverDb?> GetDriverByIdAsync(long id)
        {
            return await _context.Drivers
        .Where(d => d.Id == id)
        .Select(d => new DriverDb
        {
            Id = d.Id,
            VehicleNumber = d.VehicleNumber,
            LicenseNumber = d.LicenseNumber,
            UserId = d.UserId,
            User = new UserDb
            {
                Id = d.User.Id,
                Name = d.User.Name,
                Email = d.User.Email,
                ContactNo = d.User.ContactNo
            }
        })
        .FirstOrDefaultAsync();
        }

        public async Task<long> AddDriverAsync(DriverDb driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver.Id;
        }

        public async Task UpdateDriverAsync(DriverDb driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDriverAsync(long id)
        {
            var driver = await GetDriverByIdAsync(id);
            if (driver != null)
            {
                _context.Drivers.Remove(driver);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DriverDb>> GetAllDriversAsync()
        {
            return await _context.Drivers
                .Include(d => d.User)
                .OrderBy(d => d.User.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverDb>> GetDriversByStatusAsync(int statusId)
        {
            return await _context.Drivers
                .Include(d => d.User)
                .Where(d => d.User.StatusId == statusId)
                .OrderBy(d => d.User.Name)
                .ToListAsync();
        }
    }
}