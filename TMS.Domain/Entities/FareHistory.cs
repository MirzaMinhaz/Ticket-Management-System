namespace TMS.Domain.Entities
{
    public class FareHistory
    {
        public long Id { get; set; }

        public int ScheduleId { get; set; }          // must match the type of Schedules.Id
        public Schedule Schedule { get; set; } = null!;

        // Snapshots, so history stays correct even if names change later
        public string OperatorName { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public string RouteCode { get; set; } = string.Empty;

        public decimal PreviousFare { get; set; }    // must match the type of Schedules.BaseFare
        public decimal CurrentFare { get; set; }

        // Same audit fields as your other tables
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}