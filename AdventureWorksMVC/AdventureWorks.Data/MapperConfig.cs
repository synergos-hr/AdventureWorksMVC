using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Model.Models;
using AutoMapper;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Data
{
    public static class MapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.Initialize(config =>
            {
                // Human Resources
                config.CreateMap<Department, DepartmentModel>();
                config.CreateMap<Employee, EmployeeModel>()
                    .ForMember(dest => dest.FirstName, y => y.MapFrom(src => src.Person.FirstName))
                    .ForMember(dest => dest.MiddleName, y => y.MapFrom(src => src.Person.MiddleName))
                    .ForMember(dest => dest.LastName, y => y.MapFrom(src => src.Person.LastName));
                config.CreateMap<vEmployeeDepartment, EmployeeDepartmentModel>();
                // Production
                config.CreateMap<Product, ProductModel>();

                // Human Resources
                config.CreateMap<DepartmentModel, Department>();
                config.CreateMap<EmployeeModel, Employee>();
                // Production
                config.CreateMap<ProductModel, Product>();
            });
        }
    }
}
