// TMS.Application/Mapping/MappingProfile.cs
using AutoMapper;
using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Location Mappings
            CreateMap<Location, LocationDto>().ReverseMap(); // LocationDto has locationId: string
            CreateMap<CreateLocationDto, Location>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id will be generated in service
            CreateMap<UpdateLocationDto, Location>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id not updated by DTO

            // TicketCounter Mappings
            CreateMap<TicketCounter, TicketCounterDto>().ReverseMap(); // TicketCounterDto has ticketCounterId: string and locationId: string
            CreateMap<CreateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id will be generated in service
                .ForMember(dest => dest.CounterCode, opt => opt.Ignore()); // CounterCode generated in service
            CreateMap<UpdateTicketCounterDto, TicketCounter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id not updated by DTO
                .ForMember(dest => dest.CounterCode, opt => opt.Ignore()); // CounterCode not updated by DTO
        }
    }
}