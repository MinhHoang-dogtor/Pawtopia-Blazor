using System.ComponentModel.DataAnnotations;

namespace Pawtopia.Models
{

    public class Category
    {

        [Key]
        public string Id { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;
        public string NormalizedName { get; set; } = String.Empty;

        public string? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
    }
}
