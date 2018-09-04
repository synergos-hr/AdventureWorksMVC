using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Entity.Views.Users;
using AdventureWorks.Data.Helpers;
using AdventureWorks.Data.HelpersKendo;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.ViewModels;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryAspNetUsers : Repository<AspNetUser, UserViewModel>
    {
        public override AspNetUser GetById(object id)
        {
            return Context.Set<AspNetUser>().Include(x => x.UserProfile).FirstOrDefault(x => x.Id == (int)id);
        }

        public override UserViewModel GetModelById(object id)
        {
            vUser model = Context.vUsers.Find(id);

            UserViewModel viewModel = Mapper.Map<vUser, UserViewModel>(model);

            return viewModel;
        }

        public override GridResult<UserViewModel> GridList(GridRequest request, string defaultSort)
        {
            try
            {
                //if (request.ExtraFilters["userID"] == null)
                //    return new GridResult<object> { Status = "error", Message = "Greška: parameter is null!" };

                //string userID = request.ExtraFilters["userID"].ToString();

                IQueryable<vUser> query = from u in Context.vUsers select u;

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<UserViewModel>(request, "UserName");

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                string filter = sortFilter.Filter;

                if (!string.IsNullOrEmpty(filter))
                    query = query.Where(filter);

                List<vUser> page = query
                    .Skip(request.Skip)
                    .Take(request.Take)
                    .ToList();

                GridResult<UserViewModel> result = new GridResult<UserViewModel>
                {
                    Status = "success",
                    TotalCount = query.Count(),
                    Records = page.Select(Mapper.Map<vUser, UserViewModel>).ToList()
                };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<UserViewModel> {Status = "error", Message = ex.Message};
            }
        }

        public GridResult<UserRolesViewModel> GridListAllRoles(GridRequest request)
        {
            try
            {
                if (request.ExtraFilters["UserId"] == null)
                    return new GridResult<UserRolesViewModel>
                    {
                        Status = "error",
                        Message = "Greška: parameter is null!"
                    };

                int userId = Convert.ToInt32(request.ExtraFilters["UserId"]);

                IQueryable<UserRolesViewModel> query = from r in Context.Set<AspNetRole>()
                    from u in r.AspNetUsers.Where(x => x.Id == userId).DefaultIfEmpty()
                    where r.Code > 1
                    select new UserRolesViewModel
                    {
                        RoleId = r.Id,
                        RoleName = r.Name,
                        Selected = u != null
                    };

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<UserRolesViewModel>(request, "RoleName");

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                var page = query
                    .Skip(request.Skip)
                    .Take(request.Take);

                GridResult<UserRolesViewModel> result = new GridResult<UserRolesViewModel>
                {
                    Status = "success",
                    TotalCount = query.Count(),
                    Records = page.ToList()
                };

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<UserRolesViewModel> {Status = "error", Message = ex.Message};
            }
        }

        public IList<UserRolesViewModel> ListAllRoles(int? id)
        {
            try
            {
                IQueryable<UserRolesViewModel> query;

                if (id == null)
                {
                    query = from r in Context.Set<AspNetRole>()
                        where r.Code > 1
                        orderby r.Name
                        select new UserRolesViewModel
                        {
                            RoleId = r.Id,
                            RoleName = r.Name,
                            RoleNameTranslated = r.Translation,
                            Selected = false
                        };
                }
                else
                {
                    query = from r in Context.Set<AspNetRole>()
                        from u in r.AspNetUsers.Where(x => x.Id == id).DefaultIfEmpty()
                        where r.Code > 1
                        orderby r.Name
                        select new UserRolesViewModel
                        {
                            RoleId = r.Id,
                            RoleName = r.Name,
                            RoleNameTranslated = r.Translation,
                            Selected = u != null
                        };
                }

                return query.ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public IQueryable<AspNetRole> ListRoles(int id)
        {
            try
            {
                IQueryable<AspNetRole> query = from r in Context.Set<AspNetRole>()
                    from u in r.AspNetUsers.Where(x => x.Id == id)
                    where r.Code > 1
                    orderby r.Name
                    select r;

                return query;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
