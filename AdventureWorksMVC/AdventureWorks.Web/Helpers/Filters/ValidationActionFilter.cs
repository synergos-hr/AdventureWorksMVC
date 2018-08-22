using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AdventureWorks.Helpers.Filters
{
    public class ValidationActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid && !modelState.Keys.Any(s => s == "records[0]"))
            {
                //var errors = new JObject();
                string errors = "";

                foreach (string key in modelState.Keys)
                {
                    var state = modelState[key];

                    if (state.Errors.Any())
                    {
                        //errors[key] = state.Errors.First().ErrorMessage;
                        errors += key + ": " + (string.IsNullOrEmpty(state.Errors.First().ErrorMessage) ? state.Errors.First().Exception.Message : state.Errors.First().ErrorMessage) + "<br/>";
                    }
                }

                //context.Response = context.Request.CreateResponse<JObject>(HttpStatusCode.BadRequest, errors);

                string message = string.Format("Greške u validaciji podataka:<br/>{0}", errors.Replace("'", "\""));

                context.Response = context.Request.CreateResponse(JObject.Parse("{ status: 'error', message: '" + message + "' }"));
            }
        }
    }
}
