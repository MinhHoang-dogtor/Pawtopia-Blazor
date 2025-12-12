using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class Variation
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;
    }
}
