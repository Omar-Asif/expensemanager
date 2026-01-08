using expensemanager.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace expensemanager.Filters
{
    // Simple session-based authorization filter with optional role check
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeUserAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? _role;
        public AuthorizeUserAttribute(string? role = null) => _role = role;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var userId = httpContext.Session.GetInt32(SessionKeys.UserId);
            if (userId is null)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (!string.IsNullOrWhiteSpace(_role))
            {
                var role = httpContext.Session.GetString(SessionKeys.UserRole);
                if (!string.Equals(role, _role, StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
