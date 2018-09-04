using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.ViewModels;
using AdventureWorks.Web.Controllers.API.Base;
using AdventureWorks.Web.Helpers.Account;
using AdventureWorks.Web.Helpers.Singletons;

namespace AdventureWorks.Web.Controllers.API
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class RolesController : ApiBaseController
    {
        #region Properties

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                _roleManager = _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
                return _roleManager;
            }
        }

        #endregion

        [HttpPost]
        public GridResult<RoleViewModel> List(GridRequest request)
        {
            try
            {
                RepositoryAspNetRoles repo = new RepositoryAspNetRoles();

                return repo.GridList(request, "RoleName");
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
                return new GridResult<RoleViewModel> { Status = "error", Message = ex.Message };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public SaveResult Create(RoleViewModel model)
        {
            try
            {
                ApplicationRole role = new ApplicationRole(model.Name);

                var result = RoleManager.Create(role);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                RolesSingleton.ClearInstance();

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public SaveResult Update(RoleViewModel model)
        {
            try
            {
                ApplicationRole role = RoleManager.FindById(model.Id);

                role.Name = model.Name;

                var result = RoleManager.Update(role);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                RolesSingleton.ClearInstance();

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public SaveResult Delete(RoleViewModel model)
        {
            try
            {
                if (model.Id == 0)
                    return new SaveResult { Status = "error", Message = "No records for deletion!" };

                ApplicationRole role = RoleManager.FindById(model.Id);

                if (role == null)
                    return new SaveResult { Status = "error", Message = "Can't find role by id!" };

                IdentityResult result = RoleManager.Delete(role);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                RolesSingleton.ClearInstance();

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        //public GridResult<UserViewModel> ListRoleUsers([ModelBinder(typeof(ExtraFiltersBinder))] GridRequest request)
        public GridResult<UserViewModel> ListRoleUsers(GridRequest request)
        {
            try
            {
                if (request.ExtraFilters["RoleId"] == null)
                    return new GridResult<UserViewModel> { Status = "error", Message = "Greška: parameter is null!" };

                int roleId = Convert.ToInt32(request.ExtraFilters["RoleId"]);

                RepositoryAspNetRoles repo = new RepositoryAspNetRoles();

                GridResult<UserViewModel> result = repo.GridQueryRoleUsers(roleId, request);

                return result;
            }
            catch (Exception ex)
            {
                return new GridResult<UserViewModel> { Status = "error", Message = ex.Message };
            }
        }
    }
}