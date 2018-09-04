using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.Account.Authorization;
using AdventureWorks.Model.Account.Manage;
using AdventureWorks.Model.CustomRequests;
using AdventureWorks.Model.Kendo;
using AdventureWorks.Model.ViewModels;
using AdventureWorks.Web.Controllers.API.Base;
using AdventureWorks.Web.Helpers.Account;
using AdventureWorks.Web.Helpers.Email;
using AdventureWorks.Web.Helpers.Settings;
using AdventureWorks.Web.Helpers.Singletons;
using AdventureWorks.Web.Helpers.User;

namespace AdventureWorks.Web.Controllers.API
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class UsersController : ApiBaseController
    {
        #region Properties

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                _userManager = _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
        }

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
        public GridResult<UserViewModel> List(GridRequest request)
        {
            try
            {
                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                return repo.GridList(request, "UserName");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<UserViewModel> { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public GridResult<UserRolesViewModel> ListAllRoles(GridRequest request)
        {
            try
            {
                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                return repo.GridListAllRoles(request);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<UserRolesViewModel> { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult Create(RegisterViewModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new SaveResult { Status = "error", Message = "Model not valid!" };

                var user = new ApplicationUser { UserName = request.Email, Email = request.Email, EmailConfirmed = true, LockoutEnabled = false };  // TODO: Register field UserName

                var result = UserManager.Create(user, request.Password);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public virtual SaveResult SetPassword(SetPasswordViewModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new SaveResult { Status = "error", Message = "Model not valid!" };

                string code = UserManager.GeneratePasswordResetToken(request.UserId);

                IdentityResult result = UserManager.ResetPassword(request.UserId, code, request.NewPassword);

                if (!result.Succeeded)
                {
                    // log
                    Debug.Assert(false);

                    return new SaveResult { Status = "error", Message = "Password change failed." };
                }

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public virtual SaveResult SetEmail(SetEmailViewModel request)
        {
            try
            {
                if (request.UserId == 0)
                    return new SaveResult { Status = "error", Message = "Id is not valid!" };

                if (UserManager.IsInRole(request.UserId, "Admin"))
                    return new SaveResult { Status = "error", Message = "User is admin!" };

                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                AspNetUser user = repo.GetById(request.UserId);

                user.Email = request.NewEmail;

                repo.Update(user);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult Lock(UserIdRequest request)
        {
            try
            {
                // TODO: js error display

                if (request.Id == 0)
                    return new SaveResult { Status = "error", Message = "Id is not valid!" };

                if (UserManager.IsInRole(request.Id, "Admin"))
                    return new SaveResult { Status = "error", Message = "User is admin!" };

                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                AspNetUser user = repo.GetById(request.Id);

                user.LockoutEnabled = true;

                user.LockoutEndDateUtc = new DateTime(2100, 1, 1);

                repo.Update(user);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult Unlock(UserIdRequest request)
        {
            try
            {
                if (request.Id == 0)
                    return new SaveResult { Status = "error", Message = "Id is not valid!" };

                if (UserManager.IsInRole(request.Id, "Admin"))
                    return new SaveResult { Status = "error", Message = "User is admin!" };

                RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

                AspNetUser user = repo.GetById(request.Id);

                user.LockoutEnabled = false;

                user.LockoutEndDateUtc = null;

                repo.Update(user);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult SetRole(SetUserRoleRequest request)
        {
            try
            {
                ApplicationRole role = RoleManager.FindById(request.RoleId);

                if (!UserManager.IsInRole(HttpContext.Current.User.Identity.GetUserId<int>(), "Admin"))
                {
                    //if (UserManager.IsInRole(request.UserId, "Admin"))
                    //    return new SaveResult { Status = "error", Message = "User is admin!" };

                    if (role.Name == "Admin")
                        return new SaveResult { Status = "error", Message = "Admin role can be set by admin only!" };
                }

                var result = UserManager.AddToRoles(request.UserId, role.Name);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                UserRolesSingleton.RemoveById(request.UserId);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult UnsetRole(SetUserRoleRequest request)
        {
            try
            {
                ApplicationRole role = RoleManager.FindById(request.RoleId);

                if (!UserManager.IsInRole(HttpContext.Current.User.Identity.GetUserId<int>(), "Admin"))
                {
                    //if (UserManager.IsInRole(request.UserId, "Admin"))
                    //    return new SaveResult { Status = "error", Message = "User is admin!" };

                    if (role.Name == "Admin")
                        return new SaveResult { Status = "error", Message = "Admin role can be set by admin only!" };
                }

                var result = UserManager.RemoveFromRoles(request.UserId, role.Name);

                if (!result.Succeeded)
                    return new SaveResult { Status = "error", Message = result.Errors.First() };

                UserRolesSingleton.RemoveById(request.UserId);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public SaveResult SendTestEmail(UserIdRequest request)
        {
            try
            {
                string link = "http://adventureworksmvc.synergos.hr/";

                string message = "Hello,<br/><br/>\n" +
                                 $"This is test email sent from application {AppSettings.AppName} ({link}).<br/><br/>\n" +
                                 "Best regards<br/>";

                UserViewModel user = UserProfileHelper.GetUser(request.Id);

                if (string.IsNullOrEmpty(user.Email))
                    return new SaveResult { Status = "error", Message = "User doesn't have email address." };

                EmailHelper.SendMail(AppSettings.AppName, user.Email, string.IsNullOrEmpty(user.FullName) ? user.Email : user.FullName, message);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = ex.Message };
            }
        }
    }
}
