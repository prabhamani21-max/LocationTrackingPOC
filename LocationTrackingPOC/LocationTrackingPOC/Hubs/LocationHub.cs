using Microsoft.AspNetCore.SignalR;
using LocationTrackingCommon.Models;

namespace LocationTrackingPOC.Hubs
{
    public class LocationHub : Hub
    {
        public async Task UpdateDriverLocation(DriverLocationUpdateDto locationUpdate)
        {
            // Broadcast the location update to all connected clients
            await Clients.All.SendAsync("ReceiveDriverLocation", locationUpdate);
        }

        public async Task SubscribeToDriver(long driverId)
        {
            // Add client to a group for specific driver updates
            await Groups.AddToGroupAsync(Context.ConnectionId, $"driver-{driverId}");
        }

        public async Task UnsubscribeFromDriver(long driverId)
        {
            // Remove client from a group for specific driver updates
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"driver-{driverId}");
        }

        public async Task SubscribeToUser(long userId)
        {
            // Add client to a group for specific user ride updates
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task UnsubscribeFromUser(long userId)
        {
            // Remove client from a group for specific user ride updates
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        public async Task SubscribeToDrivers()
        {
            // Add client to drivers group for receiving ride requests
            await Groups.AddToGroupAsync(Context.ConnectionId, "drivers");
        }

        public async Task UnsubscribeFromDrivers()
        {
            // Remove client from drivers group
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "drivers");
        }
    }
}