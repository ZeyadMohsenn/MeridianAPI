using Microsoft.AspNetCore.Mvc.Filters;

namespace StoreManagement.Bases;

public class CustomFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var lang = context.HttpContext.Request.Headers["Accept-Language"];
        if (!string.IsNullOrWhiteSpace(lang) && lang.ToString().Length > 4 && lang.ToString().Substring(2, 1) == "-")
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang.ToString().Substring(0, 5));
    }
}

