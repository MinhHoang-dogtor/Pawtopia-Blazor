using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class VariationOption
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string Value { get; set; } = String.Empty;

        public string VariationId { get; set; } = String.Empty;
        public Variation Variation { get; set; } = default!;
    }
}
