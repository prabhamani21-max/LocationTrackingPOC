using AutoMapper;
using LocationTrackingCommon.Models;
using LocationTrackingRepository.Data;
using LocationTrackingRepository.Interface;
using LocationTrackingRepository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        public async Task<long> RegisterUserAsync(User user)
        {
            var entity = _mapper.Map<UserDb>(user);
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            var user = await _context.Users.FindAsync(id);
            return _mapper.Map<User>(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return _mapper.Map<User>(user);
        }
    }
}
