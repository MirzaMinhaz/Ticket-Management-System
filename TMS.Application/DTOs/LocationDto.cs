using System;

namespace TMS.Application.DTOs
{
    public class LocationDto
    {
        public Guid LocationId { get; set; } // Renamed from Id
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        //public decimal? Latitude { get; set; } // Nullable
        //public decimal? Longitude { get; set; } // Nullable
    }

    public class CreateLocationDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        //public decimal? Latitude { get; set; }
        //public decimal? Longitude { get; set; }
    }

    public class UpdateLocationDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        //public decimal? Latitude { get; set; }
        //public decimal? Longitude { get; set; }
    }
}