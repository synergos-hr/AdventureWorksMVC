using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NLog;
using AdventureWorks.Web.Helpers.Bootstrap;

namespace AdventureWorks.Web.Controllers.Mvc.Base
{
    public class BaseController : Controller
    {
        protected readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected void AlertSuccess(string message, bool dismissable = false)
        {
            AddAlert(BootstrapAlert.Styles.Success, message, dismissable);
        }

        protected void AlertInformation(string message, bool dismissable = false)
        {
            AddAlert(BootstrapAlert.Styles.Information, message, dismissable);
        }

        protected void AlertWarning(string message, bool dismissable = false)
        {
            AddAlert(BootstrapAlert.Styles.Warning, message, dismissable);
        }

        protected void AlertDanger(string message, bool dismissable = false)
        {
            AddAlert(BootstrapAlert.Styles.Danger, message, dismissable);
        }

        protected void AlertException(Exception ex, bool dismissable = false)
        {
            string message = ex.Message;

#if DEBUG
            Exception exLoop = ex;
            while (exLoop.InnerException != null)
            {
                exLoop = exLoop.InnerException;
                message += "\r\n" + exLoop.Message;
            }
#endif

            AddAlert(BootstrapAlert.Styles.Danger, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(BootstrapAlert.TempDataKey)
                ? (List<BootstrapAlert>)TempData[BootstrapAlert.TempDataKey]
                : new List<BootstrapAlert>();

            alerts.Add(new BootstrapAlert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[BootstrapAlert.TempDataKey] = alerts;
        }
    }
}