using System;
using System.Linq;
using AutoMapper;
using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Helpers;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.CustomFilters;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryProducts : Repository<Product, ProductModel>, IRepositoryProducts
    {
        protected override string GridListCustomFilter(GridRequest request)
        {
            string filter = base.GridListCustomFilter(request);

            if (request.ExtraFilters == null)
                return filter;

            filter = filter.AddExtraFilterKeyInteger(request, "SubcategoryID", "ProductSubcategoryID");

            return filter;
        }

        public ProductPhoto GetPhoto(int id)
        {
            var primaryPhoto = Context.Set<ProductProductPhoto>().SingleOrDefault(x => x.ProductID == id && x.Primary == true);

            if (primaryPhoto == null)
                return null;

            return primaryPhoto.ProductPhoto;
        }

        public virtual ListResult<ProductModel> ListFiltered(RequestProducts request)
        {
            try
            {
                IQueryable<Product> query = from p in Context.Set<Product>()
                                            select p;

                query = query.OrderBy(string.IsNullOrEmpty(request.Sort) ? "Name" : request.Sort);

                if (request.SubcategoryID != null)
                    query = query.Where(x => x.ProductSubcategoryID == request.SubcategoryID);

                var page = query
                    .Skip(request.Skip ?? 0)
                    .Take(request.Take ?? 24)
                    .ToList();

                var pageViewModel = page.Select(x => Mapper.Map<Product, ProductModel>(x)).ToList();

                ListResult<ProductModel> result = new ListResult<ProductModel>
                {
                    Status = "success",
                    TotalCount = query.Count(),
                    Records = pageViewModel
                };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ListResult<ProductModel> { Status = "error", Message = "Greška: " + ex.Message };
            }
        }
    }
}
