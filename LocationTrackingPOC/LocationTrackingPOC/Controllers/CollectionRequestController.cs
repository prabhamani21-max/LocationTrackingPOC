using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using LocationTrackingCommon.Models;
using LocationTrackingPOC.DTO;
using LocationTrackingPOC.Hubs;
using LocationTrackingService.Interface;

namespace LocationTrackingPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionRequestController : ControllerBase
    {
        private readonly ICollectionRequestService _collectionRequestService;
        private readonly IMapper _mapper;
        private readonly IHubContext<LocationHub> _hubContext;
        private readonly ICurrentUser _currentUser;

        public CollectionRequestController(ICollectionRequestService collectionRequestService, IMapper mapper, IHubContext<LocationHub> hubContext, ICurrentUser currentUser)
        {
            _collectionRequestService = collectionRequestService;
            _mapper = mapper;
            _hubContext = hubContext;
            _currentUser = currentUser;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCollectionRequest([FromBody] CollectionRequestDto dto)
        {
            var collectionRequest = _mapper.Map<CollectionRequest>(dto);
            collectionRequest.CreatedDate = DateTime.UtcNow;
            collectionRequest.CreatedBy = _currentUser.UserId;
            collectionRequest.Status = LocationTrackingCommon.Models.CollectionStatusEnum.Requested;
            collectionRequest.RequestedAt = DateTime.UtcNow;

            var id = await _collectionRequestService.CreateCollectionRequestAsync(collectionRequest);
            return Ok(new { Message = "Collection request created successfully", Id = id });
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetCollectionRequestById(long id)
        {
            var collectionRequest = await _collectionRequestService.GetCollectionRequestByIdAsync(id);
            var dto = _mapper.Map<CollectionRequestDto>(collectionRequest);
            return Ok(dto);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCollectionRequest([FromBody] CollectionRequestDto dto)
        {
            var collectionRequest = _mapper.Map<CollectionRequest>(dto);
            await _collectionRequestService.UpdateCollectionRequestAsync(collectionRequest);
            return Ok(new { Message = "Collection request updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCollectionRequest(long id)
        {
            await _collectionRequestService.DeleteCollectionRequestAsync(id);
            return Ok(new { Message = "Collection request deleted successfully" });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCollectionRequests()
        {
            var collectionRequests = await _collectionRequestService.GetAllCollectionRequestsAsync();
            var dtos = _mapper.Map<IEnumerable<CollectionRequestDto>>(collectionRequests);
            return Ok(dtos);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetCollectionRequestsByUserId(long userId)
        {
            var collectionRequests = await _collectionRequestService.GetCollectionRequestsByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<CollectionRequestDto>>(collectionRequests);
            return Ok(dtos);
        }

        [HttpGet("by-driver/{driverId}")]
        public async Task<IActionResult> GetCollectionRequestsByDriverId(long driverId)
        {
            var collectionRequests = await _collectionRequestService.GetCollectionRequestsByDriverIdAsync(driverId);
            var dtos = _mapper.Map<IEnumerable<CollectionRequestDto>>(collectionRequests);
            return Ok(dtos);
        }

        [HttpGet("by-status/{statusId}")]
        public async Task<IActionResult> GetCollectionRequestsByStatus(int statusId)
        {
            var collectionRequests = await _collectionRequestService.GetCollectionRequestsByStatusAsync(statusId);
            var dtos = _mapper.Map<IEnumerable<CollectionRequestDto>>(collectionRequests);
            return Ok(dtos);
        }

        [HttpPost("assign-driver")]
        public async Task<IActionResult> AssignDriverToCollectionRequest([FromBody] AssignDriverRequest request)
        {
            await _collectionRequestService.AssignDriverToCollectionRequestAsync(request.CollectionRequestId, request.DriverId, request.AdminId);

            // Notify the driver
            await _hubContext.Clients.Group($"driver-{request.DriverId}").SendAsync("PickupAssigned", request.CollectionRequestId);

            return Ok(new { Message = "Driver assigned successfully" });
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateCollectionRequestStatus([FromBody] UpdateStatusRequest request)
        {
            await _collectionRequestService.UpdateCollectionRequestStatusAsync(request.CollectionRequestId, (int)request.StatusId, request.UpdatedBy);
            return Ok(new { Message = "Status updated successfully" });
        }
    }

    public class AssignDriverRequest
    {
        public long CollectionRequestId { get; set; }
        public long DriverId { get; set; }
        public long AdminId { get; set; }
    }

    public class UpdateStatusRequest
    {
        public long CollectionRequestId { get; set; }
        public LocationTrackingCommon.Models.CollectionStatusEnum StatusId { get; set; }
        public long UpdatedBy { get; set; }
    }
}