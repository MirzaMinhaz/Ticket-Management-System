using AutoMapper;
using TMS.Application.DTOs;
using TMS.Domain.Entities;

namespace TMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create a bidirectional map between Location entity and LocationDto
            CreateMap<Location, LocationDto>().ReverseMap();
        }
    }
}