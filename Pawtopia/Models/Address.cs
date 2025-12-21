using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pawtopia.Models
{
    public class Address
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string FullName { get; set; } = null!; 

        public string PhoneNumber { get; set; } = null!; 

        public string AddressLine { get; set; } = null!; 

        public string Ward { get; set; } = null!;

        public string Province { get; set; } = null!;

        [ForeignKey("User")]
        public string UserId { get; set; } = null!; 
        public virtual User User { get; set; } = null!;
    }
}