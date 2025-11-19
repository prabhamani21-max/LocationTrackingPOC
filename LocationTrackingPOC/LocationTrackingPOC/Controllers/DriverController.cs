using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using LocationTrackingPOC.DTO;
using LocationTrackingCommon.Models;
using LocationTrackingPOC.Hubs;
using LocationTrackingService.Interface;

namespace LocationTrackingPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IHubContext<LocationHub> _hubContext;
        private readonly IDriverService _driverService;
        private readonly ILocationTrackingService _locationTrackingService;
        private readonly ILogger<DriverController> _logger;
        private readonly IMapper _mapper;

        public DriverController(
            IHubContext<LocationHub> hubContext,
            IDriverService driverService,
            ILocationTrackingService locationTrackingService,
            IMapper mapper,
            ILogger<DriverController> logger)
        {
            _hubContext = hubContext;
            _driverService = driverService;
            _locationTrackingService = locationTrackingService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("location/update")]
        public async Task<IActionResult> UpdateDriverLocation([FromBody] LocationTrackingPOC.DTO.DriverLocationDto locationUpdate)
        {
            try
            {
                _logger.LogInformation("Received location update for driver {DriverId}", locationUpdate.DriverId);

                // Map to common model
                var commonLocationUpdate = _mapper.Map<LocationTrackingCommon.Models.DriverLocationUpdateDto>(locationUpdate);

                // Update location in Redis and broadcast via SignalR
                await _locationTrackingService.UpdateDriverLocationAsync(commonLocationUpdate);

                // Broadcast to all clients
                await _hubContext.Clients.Group($"driver-{locationUpdate.DriverId}").SendAsync("ReceiveDriverLocation", locationUpdate);
                _logger.LogInformation("Broadcasted location to all clients");


                return Ok(new { Message = "Location updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update location for driver {DriverId}", locationUpdate.DriverId);
                return BadRequest(new { Message = "Failed to update location", Error = ex.Message });
            }
        }

        [HttpPost("geofence/check")]
        public async Task<IActionResult> CheckGeofence([FromBody] LocationTrackingPOC.DTO.GeofenceCheckDto geofenceCheck)
        {
            try
            {
                var commonGeofenceCheck = _mapper.Map<LocationTrackingCommon.Models.GeofenceCheckDto>(geofenceCheck);
                var isWithinGeofence = await _locationTrackingService.CheckGeofenceAsync(commonGeofenceCheck);

                return Ok(new
                {
                    IsWithinGeofence = isWithinGeofence,
                    CheckType = geofenceCheck.CheckType,
                    DriverId = geofenceCheck.DriverId,
                    TargetLocationId = geofenceCheck.TargetLocationId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Geofence check failed", Error = ex.Message });
            }
        }

        [HttpGet("location/{driverId}")]
        public async Task<IActionResult> GetDriverCurrentLocation(long driverId)
        {
            try
            {
                var location = await _locationTrackingService.GetDriverCurrentLocationAsync(driverId);
                return Ok(location);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "Driver location not found", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDriverById(long id)
        {
            try
            {
                var driver = await _driverService.GetDriverByIdAsync(id);
                var driverDto = _mapper.Map<DriverDto>(driver);

                return Ok(driverDto);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = "Driver not found", Error = ex.Message });
            }
        }

        [HttpPost("{driverId}/status")]
        public async Task<IActionResult> UpdateDriverStatus(long driverId, [FromBody] int status)
        {
            try
            {
                await _driverService.UpdateDriverStatusAsync(driverId, status);
                return Ok(new { Message = "Driver status updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to update driver status", Error = ex.Message });
            }
        }

        [HttpGet("{driverId}/status")]
        public async Task<IActionResult> GetDriverStatus(long driverId)
        {
            try
            {
                var status = await _driverService.GetDriverStatusAsync(driverId);
                if (status == null)
                {
                    return NotFound(new { Message = "Driver status not found" });
                }
                return Ok(new { DriverId = driverId, Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to get driver status", Error = ex.Message });
            }
        }

        [HttpPost("{driverId}/ping")]
        public async Task<IActionResult> PingDriver(long driverId)
        {
            try
            {
                await _locationTrackingService.PingDriverAsync(driverId);
                return Ok(new { Message = "Driver ping recorded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to record driver ping", Error = ex.Message });
            }
        }
    }
}