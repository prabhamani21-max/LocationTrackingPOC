using AutoMapper;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Interface;
using LocationTrackingService.Interface;
using System.Security.Cryptography;
using System.Text;

namespace LocationTrackingService.Implementation
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<long> RegisterUserAsync(User user)
        {
            return await _repository.RegisterUserAsync(user);
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _repository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var hashedPassword = HashPassword(password);
            if (hashedPassword != user.Password)
            {
                return null;
            }

            return user;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
