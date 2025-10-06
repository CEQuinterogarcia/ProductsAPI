using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        public string? ProductName { get; set; }

        [Required]
        public int SupplierID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public string? QuantityPerUnit { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public short UnitsInStock { get; set; }

        [Required]
        public short UnitsOnOrder { get; set; }

        [Required]
        public short ReorderLevel { get; set; }

        [Required]
        public bool Discontinued { get; set; }

        public Supplier? Supplier { get; set; }
        public Category? Category { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
