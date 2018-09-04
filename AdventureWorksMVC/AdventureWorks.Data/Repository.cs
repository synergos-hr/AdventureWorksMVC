using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Data.Exceptions;
using AdventureWorks.Data.Helpers;
using AdventureWorks.Data.HelpersKendo;
using AdventureWorks.Data.Log;
using AdventureWorks.Model.Kendo;

namespace AdventureWorks.Data
{
    public class Repository<TEntity, TModel> : RepositoryBase, IRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        public void SetActiveUser(int userId, string userName)
        {
            Context.UserId = userId;
            Context.UserName = userName;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public virtual IQueryable<TEntity> Query()
        {
            return Context.Set<TEntity>();
        }

        public virtual TEntity GetById(object id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                Context.Database.Log = DatabaseLog.Write;

                Context.Set<TEntity>().Add(entity);

                int count = Context.SaveChanges();

                Log.Trace($"Inserted [{entity.GetType()}]: {count}");
            }
            catch (DbEntityValidationException ex)
            {
                Log.Error(ex);
                throw new EntityValidationException(ex);
            }
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                Context.Entry(entity).State = EntityState.Modified;
                Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Error(ex);
                throw new EntityValidationException(ex);
            }
        }

        public virtual TEntity Delete(TEntity entity)
        {
            if (entity != null)
                Context.Entry(entity).State = EntityState.Deleted;

            Context.SaveChanges();

            return entity;
        }

        public virtual TModel GetModelById(object id)
        {
            TEntity model = Context.Set<TEntity>().Find(id);

            TModel viewModel = Mapper.Map<TEntity, TModel>(model);

            return viewModel;
        }

        public virtual IEnumerable<TModel> ListModels(string defaultSort)
        {
            try
            {
                IQueryable<TEntity> query = GetQuery();

                if (!string.IsNullOrEmpty(defaultSort))
                    query = query.OrderBy(defaultSort);

                return GetListModel(new ListParams(defaultSort), ref query);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public virtual GridResult<TModel> GridList(GridRequest request, string defaultSort)
        {
            try
            {
                IQueryable<TEntity> query = GetQuery();

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<TModel>(request, defaultSort);

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                string filter = sortFilter.Filter;

                List<TModel> pageViewModel = GetListModel(new ListParams(request, defaultSort, filter), ref query);

                GridResult<TModel> result = new GridResult<TModel>
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
                return new GridResult<TModel> { Status = "error", Message = ex.Message };
            }
        }

        protected List<TModel> GetListModel(ListParams listParams)
        {
            IQueryable<TEntity> query = GetQuery();

            query = query.OrderBy(listParams.DefaultSort);

            return GetListModel(listParams, ref query);
        }

        protected List<TModel> GetListModel(ListParams listParams, ref IQueryable<TEntity> query)
        {
            List<TEntity> page = GetList(listParams, ref query);

            return page.Select(Mapper.Map<TEntity, TModel>).ToList();
        }

        protected List<TEntity> GetList(ListParams listParams, ref IQueryable<TEntity> query)
        {
            string customFilter = ParseCustomFilters(listParams.CustomFilters);

            if (!string.IsNullOrEmpty(customFilter))
                listParams.Filter += (string.IsNullOrEmpty(listParams.Filter) ? "" : " AND ") + customFilter;

            if (!string.IsNullOrEmpty(listParams.Filter))
                query = query.Where(listParams.Filter);

            if (listParams.Skip != 0 || listParams.Take != 0)
            {
                return query
                    .Skip(listParams.Skip)
                    .Take(listParams.Take)
                    .ToList();
            }

            return query.ToList();
        }

        protected virtual IQueryable<TEntity> GetQuery()
        {
            IQueryable<TEntity> query = from x in Context.Set<TEntity>()
                                        select x;

            query = GetQueryInclude(query);

            return query;
        }

        protected virtual IQueryable<TEntity> GetQueryInclude(IQueryable<TEntity> query)
        {
            return query;
        }

        protected virtual string ParseCustomFilters(IDictionary<string, string> customFilters)
        {
            if (customFilters == null)
                return "";

            string filter = "";

            filter = filter.AddCustomFilterKeyInteger(customFilters, "UserId");

            filter = filter.AddCustomFilterKeyInteger(customFilters, "AspNetUserId");

            return filter;
        }
    }

    public static class RepositoryExtensions
    {
        public static string AddExtraFilterKeyInteger(this string filter, GridRequest request, string keyName)
        {
            return AddExtraFilterKeyInteger(filter, request, keyName, keyName);
        }

        public static string AddExtraFilterKeyInteger(this string filter, GridRequest request, string keyName, string fieldName)
        {
            if (request.ExtraFilters.ContainsKey(keyName) && !string.IsNullOrEmpty(Convert.ToString(request.ExtraFilters[keyName])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + $"{fieldName} = {request.ExtraFilters[keyName]}";

            return filter;
        }

        public static string AddCustomFilterKeyInteger(this string filter, IDictionary<string, string> customFilters, string keyName)
        {
            return AddCustomFilterKeyInteger(filter, customFilters, keyName, keyName);
        }

        public static string AddCustomFilterKeyInteger(this string filter, IDictionary<string, string> customFilters, string keyName, string fieldName)
        {
            if (customFilters.ContainsKey(keyName) && !string.IsNullOrEmpty(Convert.ToString(customFilters[keyName])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + $"{fieldName} = {customFilters[keyName]}";

            return filter;
        }

        public static string AddExtraFilterKeyString(this string filter, GridRequest request, string keyName)
        {
            return AddExtraFilterKeyString(filter, request, keyName, keyName);
        }

        public static string AddExtraFilterKeyString(this string filter, GridRequest request, string keyName, string fieldName)
        {
            if (request.ExtraFilters.ContainsKey(keyName) && !string.IsNullOrEmpty(Convert.ToString(request.ExtraFilters[keyName])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + $"{fieldName} = '{request.ExtraFilters[keyName]}'";

            return filter;
        }

        public static string AddCustomFilterKeyString(this string filter, IDictionary<string, string> customFilters, string keyName)
        {
            return AddCustomFilterKeyString(filter, customFilters, keyName, keyName);
        }

        public static string AddCustomFilterKeyString(this string filter, IDictionary<string, string> customFilters, string keyName, string fieldName)
        {
            if (customFilters.ContainsKey(keyName) && !string.IsNullOrEmpty(Convert.ToString(customFilters[keyName])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + $"{fieldName} = '{customFilters[keyName]}'";

            return filter;
        }
    }
}
