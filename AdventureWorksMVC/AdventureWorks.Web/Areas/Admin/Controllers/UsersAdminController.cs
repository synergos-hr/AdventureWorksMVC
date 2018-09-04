using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using AdventureWorks.Data.Entity.Tables.AspNet;
using AdventureWorks.Data.Repository;
using AdventureWorks.Model.ViewModels;
using AdventureWorks.Resources;
using AdventureWorks.Web.Controllers.Mvc.Base;
using AdventureWorks.Web.Helpers.Account;
using AdventureWorks.Web.Helpers.Attributes;
using AdventureWorks.Web.Helpers.Binders.MVC;
using AdventureWorks.Web.Helpers.Email;
using AdventureWorks.Web.Helpers.Settings;
using AdventureWorks.Web.Helpers.Singletons;
using AdventureWorks.Web.Helpers.User;
using AdventureWorks.Web.Models.ViewModels;

namespace AdventureWorks.Web.Areas.Admin.Controllers
{
    [CustomMvcAuthorize(Roles = "SuperAdmin,Admin")]
    public class UsersAdminController : BaseController
    {
        #region Propreties

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                _userManager = _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
            private set => _userManager = value;
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                _roleManager = _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                return _roleManager;
            }
            private set => _roleManager = value;
        }

        #endregion

        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        //
        // GET: Admin/UsersAdmin/
        public ActionResult Index()
        {
            return View();
        }
/*
        //
        // GET: Admin/UsersAdmin/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }
*/
        //
        // GET: Admin/UsersAdmin/Create
        public ActionResult Create()
        {
            //Get the list of Roles
            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            CreateUserViewModel model = new CreateUserViewModel();

            RepositoryAspNetUsers repo = new RepositoryAspNetUsers();

            model.RolesList = repo.ListAllRoles(null);

            return View(model);
        }

        //
        // POST: Admin/UsersAdmin/Create
        [HttpPost]
        public async Task<ActionResult> Create([EmptyStringBinder] CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.RolesList.Any(x => x.Selected))
            {
                ModelState.AddModelError("", ErrorMessages.UserCreate_NoSelectedRole);
                return View(model);
            }

            string email = !string.IsNullOrEmpty(model.Email) && EmailHelper.IsValidEmail(model.Email)
                ? model.Email
                : null;

            // Add user
            ApplicationUser user = new ApplicationUser { UserName = model.UserName, Email = email, EmailConfirmed = true, LockoutEnabled = false };

            IdentityResult resultUser = await UserManager.CreateAsync(user, model.Password);

            if (!resultUser.Succeeded)
            {
                Log.Error(resultUser.Errors.First());
                ModelState.AddModelError("", resultUser.Errors.First());
                return View(model);
            }

            // Add profile
            try
            {
                RepositoryUserProfiles repo = new RepositoryUserProfiles();

                repo.SetActiveUser(User.Identity.GetUserId<int>(), User.Identity.Name);

                UserProfile userProfile = new UserProfile();

                Mapper.Map(model, userProfile);

                userProfile.UserId = user.Id;

                repo.Insert(userProfile);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                AlertException(ex);
            }

            // Send email to user
            try
            {
                if (model.SendEmailToUser && !string.IsNullOrEmpty(email))
                {
                    string link = "http://adventureworksmvc.synergos.hr/";

                    string applicationName = AppSettings.AppName;

                    string message = "Hello,<br/><br/>\n" +
                                     $"You are registered as a user of {applicationName}:<br/>\n" +
                                     $"<a href='{link}'>{applicationName}</a><br/><br/>\n" +
                                     $"Initial password for entering is: {model.Password}.<br/>It is recommended to change it in application.<br/>" +
                                     "Best regards<br/>";

                    EmailHelper.SendMail(AppSettings.AppName, email, string.IsNullOrEmpty(model.FullName) ? email : model.FullName, message);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                AlertException(ex);
            }

            // Add user to the selected roles
            try
            {
                List<string> rolesAdd = new List<string>();

                foreach (UserRolesViewModel role in model.RolesList)
                {
                    if (role.Selected)
                        rolesAdd.Add(role.RoleName);
                }

                var resultRoles = await UserManager.AddToRolesAsync(user.Id, rolesAdd.ToArray());

                if (!resultRoles.Succeeded)
                {
                    Log.Error(resultRoles.Errors.First());
                    AlertDanger(resultRoles.Errors.First());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                AlertException(ex);
            }

            AlertSuccess("User is successfully added.", true);

            return RedirectToAction("Index");
        }

        //
        // GET: Admin/UsersAdmin/Edit
        public async Task<ActionResult> Edit(int id)
        {
            //ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            UserProfileViewModel profile = await Task.Run(() => UserProfileHelper.GetProfile(id));

            EditUserViewModel model;

            if (profile == null)
            {
                UserViewModel user = UserProfileHelper.GetUser(id);

                model = new EditUserViewModel
                {
                    UserId = id,
                    UserName = user.UserName,
                    Email = user.Email
                };
            }
            else
            {
                model = Mapper.Map<UserProfileViewModel, EditUserViewModel>(profile);
            }

            return View(model);
        }

        //
        // POST: Admin/UsersAdmin/Edit
        [HttpPost]
        public ActionResult Edit([EmptyStringBinder] EditUserViewModel viewModel)
        {
            try
            {
                RepositoryUserProfiles repoProfiles = new RepositoryUserProfiles();

                repoProfiles.SetActiveUser(User.Identity.GetUserId<int>(), User.Identity.Name);

                UserProfile userProfile = repoProfiles.GetById(viewModel.UserId);

                if (userProfile == null)
                {
                    userProfile = new UserProfile();

                    Mapper.Map(viewModel, userProfile);

                    repoProfiles.Insert(userProfile);
                }
                else
                {
                    Mapper.Map(viewModel, userProfile);

                    repoProfiles.Update(userProfile);

                    UserProfileSingleton.RemoveProfileById(userProfile.UserId);
                }

                RepositoryAspNetUsers repoUsers = new RepositoryAspNetUsers();

                AspNetUser user = repoUsers.GetById(viewModel.UserId);

                if (user.Email != viewModel.Email)
                {
                    user.Email = viewModel.Email;

                    repoUsers.Update(user);
                }

                AlertSuccess("Changes on profile are saved.", true);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                AlertException(ex);
                return RedirectToAction("Edit");
            }
        }

/*
        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
*/
    }
}