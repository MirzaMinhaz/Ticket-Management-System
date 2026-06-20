namespace TMS.Application.DTOs
{
    public class UserProfileResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}