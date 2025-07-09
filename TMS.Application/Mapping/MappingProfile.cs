// In your AutoMapper Profile (e.g., MappingProfile.cs)
using AutoMapper;
using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Location Mappings (assuming you'll apply similar int ID change to Location)
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();


            // TicketCounter Mappings
            CreateMap<TicketCounter, TicketCounterDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Explicitly map Id from BaseEntity
                                                                               // If you want LocationName in DTO, ensure Location is included in query
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location != null ? src.Location.Name : null))
                .ReverseMap(); // Allows mapping DTO back to Entity for updates

            CreateMap<CreateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Database generates Id
            CreateMap<UpdateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id is from route, not DTO
            // For updates, the mapper will update existingTicketCounter based on the DTO.
            // The existingTicketCounter already has its Id.
        }
    }
}