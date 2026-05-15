using AutoMapper;
using TMS.Application.DTOs;
using TMS.Application.DTOs.Route;
using TMS.Application.DTOs.Schedule;
using TMS.Application.DTOs.Ticket;
using TMS.Domain.Entities;

namespace TMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Location Mappings
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            // Vehicle Mappings 
            CreateMap<Vehicle, VehicleDto>().ReverseMap();

            CreateMap<CreateVehicleDto, Vehicle>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());

            CreateMap<UpdateVehicleDto, Vehicle>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleCode, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
            // TicketCounter Mappings
            CreateMap<TicketCounter, TicketCounterDto>().ReverseMap();
            CreateMap<CreateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CounterCode, opt => opt.Ignore());
            CreateMap<UpdateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CounterCode, opt => opt.Ignore());

            // Route Mappings
            CreateMap<Route, RouteDto>().ReverseMap();
            CreateMap<CreateRouteDto, Route>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateRouteDto, Route>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // ✅ Schedule Mappings
            CreateMap<Schedule, ScheduleDto>()
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route.RouteName))
                .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Model));

            CreateMap<CreateScheduleDto, Schedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleCode, opt => opt.Ignore()); // generated in service

            CreateMap<UpdateScheduleDto, Schedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ScheduleCode, opt => opt.Ignore()); // not updated by DTO

            CreateMap<Ticket, TicketDto>().ReverseMap();
            CreateMap<CreateTicketDto, Ticket>();
            CreateMap<UpdateTicketDto, Ticket>();

            // In MappingProfile.cs constructor, add these lines:
            CreateMap<Trip, TripDto>();
            CreateMap<CreateTripDto, Trip>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Schedule, opt => opt.Ignore());

            //CreateMap<Schedule, ScheduleDto>().ReverseMap();
        }
    }
}
