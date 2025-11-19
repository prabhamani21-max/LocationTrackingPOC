using LocationTrackingCommon.Models;

namespace LocationTrackingService.Interface
{
    public interface ILocationTrackingService
    {
        Task UpdateDriverLocationAsync(DriverLocationUpdateDto locationUpdate);
        Task<DriverLocationUpdateDto?> GetDriverCurrentLocationAsync(long driverId);
        Task PingDriverAsync(long driverId);
        Task<bool> CheckGeofenceAsync(GeofenceCheckDto geofenceCheck);
    }
}