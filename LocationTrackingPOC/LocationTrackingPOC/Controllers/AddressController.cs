using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using LocationTrackingCommon.Models;
using LocationTrackingPOC.DTO;
using LocationTrackingService.Interface;
using System.Security.Claims;

namespace LocationTrackingPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        // GeometryFactory is used to create Point objects
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public AddressController(IAddressService addressService, IMapper mapper, ICurrentUser currentUser)
        {
            _addressService = addressService;
            // SRID 4326 is standard for GPS
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpPost("location")]
        public async Task<IActionResult> SaveUserAddress([FromBody] LocationDto dto)
        {
            var newAddress= _mapper.Map<Address>(dto);
            newAddress.UserId = _currentUser.UserId; // Use current user or default to 3
            // Call the service with the DTO and the secure User ID
            var newlocation = await _addressService.SaveNewAddressAsync(newAddress);
            return Ok(newlocation);


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddress(long id)
        {
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
                return NotFound();
            var dto = _mapper.Map<LocationDto>(address);
            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAddressesByUser([FromQuery] long userId)
        {
            var addresses = await _addressService.GetAddressesByUserIdAsync(userId);
            var dtos = _mapper.Map<IEnumerable<LocationDto>>(addresses);
            return Ok(dtos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(long id, [FromBody] LocationDto dto)
        {
            var address = _mapper.Map<Address>(dto);
            address.Id = id;
            var updatedAddress = await _addressService.UpdateAddressAsync(address);
            var updatedDto = _mapper.Map<LocationDto>(updatedAddress);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(long id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

    }
}
