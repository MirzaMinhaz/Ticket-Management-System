using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Application.DTOs.Route
{
    // For creating a new Route
    public class CreateRouteDto
    {
        public string DepartureLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string RouteName { get; set; }
        public decimal EstimatedDurationHours { get; set; }
        public string RouteCode { get; set; }
        public string CreatedBy { get; set; } // Audit field
    }

    // For updating an existing Route
    public class UpdateRouteDto
    {
        public string DepartureLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string RouteName { get; set; }
        public decimal EstimatedDurationHours { get; set; }
        public string RouteCode { get; set; }
        public string LastModifiedBy { get; set; } // Audit field
    }

    // For reading/displaying Route details
    public class RouteDto
    {
        public int Id { get; set; }
        public string DepartureLocationCode { get; set; }
        public string DestinationLocationCode { get; set; }
        public string RouteName { get; set; }
        public decimal EstimatedDurationHours { get; set; }
        public string RouteCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}

