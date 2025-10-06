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
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _context.Employee.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<EmployeeDto>>(employees));
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return NotFound();

            return Ok(_mapper.Map<EmployeeDto>(employee));
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(EmployeeDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeID }, _mapper.Map<EmployeeDto>(employee));
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EmployeeDto dto)
        {
            if (id != dto.EmployeeID) return BadRequest();

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return NotFound();

            _mapper.Map(dto, employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return NotFound();

            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/employees/{id}/manager → obtener jefe del empleado
        [HttpGet("{id}/manager")]
        public async Task<ActionResult<EmployeeDto>> GetManager(int id)
        {
            var employee = await _context.Employee
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee?.Manager == null) return NotFound("Este empleado no tiene jefe asignado.");

            return Ok(_mapper.Map<EmployeeDto>(employee.Manager));
        }

        // POST: api/employees/bulk → insertar empleados de prueba
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkInsertEmployees([FromQuery] int count = 10)
        {
            if (count <= 0 || count > 1000)
                return BadRequest("El parámetro 'count' debe estar entre 1 y 1000.");

            var random = new Random();
            var existingPhones = await _context.Employee
                .Select(e => e.HomePhone)
                .ToHashSetAsync();

            var employees = new List<Employee>();
            int attempts = 0;

            while (employees.Count < count && attempts < count * 2)
            {
                string phone = $"300-{random.Next(1000000, 9999999)}";
                if (existingPhones.Contains(phone))
                {
                    attempts++;
                    continue;
                }

                existingPhones.Add(phone);

                employees.Add(new Employee
                {
                    LastName = $"Apellido{employees.Count + 1}",
                    FirstName = $"Nombre{employees.Count + 1}",
                    Title = "Desarrollador",
                    TitleOfCourtesy = "Sr.",
                    BirthDate = DateTime.SpecifyKind(DateTime.Today.AddYears(-random.Next(25, 50)), DateTimeKind.Utc),
                    HireDate = DateTime.SpecifyKind(DateTime.Today.AddYears(-random.Next(1, 10)), DateTimeKind.Utc),
                    Address = $"Calle {random.Next(1, 100)}",
                    City = "Bogotá",
                    Region = "Cundinamarca",
                    PostalCode = $"{random.Next(10000, 99999)}",
                    Country = "Colombia",
                    HomePhone = phone,
                    Extension = $"{random.Next(100, 999)}",
                    Notes = "Empleado de prueba"
                });

                attempts++;
            }

            if (employees.Count == 0)
                return BadRequest("No se pudieron generar empleados únicos.");

            await _context.Employee.AddRangeAsync(employees);
            await _context.SaveChangesAsync();

            return Ok($"{employees.Count} empleados insertados correctamente.");
        }



    }
}
