using System.ComponentModel.DataAnnotations;

namespace ProductsAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? TitleOfCourtesy { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        [Required]
        public string? Address { get; set; }

        [Required]
        public string? City { get; set; }

        [Required]
        public string? Region { get; set; }

        [Required]
        public string? PostalCode { get; set; }

        [Required]
        public string? Country { get; set; }

        [Required]
        public string? HomePhone { get; set; }

        [Required]
        public string? Extension { get; set; }

        public byte[]? Photo { get; set; }
        public string? Notes { get; set; }
        public int? ReportsTo { get; set; }

        public Employee? Manager { get; set; }
        public ICollection<Employee>? Subordinates { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
