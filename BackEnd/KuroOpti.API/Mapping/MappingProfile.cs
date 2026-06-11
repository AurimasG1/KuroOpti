using System.Text.Json;
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
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<FuelStation, FuelStationDto>().ReverseMap();
            CreateMap<Entities.Route, RouteDto>().ReverseMap();
            CreateMap<UserRouteStation, FuelStationDto>();
            CreateMap<RoutePlanningHistory, RoutePlanningHistoryDto>()
                .ForMember(d => d.Stations, opt => opt.MapFrom(src => src.Stations));
            CreateMap<CreateRouteRequest, Entities.Route>()
                .ForMember(dest => dest.Polyline, opt => opt.MapFrom(src => src.Polyline));
        }
    }
}
