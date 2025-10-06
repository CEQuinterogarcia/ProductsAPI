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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Shipper)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _context.Order
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.Shipper)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null) return NotFound();

            return Ok(_mapper.Map<OrderDto>(order));
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(OrderDto dto)
        {
            var order = _mapper.Map<Order>(dto);
            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderID }, _mapper.Map<OrderDto>(order));
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDto dto)
        {
            if (id != dto.OrderID) return BadRequest();

            var order = await _context.Order.FindAsync(id);
            if (order == null) return NotFound();

            _mapper.Map(dto, order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null) return NotFound();

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/orders/bulk → insertar órdenes de prueba
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertOrders([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 1000)
                return BadRequest("El parámetro 'count' debe estar entre 1 y 1000.");

            var customerIds = await _context.Customer
                .Select(c => c.CustomerID)
                .ToListAsync();

            if (!customerIds.Any())
                return BadRequest("No hay clientes disponibles en la base de datos.");

            var random = new Random();
            var orders = new List<Order>();

            for (int i = 0; i < count; i++)
            {
                var customerId = customerIds[random.Next(customerIds.Count)];

                orders.Add(new Order
                {
                    CustomerID = customerId,
                    EmployeeID = random.Next(1, 10),
                    OrderDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    RequiredDate = DateTime.UtcNow.AddDays(random.Next(1, 30)),
                    ShippedDate = DateTime.UtcNow.AddDays(random.Next(1, 15)),
                    ShipVia = random.Next(1, 3),
                    Freight = Math.Round((decimal)(random.NextDouble() * 100), 2),
                    ShipName = $"Cliente {i + 1}",
                    ShipAddress = $"Calle {random.Next(1, 100)}",
                    ShipCity = "Bogotá",
                    ShipRegion = "Cundinamarca",
                    ShipPostalCode = $"{random.Next(10000, 99999)}",
                    ShipCountry = "Colombia"
                });
            }

            await _context.Order.AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            return Ok($"{orders.Count} órdenes insertadas correctamente.");
        }


    }
}
