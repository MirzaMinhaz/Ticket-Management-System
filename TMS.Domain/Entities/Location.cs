using TMS.Domain.Entities; // Ensure this using statement is present

namespace TMS.Domain.Entities
{
    public class Location : BaseEntity
    {
        public string Name { get; private set; }
        public string Type { get; private set; } // e.g., "City", "Bus Terminal", "Airport"
        public string Address { get; private set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        // Private constructor for EF Core and controlled creation via factory methods
        private Location() { }

        // Factory method to create a new Location (enforces invariants)
        public static Location Create(string name, string type, string address, double? latitude, double? longitude)
        {
            // Perform domain validation here
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Location name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Location type cannot be empty.", nameof(type));

            return new Location
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        // Method to update properties of an existing Location (enforces invariants)
        public void Update(string name, string type, string address, double? latitude, double? longitude)
        {
            // Perform domain validation before updating
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Location name cannot be empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Location type cannot be empty.", nameof(type));

            Name = name;
            Type = type;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}