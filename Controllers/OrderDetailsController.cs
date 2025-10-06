using System.Linq;
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
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderDetailsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/orderdetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailDto>>> GetAll()
        {
            var details = await _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<OrderDetailDto>>(details));
        }

        // GET: api/orderdetails/{orderId}/{productId}
        [HttpGet("{orderId:int}/{productId:int}")]
        public async Task<ActionResult<OrderDetailDto>> GetById(int orderId, int productId)
        {
            var detail = await _context.OrderDetail
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId);

            if (detail == null) return NotFound();

            return Ok(_mapper.Map<OrderDetailDto>(detail));
        }

        // POST: api/orderdetails
        [HttpPost]
        public async Task<ActionResult<OrderDetailDto>> Create(OrderDetailDto dto)
        {
            var detail = _mapper.Map<OrderDetail>(dto);
            _context.OrderDetail.Add(detail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { orderId = detail.OrderID, productId = detail.ProductID }, _mapper.Map<OrderDetailDto>(detail));
        }

        // PUT: api/orderdetails/{orderId}/{productId}
        [HttpPut("{orderId:int}/{productId:int}")]
        public async Task<IActionResult> Update(int orderId, int productId, OrderDetailDto dto)
        {
            if (orderId != dto.OrderID || productId != dto.ProductID)
                return BadRequest("IDs no coinciden.");

            var detail = await _context.OrderDetail
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId);

            if (detail == null) return NotFound();

            _mapper.Map(dto, detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orderdetails/{orderId}/{productId}
        [HttpDelete("{orderId:int}/{productId:int}")]
        public async Task<IActionResult> Delete(int orderId, int productId)
        {
            var detail = await _context.OrderDetail
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId);

            if (detail == null) return NotFound();

            _context.OrderDetail.Remove(detail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/orderdetails/bulk → insertar detalles de prueba
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertDetails([FromQuery] int count = 5)
        {
            if (count <= 0 || count > 1000)
                return BadRequest("El parámetro 'count' debe estar entre 1 y 1000.");

            var orderIds = await _context.Order.Select(o => o.OrderID).ToListAsync();
            var productIds = await _context.Product.Select(p => p.ProductID).ToListAsync();

            if (!orderIds.Any() || !productIds.Any())
                return BadRequest("Se requieren órdenes y productos existentes.");

            var random = new Random();
            var details = new List<OrderDetail>();
            var usedPairs = new HashSet<(int, int)>();

            int attempts = 0;
            while (details.Count < count && attempts < count * 2)
            {
                var orderId = orderIds[random.Next(orderIds.Count)];
                var productId = productIds[random.Next(productIds.Count)];

                if (usedPairs.Contains((orderId, productId)))
                {
                    attempts++;
                    continue;
                }

                usedPairs.Add((orderId, productId));

                details.Add(new OrderDetail
                {
                    OrderID = orderId,
                    ProductID = productId,
                    UnitPrice = Math.Round((decimal)(random.NextDouble() * 100), 2),
                    Quantity = (short)random.Next(1, 10),
                    Discount = (float)Math.Round(random.NextDouble(), 2)
                });

                attempts++;
            }

            if (details.Count == 0)
                return BadRequest("No se pudieron generar combinaciones únicas de orden-producto.");

            await _context.OrderDetail.AddRangeAsync(details);
            await _context.SaveChangesAsync();

            return Ok($"{details.Count} detalles de orden insertados correctamente.");
        }

    }
}
