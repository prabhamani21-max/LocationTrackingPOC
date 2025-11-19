using LocationTrackingCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingService.Interface
{
    public interface IUserService
    {
        Task<long> RegisterUserAsync(User user);
        Task<User> GetUserByIdAsync(long id);
        Task<User> LoginAsync(string email, string password);
    }
}
