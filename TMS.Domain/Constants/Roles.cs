using System.Linq;

namespace TMS.Domain.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string StationAgent = "StationAgent";
        public const string CounterAgent = "CounterAgent";
        public const string Customer = "Customer";

        // Roles an Admin is allowed to assign via the staff-registration endpoint.
        // Admin is deliberately excluded — creating more Admins should be a rarer,
        // more deliberate action than day-to-day staff onboarding.
        public static readonly string[] AssignableStaffRoles = { Manager, StationAgent, CounterAgent };

        public static bool IsValidStaffRole(string role) =>
            AssignableStaffRoles.Contains(role);
    }
}