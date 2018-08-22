using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdventureWorks.Web.Models.Menu
{
    public class MenuItem
    {
        #region Enums

        public enum ItemControllerType
        {
            MvcControler,
            Link
        }

        #endregion

        #region Properties

        public string Text { get; set; }
        public bool Expanded { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Parameters { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }
        public List<MenuItem> MenuItems { get; set; }
        public ItemControllerType ControllerType { get; set; }
        public string AllowedRoles { get; set; }
        public string IconClass { get; set; }
        public string Url { get; set; }

        #endregion

        public MenuItem()
        {
            Action = "Index";
            Enabled = true;
            Visible = true;
            ControllerType = ItemControllerType.MvcControler;
        }

        private bool HasMenuItems { get { return MenuItems != null && MenuItems.Count > 0; } }

        public bool ChildSelected { get { return HasMenuItems && MenuItems.Any(mi => mi.Selected); } }

        public bool Selected
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Controller))
                    return false;

                var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

                var currentAction = routeValues["action"].ToString();
                var currentController = routeValues["controller"].ToString();

                return currentAction.Equals(Action, StringComparison.InvariantCulture) && currentController.Equals(Controller, StringComparison.InvariantCulture);
            }
        }

        public string Icon
        {
            get
            {
                if (string.IsNullOrEmpty(IconClass))
                    return "";

                return $"<i class=\"fa {IconClass}\"></i> ";
            }
        }

        public string Href
        {
            get
            {
                switch (ControllerType)
                {
                    case ItemControllerType.MvcControler:
                        return new UrlHelper(HttpContext.Current.Request.RequestContext).Action(Action, Controller);
                    case ItemControllerType.Link:
                        return Action;
                    default:
                        throw new ArgumentException("Unknown ControllerType!");
                }
            }
        }
    }
}