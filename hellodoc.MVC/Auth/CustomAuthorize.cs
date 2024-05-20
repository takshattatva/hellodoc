using hellodoc.BAL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace hellodoc.MVC.Auth
{
    public class CustomAuthorize : Attribute,IAuthorizationFilter
    {
        private readonly string[] _role;

        public CustomAuthorize(params string[] role)
        {
            _role = role;
        }

        #region OnAuthorize

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtservice = context.HttpContext.RequestServices.GetService<IJwtServiceRepo>();

            if (jwtservice == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Patient", Action = "Index" }));
                return;
            }

            var session=context.HttpContext.Session;
            string token = session.GetString("token");

            if(token == null || !jwtservice.ValidateToken(token,out JwtSecurityToken jwtSecurityToken)) 
            {
                if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Patient", Action = "AjaxLogout" }));
                    return;
                }
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Patient", Action = "Index" }));
                return;
            }

            var roleclaims = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (roleclaims == null) 
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Patient", Action = "Index" }));
                return;
            }

            if (string.IsNullOrWhiteSpace(roleclaims.Value) || _role.Any() && !_role.Contains(roleclaims.Value))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Patient", Action = "Accessdenied" }));
                return;
            }
        }

        #endregion
    }
}
