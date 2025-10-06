using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Dtos;
using ProductsAPI.Models;


namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SuppliersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            var suppliers = await _context.Supplier.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<SupplierDto>>(suppliers));
        }

        // GET: api/suppliers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null) return NotFound();

            return Ok(_mapper.Map<SupplierDto>(supplier));
        }

        // POST: api/suppliers
        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create(SupplierDto dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);
            _context.Supplier.Add(supplier);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = supplier.SupplierID }, _mapper.Map<SupplierDto>(supplier));
        }

        // PUT: api/suppliers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SupplierDto dto)
        {
            if (id != dto.SupplierID) return BadRequest();

            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null) return NotFound();

            _mapper.Map(dto, supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/suppliers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null) return NotFound();

            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/suppliers/bulk → carga masiva de proveedores de prueba
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertSuppliers([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 1000)
                return BadRequest("El parámetro 'count' debe estar entre 1 y 1000.");

            var random = new Random();
            var existingNames = await _context.Supplier
                .Select(s => s.CompanyName)
                .ToHashSetAsync();

            var suppliers = new List<Supplier>();
            int attempts = 0;

            while (suppliers.Count < count && attempts < count * 2)
            {
                string companyName = $"Empresa {random.Next(1, 10000)}";

                if (existingNames.Contains(companyName))
                {
                    attempts++;
                    continue;
                }

                existingNames.Add(companyName); // evitar duplicados en esta misma carga

                suppliers.Add(new Supplier
                {
                    CompanyName = companyName,
                    ContactName = $"Contacto {suppliers.Count + 1}",
                    ContactTitle = "Gerente de compras",
                    Address = $"Calle {random.Next(1, 100)} #{random.Next(1, 50)}",
                    City = "Bogotá",
                    Region = "Cundinamarca",
                    PostalCode = $"{random.Next(10000, 99999)}",
                    Country = "Colombia",
                    Phone = $"300-{random.Next(1000000, 9999999)}",
                    Fax = $"601-{random.Next(2000000, 2999999)}",
                    HomePage = $"https://empresa{suppliers.Count + 1}.com"
                });

                attempts++;
            }

            if (suppliers.Count == 0)
                return BadRequest("No se pudieron generar proveedores únicos.");

            await _context.Supplier.AddRangeAsync(suppliers);
            await _context.SaveChangesAsync();

            return Ok($"{suppliers.Count} proveedores insertados sin duplicados.");
        }


    }
}
