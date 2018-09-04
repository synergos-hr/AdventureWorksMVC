using System;
using System.Linq;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Helpers;
using AdventureWorks.Data.HelpersKendo;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.ViewModels;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryAspNetRoles : Repository<AspNetRole, RoleViewModel>
    {
        public override GridResult<RoleViewModel> GridList(GridRequest request, string defaultSort)
        {
            try
            {
                //if (request.ExtraFilters["userID"] == null)
                //    return new GridResult<object> { Status = "error", Message = "Greška: parameter is null!" };

                //string userID = request.ExtraFilters["userID"].ToString();

                IQueryable<RoleViewModel> query = from r in Context.AspNetRoles
                                                  where r.Code > 1
                                                  select new RoleViewModel
                                                  {
                                                      Id = r.Id,
                                                      Name = r.Name,
                                                      Translation = r.Translation
                                                  };

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<RoleViewModel>(request, "Name");

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                string filter = sortFilter.Filter;

                // custom filters

                if (!string.IsNullOrEmpty(filter))
                    query = query.Where(filter);

                var page = query
                    .Skip(request.Skip)
                    .Take(request.Take);

                GridResult<RoleViewModel> result = new GridResult<RoleViewModel>
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
                return new GridResult<RoleViewModel> { Status = "error", Message = ex.Message };
            }
        }

        public GridResult<UserViewModel> GridQueryRoleUsers(int roleId, GridRequest request)
        {
            try
            {
                DateTime minLockoutDate = DateTime.Parse("2100-01-01");

                IQueryable<UserViewModel> query = from u in Context.Set<AspNetUser>()
                                                  from rl in u.AspNetRoles.Where(x => x.Id == roleId && x.Code > 1)
                                                  join p in Context.Set<UserProfile>() on u.Id equals p.UserId into joinedP
                                                  from p in joinedP.DefaultIfEmpty()
                                                  select new UserViewModel
                                                  {
                                                      UserId = u.Id,
                                                      UserName = u.UserName,
                                                      Email = u.Email,
                                                      FirstName = p.FirstName,
                                                      LastName = p.LastName,
                                                      Locked = ((DateTime?)u.LockoutEndDateUtc ?? DateTime.Now) >= minLockoutDate.Date
                                                  };

                GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<UserViewModel>(request, "UserName");

                if (!string.IsNullOrEmpty(sortFilter.Sort))
                    query = query.OrderBy(sortFilter.Sort);

                string filter = sortFilter.Filter;

                // custom filters

                if (!string.IsNullOrEmpty(filter))
                    query = query.Where(filter);

                var page = query
                    .Skip(request.Skip)
                    .Take(request.Take);

                GridResult<UserViewModel> result = new GridResult<UserViewModel>
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
                return new GridResult<UserViewModel> { Status = "error", Message = ex.Message };
            }
        }
    }
}
