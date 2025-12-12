using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = String.Empty;

        public string FullName { get; set; } = String.Empty;
        public string PhoneNumber { get; set; } = String.Empty;
        public string AddressLine { get; set; } = String.Empty;
        public string Ward { get; set; } = String.Empty;
        public string Province { get; set; } = String.Empty;

        public string UserId { get; set; } = String.Empty;
        public User User { get; set; } = default!;
    }
}
