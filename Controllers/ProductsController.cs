using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;
using ProductsAPI.Dtos;
using AutoMapper;


namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST /Category → crear categorías "SERVIDORES" y "CLOUD"
        [HttpPost("category/init")]
        public async Task<IActionResult> CreateDefaultCategories()
        {
            var names = new[] { "SERVIDORES", "CLOUD" };
            foreach (var name in names)
            {
                if (!await _context.Category.AnyAsync(c => c.CategoryName == name))
                {
                    _context.Category.Add(new Category
                    {
                        CategoryName = name,
                        Description = $"Categoría para {name.ToLower()}",
                        Picture = null
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Categorías 'SERVIDORES' y 'CLOUD' creadas.");
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _context.Product
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(ProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.ProductID }, _mapper.Map<ProductDto>(product));
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto dto)
        {
            if (id != dto.ProductID) return BadRequest();

            var product = await _context.Product.FindAsync(id);
            if (product == null) return NotFound();

            _mapper.Map(dto, product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return NotFound();

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/products/bulk → insertar productos de prueba
        // POST /api/products/bulk → carga masiva de productos aleatorios
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertProducts([FromQuery] int count = 500)
        {
            var random = new Random();
            var servidores = await _context.Category.FirstOrDefaultAsync(c => c.CategoryName == "SERVIDORES");
            var cloud = await _context.Category.FirstOrDefaultAsync(c => c.CategoryName == "CLOUD");

            if (servidores == null || cloud == null)
                return BadRequest("Las categorías 'SERVIDORES' y 'CLOUD' deben existir.");

            var suppliers = await _context.Supplier.Select(s => s.SupplierID).ToListAsync();
            var categories = new[] { servidores.CategoryID, cloud.CategoryID };

            var products = new List<Product>(count);
            for (int i = 0; i < count; i++)
            {
                products.Add(new Product
                {
                    ProductName = $"Producto {i + 1}",
                    SupplierID = suppliers[random.Next(suppliers.Count)],
                    CategoryID = categories[random.Next(categories.Length)],
                    QuantityPerUnit = $"{random.Next(1, 10)} unidades",
                    UnitPrice = (decimal)(random.NextDouble() * 100),
                    UnitsInStock = (short)random.Next(0, 100),
                    UnitsOnOrder = (short)random.Next(0, 50),
                    ReorderLevel = (short)random.Next(1, 20),
                    Discontinued = false
                });
            }

            await _context.Product.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            return Ok($"{count} productos insertados Correctamente.");
        }

        // GET /api/products → paginación, búsqueda y filtros
        [HttpGet("fil")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            [FromQuery] string search = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Product
                .Where(p => string.IsNullOrEmpty(search) || p.ProductName.Contains(search))
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var products = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        // GET /api/products/{id} → detalle con imagen de categoría
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var product = await _context.Product
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null) return NotFound();

            return Ok(new
            {
                Product = _mapper.Map<ProductDto>(product),
                CategoryImage = product.Category?.Picture != null
                    ? Convert.ToBase64String(product.Category.Picture)
                    : null
            });
        }

       


    }
}
