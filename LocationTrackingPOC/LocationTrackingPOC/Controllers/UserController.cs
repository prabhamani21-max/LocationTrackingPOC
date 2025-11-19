using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocationTrackingCommon.Models;
using LocationTrackingPOC.DTO;
using LocationTrackingPOC.Helper;
using LocationTrackingRepository.Models;
using LocationTrackingService.Interface;

namespace LocationTrackingPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUser _currentUser;

        public UserController(IUserService userService, IMapper mapper, ITokenService tokenService, ICurrentUser currentUser)
        {

            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
            _currentUser = currentUser;

        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserDto dto)
        {
 

            var user = _mapper.Map<User>(dto);

            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = _currentUser.UserId;

            user.StatusId = 1; // Active status
            user.Password = PasswordHasher.HashPassword(user.Password);

            var id = await _userService.RegisterUserAsync(user);
            var newUser = await _userService.GetUserByIdAsync(id);
            // var userDto = _mapper.Map<UserDto>(newUser);
            return Ok(new { Message = "User registered successfully", UserId = id });
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {

            var user = await _userService.GetUserByIdAsync(id);


            var userDto = _mapper.Map<UserDto>(user);

            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userService.LoginAsync(dto.Email, dto.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _tokenService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

    }
}
