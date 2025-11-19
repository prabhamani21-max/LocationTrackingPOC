namespace LocationTrackingService.Interface
{
    public interface ICurrentUser
    {
        public long UserId { get; set; }
       public  string Email { get; set; }
       public string Name { get; set; }
       public int RoleId { get; set; }
    }
}