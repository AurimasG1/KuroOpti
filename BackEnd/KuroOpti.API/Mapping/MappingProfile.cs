using AutoMapper;
using KuroOpti.API.Requests;
using KuroOpti.API.Responses;
using KuroOpti.Entities;

namespace KuroOpti.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<RegistrationRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<FuelStation, FuelStationDto>();
            CreateMap<Entities.Route, RouteDto>().ReverseMap();
        }
    }
}
