using AutoMapper;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using LocationTrackingService.Interface;

namespace LocationTrackingService.Implementation
{
    public class CollectionRequestService : ICollectionRequestService
    {
        private readonly ICollectionRequestRepository _collectionRequestRepository;
        private readonly IMapper _mapper;

        public CollectionRequestService(ICollectionRequestRepository collectionRequestRepository, IMapper mapper)
        {
            _collectionRequestRepository = collectionRequestRepository;
            _mapper = mapper;
        }

        public async Task<CollectionRequest> GetCollectionRequestByIdAsync(long id)
        {
            var collectionRequestDb = await _collectionRequestRepository.GetCollectionRequestByIdAsync(id);
            return _mapper.Map<CollectionRequest>(collectionRequestDb);
        }

        public async Task<long> CreateCollectionRequestAsync(CollectionRequest collectionRequest)
        {
            var collectionRequestDb = _mapper.Map<CollectionRequestDb>(collectionRequest);
            collectionRequestDb.CreatedDate = DateTime.UtcNow;
            return await _collectionRequestRepository.AddCollectionRequestAsync(collectionRequestDb);
        }

        public async Task UpdateCollectionRequestAsync(CollectionRequest collectionRequest)
        {
            var collectionRequestDb = _mapper.Map<CollectionRequestDb>(collectionRequest);
            await _collectionRequestRepository.UpdateCollectionRequestAsync(collectionRequestDb);
        }

        public async Task DeleteCollectionRequestAsync(long id)
        {
            await _collectionRequestRepository.DeleteCollectionRequestAsync(id);
        }

        public async Task<IEnumerable<CollectionRequest>> GetAllCollectionRequestsAsync()
        {
            var collectionRequestsDb = await _collectionRequestRepository.GetAllCollectionRequestsAsync();
            return _mapper.Map<IEnumerable<CollectionRequest>>(collectionRequestsDb);
        }

        public async Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByUserIdAsync(long userId)
        {
            var collectionRequestsDb = await _collectionRequestRepository.GetCollectionRequestsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CollectionRequest>>(collectionRequestsDb);
        }

        public async Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByDriverIdAsync(long driverId)
        {
            var collectionRequestsDb = await _collectionRequestRepository.GetCollectionRequestsByDriverIdAsync(driverId);
            return _mapper.Map<IEnumerable<CollectionRequest>>(collectionRequestsDb);
        }

        public async Task<IEnumerable<CollectionRequest>> GetCollectionRequestsByStatusAsync(int statusId)
        {
            var collectionRequestsDb = await _collectionRequestRepository.GetCollectionRequestsByStatusAsync(statusId);
            return _mapper.Map<IEnumerable<CollectionRequest>>(collectionRequestsDb);
        }

        public async Task AssignDriverToCollectionRequestAsync(long collectionRequestId, long driverId, long adminId)
        {
            var collectionRequestDb = await _collectionRequestRepository.GetCollectionRequestByIdAsync(collectionRequestId);
            if (collectionRequestDb == null)
            {
                throw new ArgumentException("Collection request not found");
            }

            collectionRequestDb.DriverId = driverId;
            collectionRequestDb.AssignedAt = DateTime.UtcNow;
            collectionRequestDb.Status = (int)LocationTrackingCommon.Models.CollectionStatusEnum.AssignedDriver;
            collectionRequestDb.UpdatedBy = adminId;
            collectionRequestDb.UpdatedDate = DateTime.UtcNow;

            await _collectionRequestRepository.UpdateCollectionRequestAsync(collectionRequestDb);
        }

        public async Task UpdateCollectionRequestStatusAsync(long collectionRequestId, int statusId, long updatedBy)
        {
            var collectionRequestDb = await _collectionRequestRepository.GetCollectionRequestByIdAsync(collectionRequestId);
            if (collectionRequestDb == null)
            {
                throw new ArgumentException("Collection request not found");
            }

            collectionRequestDb.Status = statusId;
            collectionRequestDb.UpdatedBy = updatedBy;
            collectionRequestDb.UpdatedDate = DateTime.UtcNow;

            if (statusId == (int)LocationTrackingCommon.Models.CollectionStatusEnum.Completed)
            {
                collectionRequestDb.CompletedAt = DateTime.UtcNow;
            }

            await _collectionRequestRepository.UpdateCollectionRequestAsync(collectionRequestDb);
        }
    }
}