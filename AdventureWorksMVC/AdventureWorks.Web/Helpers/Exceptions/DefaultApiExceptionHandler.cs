using NLog;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace AdventureWorks.Web.Helpers.Exceptions
{
    // http://stackoverflow.com/questions/13831933/mvc-4-global-exception-filter-how-to-implement
    public class DefaultApiExceptionHandler : IExceptionFilter
    {
        private readonly Logger _logger;

        public DefaultApiExceptionHandler()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public bool AllowMultiple
        {
            get { return true; }
        }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                _logger.Error(actionExecutedContext.Exception, "web service error");

                // TODO: return json ErrorResult

                //var response = actionExecutedContext.Response;
                //response.Write(new ErrorResult { Status = "error", Message = filterContext.Exception.Message });

                //actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new HttpMessageContent() {} };
                //Task<string> requestContent = actionExecutedContext.Request.Content.ReadAsStringAsync().ContinueWith(requestResult => string.IsNullOrEmpty(requestResult.Result) ? "N/A" : requestResult.Result);
                //return requestContent;
            }, cancellationToken);
        }
    }
}
