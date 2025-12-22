namespace Pawtopia.DTOs
{
    public class AddressDto
    {
        public string? Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Province { get; set; } = null!;
        public string District { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public string DetailAddress { get; set; } = null!;
        public bool IsDefault { get; set; }
    }
}