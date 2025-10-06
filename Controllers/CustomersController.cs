using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ProductsAPI.Data;
using ProductsAPI.Dtos;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _context.Customer.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(string id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null) return NotFound();

            return Ok(_mapper.Map<CustomerDto>(customer));
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(CustomerDto dto)
        {
            if (await _context.Customer.AnyAsync(c => c.CustomerID == dto.CustomerID))
                return BadRequest("Ya existe un cliente con ese CustomerID.");

            var customer = _mapper.Map<Customer>(dto);
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerID }, _mapper.Map<CustomerDto>(customer));
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, CustomerDto dto)
        {
            if (id != dto.CustomerID) return BadRequest();

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null) return NotFound();

            _mapper.Map(dto, customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/customers/bulk → insertar clientes de prueba
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertCustomers([FromQuery] int count = 10)
        {
            var random = new Random();
            var customers = new List<Customer>();

            for (int i = 0; i < count; i++)
            {
                string customerId = $"CUST{i + 1:D4}";
                if (await _context.Customer.AnyAsync(c => c.CustomerID == customerId))
                    continue;

                customers.Add(new Customer
                {
                    CustomerID = customerId,
                    CompanyName = $"Empresa {i + 1}",
                    ContactName = $"Contacto {i + 1}",
                    ContactTitle = "Gerente",
                    Address = $"Calle {random.Next(1, 100)} #{random.Next(1, 50)}",
                    City = "Bogotá",
                    Region = "Cundinamarca",
                    PostalCode = $"{random.Next(10000, 99999)}",
                    Country = "Colombia",
                    Phone = $"300-{random.Next(1000000, 9999999)}",
                    Fax = $"601-{random.Next(2000000, 2999999)}"
                    
                });
            }

            await _context.Customer.AddRangeAsync(customers);
            await _context.SaveChangesAsync();

            return Ok($"{customers.Count} clientes insertados.");
        }

    }
}
