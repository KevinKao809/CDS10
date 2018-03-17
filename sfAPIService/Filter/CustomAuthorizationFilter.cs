using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Diagnostics.Contracts;

namespace sfAPIService.Filter
{
    public class CustomAuthorizationFilter : AuthorizationFilterAttribute
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            if (IsAllowAnonymous(actionContext))
                return Task.FromResult<object>(null);

            var actionFilter = actionContext.ActionDescriptor.GetFilters();
            foreach (CustomAuthorizationFilter filter in actionFilter)
            {
                if (filter.ClaimType == this.ClaimType)
                    this.ClaimValue = filter.ClaimValue;
            }           

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (!principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }

            if (ClaimValue.Contains(','))
            {
                List<string> claimValueList = ClaimValue.Split(',').ToList<string>();
                for (int i = 0; i < claimValueList.Count; i++)
                {
                    claimValueList[i] = claimValueList[i].Trim();
                }

                if (!(principal.HasClaim(x => x.Type == ClaimType && claimValueList.Contains(x.Value))))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    return Task.FromResult<object>(null);
                }
            }
            else
            {
                if (!(principal.HasClaim(x => x.Type == ClaimType && x.Value == ClaimValue)))
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    return Task.FromResult<object>(null);
                }
            }           

            //User is Authorized, complete execution
            return Task.FromResult<object>(null);
        }        
        private static bool IsAllowAnonymous(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }
    }
    
}