using System;
using System.Threading.Tasks;
using System.Web.Http;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.CustomFilters;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Web.Controllers.API.Base;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Web.Controllers.API
{
    public class ProductsController : ApiBaseModelController<Product, ProductModel, RepositoryProducts>
    {
        public ProductsController()
            : base("ProductID", "Name")
        {
        }

        [HttpPost]
        public virtual async Task<ListResult<ProductModel>> ListFiltered(RequestProducts request)
        {
            try
            {
                return await Task.Run(() => _repository.ListFiltered(request));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ListResult<ProductModel> { Status = "error", Message = ex.Message };
            }
        }
    }
}
