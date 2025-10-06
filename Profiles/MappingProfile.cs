using ProductsAPI.Dtos;
using ProductsAPI.Models;
using AutoMapper;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductsAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product
            CreateMap<Product, ProductDto>().ReverseMap();

            // Category
            CreateMap<Category, CategoryDto>().ReverseMap();

            // Customer
            CreateMap<Customer, CustomerDto>().ReverseMap();

            // Employee
            CreateMap<Employee, EmployeeDto>().ReverseMap();

            // Order
            CreateMap<Order, OrderDto>().ReverseMap();

            // OrderDetail
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();

            // Shipper
            CreateMap<Shipper, ShipperDto>().ReverseMap();

            // Supplier
            CreateMap<Supplier, SupplierDto>().ReverseMap();
        }
    }
}
