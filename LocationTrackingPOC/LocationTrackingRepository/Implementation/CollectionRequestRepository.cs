using Microsoft.EntityFrameworkCore;
using LocationTrackingRepository.Data;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Implementation
{
    public class CollectionRequestRepository : ICollectionRequestRepository
    {
        private readonly AppDbContext _context;

        public CollectionRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CollectionRequestDb?> GetCollectionRequestByIdAsync(long id)
        {
            return await _context.Rides
                .Include(cr => cr.User)
                .Include(cr => cr.Driver)
                .Include(cr => cr.PickupLocation)
                .Include(cr => cr.CollectionStatus)
                .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        public async Task<long> AddCollectionRequestAsync(CollectionRequestDb collectionRequest)
        {
            _context.Rides.Add(collectionRequest);
            await _context.SaveChangesAsync();
            return collectionRequest.Id;
        }

        public async Task UpdateCollectionRequestAsync(CollectionRequestDb collectionRequest)
        {
            _context.Rides.Update(collectionRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCollectionRequestAsync(long id)
        {
            var collectionRequest = await GetCollectionRequestByIdAsync(id);
            if (collectionRequest != null)
            {
                _context.Rides.Remove(collectionRequest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CollectionRequestDb>> GetAllCollectionRequestsAsync()
        {
            return await _context.Rides
                .Include(cr => cr.User)
                .Include(cr => cr.Driver)
                .Include(cr => cr.PickupLocation)
                .Include(cr => cr.CollectionStatus)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByUserIdAsync(long userId)
        {
            return await _context.Rides
                .Include(cr => cr.User)
                .Include(cr => cr.Driver)
                .Include(cr => cr.PickupLocation)
                .Include(cr => cr.CollectionStatus)
                .Where(cr => cr.UserId == userId)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByDriverIdAsync(long driverId)
        {
            return await _context.Rides
                .Include(cr => cr.User)
                .Include(cr => cr.Driver)
                .Include(cr => cr.PickupLocation)
                .Include(cr => cr.CollectionStatus)
                .Where(cr => cr.DriverId == driverId)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByStatusAsync(int statusId)
        {
            return await _context.Rides
                .Include(cr => cr.User)
                .Include(cr => cr.Driver)
                .Include(cr => cr.PickupLocation)
                .Include(cr => cr.CollectionStatus)
                .Where(cr => cr.Status == statusId)
                .OrderByDescending(cr => cr.RequestedAt)
                .ToListAsync();
        }
    }
}