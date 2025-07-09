// TMS.Domain/Entities/Role.cs
using System;
using System.Collections.Generic; // Used if Role has collection properties

namespace TMS.Domain.Entities
{
    public class Role : BaseEntity<int> // Inherit from BaseEntity<int>
    {
        // Primary key is 'Id' from BaseEntity<int>

        public string RoleName { get; set; } // e.g., "Admin", "User", "Ticket Agent"
                                             // You might also add a RoleCode if needed (e.g., ROL-001)

        // If a role can have a collection of Users, add it here
        // public ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}