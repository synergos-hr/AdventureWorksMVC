using AutoMapper;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Entity.Views;
using AdventureWorks.Data.Entity.Views.Users;
using AdventureWorks.Model.Models;
using AdventureWorks.Model.ViewModels;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Web
{
    public class MapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.Initialize(config =>
            {
                // AspNet

                config.CreateMap<AspNetUser, UserViewModel>();

                config.CreateMap<vUser, UserViewModel>();

                // Profile

                config.CreateMap<UserProfile, UserProfileViewModel>()
                    .ForMember(i => i.UserName, opt => opt.MapFrom(o => o.AspNetUser.UserName))
                    .ForMember(i => i.Email, opt => opt.MapFrom(o => o.AspNetUser.Email));
                config.CreateMap<UserProfileViewModel, UserProfile>();

                // Human Resources

                config.CreateMap<Department, DepartmentModel>();
                config.CreateMap<DepartmentModel, Department>();

                config.CreateMap<Employee, EmployeeModel>()
                    .ForMember(dest => dest.FirstName, y => y.MapFrom(src => src.Person.FirstName))
                    .ForMember(dest => dest.MiddleName, y => y.MapFrom(src => src.Person.MiddleName))
                    .ForMember(dest => dest.LastName, y => y.MapFrom(src => src.Person.LastName));
                config.CreateMap<vEmployeeDepartment, EmployeeDepartmentModel>();
                config.CreateMap<EmployeeModel, Employee>();

                // Production

                config.CreateMap<Product, ProductModel>();
                config.CreateMap<ProductModel, Product>();
            });
        }
    }
}
