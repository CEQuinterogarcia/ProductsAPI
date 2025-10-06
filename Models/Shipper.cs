using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Shipper
    {
        [Key]
        public int ShipperID { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Phone { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
