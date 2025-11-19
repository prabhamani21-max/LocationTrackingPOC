using LocationTrackingCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Interface
{
    public interface IUserRepository
    {
        Task<long> RegisterUserAsync(User user);
        Task<User> GetUserByIdAsync(long id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
