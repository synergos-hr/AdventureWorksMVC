using System.Collections.Generic;
using System.Linq;
using AdventureWorks.Model.Kendo;

namespace AdventureWorks.Data.Contracts.Repository
{
    public interface IRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        void SetActiveUser(int userId, string userName);

        IEnumerable<TEntity> GetAll();
        IQueryable<TEntity> Query();
        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        TEntity Delete(TEntity entity);

        TModel GetModelById(object id);
        IEnumerable<TModel> ListModels(string defaultSort);

        GridResult<TModel> GridList(GridRequest request, string defaultSort);
    }
}
