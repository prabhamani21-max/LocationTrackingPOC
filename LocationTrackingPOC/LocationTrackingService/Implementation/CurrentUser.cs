using LocationTrackingService.Interface;

namespace LocationTrackingService.Implementation
{
    public class CurrentUser : ICurrentUser
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int RoleId { get; set; }
    }
}