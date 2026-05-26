using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Common.Requests;
using KuroOpti.Entities;

namespace KuroOpti.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<User, UserDto>();
            CreateMap<RegistrationRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // FuelStation
            CreateMap<FuelStation, FuelStationDto>();
            CreateMap<FuelStationDto, FuelStation>().ForMember(x => x.Id, opt => opt.Ignore()); // saugiau

            // Route

            CreateMap<Entities.Route, RouteDto>();

            CreateMap<RouteDto, Entities.Route>()
                .ForMember(r => r.UserId, opt => opt.Ignore())
                .ForMember(r => r.User, opt => opt.Ignore())
                .ForMember(r => r.SelectedStations, opt => opt.Ignore())
                .ForMember(r => r.SearchLogs, opt => opt.Ignore());

            // UserRouteStation → FuelStationDto
            CreateMap<UserRouteStation, FuelStationDto>()
                .ConvertUsing(src => new FuelStationDto
                {
                    Id = src.FuelStation.Id,
                    Name = src.FuelStation.Name,
                    Municipality = src.FuelStation.Municipality,
                    Address = src.FuelStation.Address,
                    DieselPrice = src.FuelStation.DieselPrice,
                    PetrolPrice = src.FuelStation.PetrolPrice,
                    LpgPrice = src.FuelStation.LpgPrice,
                    Latitude = src.FuelStation.Latitude,
                    Longitude = src.FuelStation.Longitude,
                });
        }
    }
}
