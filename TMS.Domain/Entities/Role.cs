// TMS.Domain/Entities/Role.cs
using System;
// Remove any other using statements if not explicitly needed, e.g., System.Collections.Generic;

namespace TMS.Domain.Entities // <-- VERY IMPORTANT: Ensure this namespace is correct
{
    public class Role : BaseEntity // Inherit from BaseEntity which gives it the 'Id' property
    {
        // No other properties are needed here if your DB table only has RoleId
        // The 'Id' property from BaseEntity will be mapped to 'RoleId' column in DB
        // via the convention you set in OnModelCreating (entityType.DisplayName() + "Id")
    }
}