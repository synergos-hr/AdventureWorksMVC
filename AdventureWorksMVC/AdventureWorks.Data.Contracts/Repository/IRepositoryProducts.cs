using AdventureWorks.Data.Entity.Tables;

namespace AdventureWorks.Data.Contracts.Repository
{
    public interface IRepositoryProducts : IRepository<Product, Model.Models.ProductModel>
    {
        ProductPhoto GetPhoto(int id);
    }
}
