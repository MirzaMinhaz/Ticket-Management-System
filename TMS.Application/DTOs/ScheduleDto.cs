using System;
using System.ComponentModel.DataAnnotations;

namespace TMS.Application.DTOs.Schedule
{
    // For creating a new Schedule
    public class CreateScheduleDto
    {
        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; } // e.g., Scheduled, Departed, Cancelled
    }

    // For updating an existing Schedule
    public class UpdateScheduleDto
    {
        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; }
    }

    // For reading/displaying Schedule details
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int VehicleId { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseFare { get; set; }
        public string Status { get; set; }
        public string ScheduleCode { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }

        // Optional: lightweight navigation info (instead of full entity)
        public string RouteName { get; set; }
        public string VehicleName { get; set; }
    }

    // For the "Recent Fare Updates" table on the Fare Matrix screen
    public class FareHistoryDto
    {
        public long Id { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public decimal PreviousFare { get; set; }
        public decimal CurrentFare { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // For updating only the base fare of a Schedule (used by the Fare Matrix screen)
    public class UpdateFareDto
    {
        [Range(typeof(decimal), "0", "4999.99", ErrorMessage = "Fare must be less than 5000.")]
        public decimal BaseFare { get; set; }
    }
}