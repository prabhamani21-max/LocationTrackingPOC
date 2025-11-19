using AutoMapper;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using LocationTrackingService.Interface;

namespace LocationTrackingService.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;
        private readonly StackExchange.Redis.IDatabase _redisDatabase;
        private readonly ICurrentUser _currentUser;

        public DriverService(IDriverRepository driverRepository, IMapper mapper, StackExchange.Redis.IDatabase redisDatabase, ICurrentUser currentUser)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
            _redisDatabase = redisDatabase;
            _currentUser = currentUser;
        }

        public async Task<Driver> GetDriverByIdAsync(long id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            return _mapper.Map<Driver>(driver);
        }

        public async Task<long> RegisterDriverAsync(Driver driverDto)
        {
            var driver = _mapper.Map<DriverDb>(driverDto);
            driver.CreatedDate = DateTime.UtcNow;
            driver.CreatedBy = _currentUser.UserId; // Use current user or default to 1

            return await _driverRepository.AddDriverAsync(driver);
        }

        public async Task UpdateDriverStatusAsync(long driverId, int status)
        {
            if (status < 1 || status > 5)
            {
                throw new ArgumentException("Invalid status. Must be 1 (Online), 2 (Offline), or 3 (Busy)");
            }

            var key = $"driver:{driverId}:status";
            await _redisDatabase.StringSetAsync(key, status.ToString());
        }

        public async Task<int?> GetDriverStatusAsync(long driverId)
        {
            var key = $"driver:{driverId}:status";
            var status = await _redisDatabase.StringGetAsync(key);
            return status.HasValue && int.TryParse(status.ToString(), out var statusInt) ? statusInt : null;
        }
    }
}