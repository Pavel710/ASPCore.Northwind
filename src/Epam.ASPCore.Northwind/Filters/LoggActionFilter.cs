using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Filters
{
    public class LoggActionFilter : IActionFilter
    {
        private readonly bool _loggingParameters;

        public LoggActionFilter(bool loggingParameters = false)
        {
            _loggingParameters = loggingParameters;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Log.Information("Start action " + context.ActionDescriptor.DisplayName);
            if (_loggingParameters)
            {
                var parameters = string.Join(", ", context.ActionDescriptor.Parameters.Select(x => x.Name).ToList());
                Log.Information("Action Parameters: " + parameters);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Log.Information("End action " + context.ActionDescriptor.DisplayName);
        }
    }
}
