using AutoMapper;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Model.Models;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Web
{
    public class MapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.Initialize(config =>
            {
                // Human Resources
                config.CreateMap<Department, DepartmentModel>();
                // Production
                config.CreateMap<Product, ProductModel>();

                // Human Resources
                config.CreateMap<DepartmentModel, Department>();
                // Production
                config.CreateMap<ProductModel, Product>();
            });
        }
    }
}
