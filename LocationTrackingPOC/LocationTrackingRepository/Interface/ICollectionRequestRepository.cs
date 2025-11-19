using LocationTrackingRepository.Models;

namespace LocationTrackingRepository.Interface
{
    public interface ICollectionRequestRepository
    {
        Task<CollectionRequestDb?> GetCollectionRequestByIdAsync(long id);
        Task<long> AddCollectionRequestAsync(CollectionRequestDb collectionRequest);
        Task UpdateCollectionRequestAsync(CollectionRequestDb collectionRequest);
        Task DeleteCollectionRequestAsync(long id);
        Task<IEnumerable<CollectionRequestDb>> GetAllCollectionRequestsAsync();
        Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByUserIdAsync(long userId);
        Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByDriverIdAsync(long driverId);
        Task<IEnumerable<CollectionRequestDb>> GetCollectionRequestsByStatusAsync(int statusId);
    }
}