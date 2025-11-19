using LocationTrackingCommon.Models;

namespace LocationTrackingService.Interface
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}