using System.Web.Mvc;

namespace AdventureWorks.Web.Helpers.Binders.MVC
{
    public class EmptyStringBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new EmptyStringModelBinder();
        }
    }
}
