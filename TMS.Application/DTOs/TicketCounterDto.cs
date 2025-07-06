using System;

namespace TMS.Application.DTOs
{
    public class TicketCounterDto
    {
        public Guid TicketCounterId { get; set; } // Renamed from Id
        public Guid LocationId { get; set; }
        public string CounterName { get; set; }
        public string CounterCode { get; set; }
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        //public decimal? Latitude { get; set; }
        //public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
        public LocationDto Location { get; set; } // Include related Location info
    }

    public class CreateTicketCounterDto
    {
        public Guid LocationId { get; set; }
        public string CounterName { get; set; }
        public string CounterCode { get; set; }
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateTicketCounterDto
    {
        public string CounterName { get; set; }
        public string CounterCode { get; set; }
        public string AddressDetails { get; set; }
        public string ContactNumber { get; set; }
        public string OperatingHours { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}