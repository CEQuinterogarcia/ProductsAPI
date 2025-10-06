using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public string CustomerID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        [Required]
        public int ShipVia { get; set; }

        [Required]
        public decimal Freight { get; set; }

        [Required]
        public string ShipName { get; set; }

        [Required]
        public string ShipAddress { get; set; }

        [Required]
        public string ShipCity { get; set; }

        [Required]
        public string ShipRegion { get; set; }

        [Required]
        public string ShipPostalCode { get; set; }

        [Required]
        public string ShipCountry { get; set; }

        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Shipper Shipper { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
