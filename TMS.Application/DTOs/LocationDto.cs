namespace TMS.Application.DTOs
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        // You can add CreatedAt/UpdatedAt if the frontend needs them
    }
}