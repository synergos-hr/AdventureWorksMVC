using System.Collections.Generic;
using System.Linq;
using AdventureWorks.Model.Kendo;

namespace AdventureWorks.Data.Contracts.Repository
{
    public interface IRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        void SetUserId(int userId);
        IEnumerable<TEntity> GetAll();
        IQueryable<TEntity> Query();
        TEntity GetById(object id);
        TModel GetModelById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        TEntity Delete(TEntity entity);
        GridResult<TModel> GridList(GridRequest request, string defaultSort);
    }
}
