using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace AdventureWorks.Web.Helpers.Binders.MVC
{
    public sealed class EmptyStringModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            object model = base.BindModel(controllerContext, bindingContext);

            PropertyInfo[] properties = (from p in bindingContext.ModelType.GetProperties()
                                            where p.PropertyType == typeof(string) &&
                                                p.CanRead &&
                                                p.CanWrite &&
                                                p.GetValue(model, null) == null
                                            select p).ToArray();

            foreach (PropertyInfo item in properties)
                item.SetValue(model, string.Empty, null);

            return model;
        }
    }
}
