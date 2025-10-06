📦 ProductsAPI

ProductsAPI es una API RESTful desarrollada en ASP.NET Core para gestionar productos, órdenes, empleados, clientes, proveedores y shippers. Utiliza PostgreSQL como base de datos, AutoMapper para mapeo entre entidades y DTOs, y sigue principios de arquitectura limpia y modular.

🚀 Características

- CRUD completo para:
- Productos
- Órdenes
- Detalles de orden
- Empleados
- Clientes
- Proveedores
- Shippers
- Inserción masiva de datos de prueba (bulk)
- Relaciones entre entidades (Customer, Employee, Shipper, Product)
- Validación de datos y manejo de errores
- Mapeo limpio con AutoMapper
- Organización modular por dominio
- Preparado para integración con Swagger

🧱 Tecnologías utilizadas

- ASP.NET Core 7+
- Entity Framework Core
- PostgreSQL
- AutoMapper
- C#
- Swagger (opcional para documentación interactiva)

📁 Estructura del proyecto

ProductsAPI/
├── Controllers/
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── OrderDetailsController.cs
│   ├── EmployeesController.cs
│   └── CustomersController.cs
├── Models/
│   ├── Product.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Employee.cs
│   ├── Customer.cs
│   └── Shipper.cs
├── Dtos/
│   ├── ProductDto.cs
│   ├── OrderDto.cs
│   ├── OrderDetailDto.cs
│   ├── EmployeeDto.cs
│   ├── CustomerDto.cs
│   └── ShipperDto.cs
├── Data/
│   └── AppDbContext.cs
├── Profiles/
│   └── MappingProfile.cs
└── Program.cs



⚙️ Instalación y configuración
- Clona el repositorio
git clone https://github.com/CEQuinterogarcia/ProductsAPI.git
cd ProductsAPI
- Configura la cadena de conexión en appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ProductsDb;Username=postgres;Password=tu_clave"
}
- Ejecuta las migraciones
dotnet ef database update
- Inicia el proyecto
dotnet run



📌 Endpoints destacados
|  |  |  | 
|  | /api/products |  | 
|  | /api/orders/bulk |  | 
|  | /api/orderdetails/bulk |  | 
|  | /api/employees/{id}/manager |  | 
|  | /api/employees/bulk |  | 



🧪 Datos de prueba
Puedes usar los endpoints bulk para poblar la base de datos con datos aleatorios y únicos. Se validan claves foráneas (CustomerID, ProductID, ShipVia, EmployeeID) para evitar errores de integridad referencial.

🧠 Autor
Camilo Quintero
Full Stack Developer & Systems Engineer
Apasionado por crear aplicaciones web funcionales, escalables y bien estructuradas.
🔗 GitHub
📍 Bogotá, Colombia


