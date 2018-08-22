namespace AdventureWorks.Web.Helpers.Bootstrap
{
    // http://jameschambers.com/2014/06/day-14-bootstrap-alerts-and-mvc-framework-tempdata/
    public class BootstrapAlert
    {
        public static class Styles
        {
            public const string Success = "success";
            public const string Information = "info";
            public const string Warning = "warning";
            public const string Danger = "danger";
        }

        public const string TempDataKey = "TempDataAlerts";

        public string AlertStyle { get; set; }
        public string Message { get; set; }
        public bool Dismissable { get; set; }
    }
}
