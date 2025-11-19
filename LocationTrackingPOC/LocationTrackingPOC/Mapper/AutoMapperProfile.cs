using LocationTrackingCommon.Models;
using LocationTrackingRepository.Models;
using AutoMapper;
using LocationTrackingPOC.DTO;

namespace LocationTrackingPOC.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDb>().ReverseMap();
            CreateMap<AddressDb, Address>();

            CreateMap<LocationDto, Address>().ReverseMap();
            CreateMap<DriverDto, Driver>().ReverseMap();
            CreateMap<DriverLocationDto, DriverLocation>().ReverseMap();
            CreateMap<LocationTrackingCommon.Models.DriverLocationUpdateDto, DriverLocation>();
            CreateMap<LocationTrackingCommon.Models.DriverLocationUpdateDto, LocationTrackingPOC.DTO.DriverLocationDto>().ReverseMap();
            CreateMap<DriverDb, Driver>().ReverseMap();
            CreateMap<LocationTrackingPOC.DTO.GeofenceCheckDto, LocationTrackingCommon.Models.GeofenceCheckDto>().ReverseMap();
            CreateMap<CollectionRequestDto, CollectionRequest>().ReverseMap();
            CreateMap<CollectionRequest, CollectionRequestDb>().ReverseMap();
        }
    }
}
