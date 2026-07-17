using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace FBSC.Common.Web.Utility.Annotations;

/// <summary>
/// Enables or disables an action based on whether the form field with the specified name is present.
/// </summary>
/// <remarks>
/// Initializes an instance of <see cref="FormValueRequiredAttribute"/> with the name of the form field to check.
/// </remarks>
/// <param name="name"></param>
public sealed class FormValueRequiredAttribute(string name) : ActionMethodSelectorAttribute
{

    /// <summary>
    /// Determines whether the action selection is valid for the specified route context.
    /// </summary>
    /// <param name="routeContext"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
    {
        if (string.Equals(routeContext.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(routeContext.HttpContext.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.IsNullOrEmpty(routeContext.HttpContext.Request.ContentType))
        {
            return false;
        }

        if (!routeContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !string.IsNullOrEmpty(routeContext.HttpContext.Request.Form[name]);
    }
}