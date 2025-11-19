using LocationTrackingCommon.Models;

namespace LocationTrackingService.Interface
{
    public interface ICollectionRequestService
    {
        Task<CollectionRequest> GetCollectionRequestByIdAsync(long id);
        Task<long> CreateCollectionRequestAsync(CollectionRequest collectionRequest);
        Task UpdateCollectionRequestAsync(CollectionRequest collectionRequest);
        Task DeleteCollectionRequestAsync(long id);
        Task<IEnumerable<CollectionRequest>> GetAllCollectionRequestsAsync();
        Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByUserIdAsync(long userId);
        Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByDriverIdAsync(long driverId);
        Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByStatusAsync(int statusId);
        Task AssignDriverToCollectionRequestAsync(long collectionRequestId, long driverId, long adminId);
        Task UpdateCollectionRequestStatusAsync(long collectionRequestId, int statusId, long updatedBy);
    }
}