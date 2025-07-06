using AutoMapper;
using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Location Mappings
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.Id)) // Map BaseEntity.Id to LocationDto.LocationId
                .ReverseMap(); // Allows mapping back from DTO to Entity

            CreateMap<CreateLocationDto, Location>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()); // EF will generate ID on add

            CreateMap<UpdateLocationDto, Location>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID handled by route parameter

            // TicketCounter Mappings (NEW)
            CreateMap<TicketCounter, TicketCounterDto>()
                .ForMember(dest => dest.TicketCounterId, opt => opt.MapFrom(src => src.Id)) // Map BaseEntity.Id to TicketCounterDto.TicketCounterId
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location)) // Map nested Location
                .ReverseMap();

            CreateMap<CreateTicketCounterDto, TicketCounter>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()); // EF will generate ID on add

            CreateMap<UpdateTicketCounterDto, TicketCounter>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()); // ID handled by route parameter
        }
    }
}