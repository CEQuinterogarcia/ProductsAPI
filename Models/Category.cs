using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        [Required]
        public string? Description { get; set; }

        public byte[]? Picture { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
