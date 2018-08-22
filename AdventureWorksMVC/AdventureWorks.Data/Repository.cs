using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using AutoMapper;
using NLog;
using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Data.Exceptions;
using AdventureWorks.Data.Helpers;
using AdventureWorks.Data.HelpersKendo;
using AdventureWorks.Model.Kendo;

namespace AdventureWorks.Data
{
    public class Repository<TEntity, TModel> : IRepository<TEntity, TModel>
        where TEntity : class
        where TModel : class
    {
        protected AppDbContext Context = new AppDbContext();

        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void SetUserId(int userId)
        {
            Context.UserId = userId;
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

        public virtual TModel GetModelById(object id)
        {
            var model = Context.Set<TEntity>().Find(id);

            var viewModel = Mapper.Map<TEntity, TModel>(model);

            return viewModel;
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                Context.Set<TEntity>().Add(entity);
                Context.SaveChanges();
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

        public virtual GridResult<TModel> GridList(GridRequest request, string defaultSort)
        {
            try
            {
                IQueryable<TEntity> query = GridListQuery();

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<TModel>(request, defaultSort);

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                string filter = sortFilter.Filter;

                string customFilter = GridListCustomFilter(request);

                if (!string.IsNullOrEmpty(customFilter))
                    filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + customFilter;

                if (!string.IsNullOrEmpty(filter))
                    query = query.Where(filter);

                var page = query
                    .Skip(request.Skip)
                    .Take(request.Take)
                    .ToList();

                var pageViewModel = page.Select(x => Mapper.Map<TEntity, TModel>(x)).ToList();

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
                return new GridResult<TModel> { Status = "error", Message = "Greška: " + ex.Message };
            }
        }

        protected virtual IQueryable<TEntity> GridListQuery()
        {
            IQueryable<TEntity> query = from x in Context.Set<TEntity>()
                                        select x;

            return query;
        }

        protected virtual string GridListCustomFilter(GridRequest request)
        {
            if (request.ExtraFilters == null)
                return "";

            string filter = "";

            if (request.ExtraFilters.ContainsKey("UserId") && !string.IsNullOrEmpty(Convert.ToString(request.ExtraFilters["UserId"])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + string.Format("UserId = {0}", request.ExtraFilters["UserId"]);

            return filter;
        }

        //public static void AddCustomFilterKeyInteger(this string filter, GridRequest request, string keyName)
        //{
        //    if (request.ExtraFilters.ContainsKey("SubcategoryID") && !string.IsNullOrEmpty(Convert.ToString(request.ExtraFilters["SubcategoryID"])))
        //        filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + string.Format("SubcategoryID = {0}", request.ExtraFilters["SubcategoryID"]);
        //}
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
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + string.Format("{0} = {1}", fieldName, request.ExtraFilters[keyName]);

            return filter;
        }

        public static string AddExtraFilterKeyString(this string filter, GridRequest request, string keyName)
        {
            return AddExtraFilterKeyString(filter, request, keyName, keyName);
        }

        public static string AddExtraFilterKeyString(this string filter, GridRequest request, string keyName, string fieldName)
        {
            if (request.ExtraFilters.ContainsKey(keyName) && !string.IsNullOrEmpty(Convert.ToString(request.ExtraFilters[keyName])))
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + string.Format("{0} = '{1}'", fieldName, request.ExtraFilters[keyName]);

            return filter;
        }
    }
}
