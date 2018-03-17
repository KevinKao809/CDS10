using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;

using System.Web.Helpers;
using sfAPIService.Models;
using Microsoft.Owin.Security;
using sfShareLib;

namespace sfAPIService.Providers
{
    public class UserClaims
    {
        public bool IsAuthenticated { get; set; }
        public int CompanyId { get; set; }
    }
    public class OAuthProviders : OAuthAuthorizationServerProvider
    {
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
           context.Validated();
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //username, password, role
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var FormDates = await context.Request.ReadFormAsync();
            string email = FormDates.Get("email");
            string password = FormDates.Get("password");
            string role = FormDates.Get("role");
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                context.SetError("Authentication Fail", "Incomplete parameters");
            }

            UserClaims userClaims = loginAuthentication(email, password, role);
            if (userClaims.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Roles", role.ToLower(), ClaimValueTypes.String));
                identity.AddClaim(new Claim("CompanyId", userClaims.CompanyId.ToString(), ClaimValueTypes.Integer32));

                //Set current principal
                var claimPrincipal = new ClaimsPrincipal(identity);
                Thread.CurrentPrincipal = claimPrincipal;

                var ticket = new AuthenticationTicket(identity, addTokenOtherInfo(email, role));
                context.Validated(ticket);
                
                //context.Validated(identity);  //original
            }
            else
            {
                //帳密驗證失敗
                //context.Response.StatusCode = 404;
                context.SetError("Authentication Fail", "Authentication Fail.");
                
            }
        }

        //change token return format
        public override System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        //帳密驗證
        private  UserClaims loginAuthentication(string id, string password, string role)
        {
            UserClaims userClaims = new UserClaims();
            userClaims.IsAuthenticated = false;
            userClaims.CompanyId = 0;

            switch (role)
            {
                case "superadmin":
                    using (var ctx = new SFDatabaseEntities())
                    {
                        var superAdmin = ctx.SuperAdmin
                            .Where(s => s.Email == id && s.DeletedFlag==false)
                            .Select(s => new { s.Password }).FirstOrDefault();

                        try
                        {
                            if (Crypto.VerifyHashedPassword(superAdmin.Password, password))
                                userClaims.IsAuthenticated = true;
                        }
                        catch
                        {
                        }
                    }
                    break;
                case "admin":
                    using (var ctx = new SFDatabaseEntities())
                    {
                        var employee = ctx.Employee
                            .Where(s => s.Email == id && s.DeletedFlag == false && s.Company.DeletedFlag == false)
                            .Select(s => new { s.Password }).FirstOrDefault();

                        try
                        {
                            if (Crypto.VerifyHashedPassword(employee.Password, password))
                                userClaims.IsAuthenticated = true;
                        }
                        catch
                        {
                        }
                    }
                    break;
                case "device":
                    AccountModels accountModels = new AccountModels();
                    userClaims.IsAuthenticated = accountModels.CheckIoTDevicePassword(id, password);
                    break;
                case "external":
                    using (var ctx = new SFDatabaseEntities())
                    {
                        var company = ctx.Company
                            .Where(s => s.ExtAppAuthenticationKey == password && s.DeletedFlag == false)
                            .Select(s => new { s.Id }).FirstOrDefault();
                        if (company != null)
                        {
                            userClaims.IsAuthenticated = true;
                            userClaims.CompanyId = company.Id;
                        }
                    }
                    break;
            }
            return userClaims;
        }

        //Add return token info
        private AuthenticationProperties addTokenOtherInfo(string id, string role)
        {
            var tokenOtherInfo = new AuthenticationProperties(new Dictionary<string, string> { });
            switch (role)
            {
                case "admin":
                    using (var ctx = new SFDatabaseEntities())
                    {
                        var employee = ctx.Employee
                            .Where(s => s.Email == id && s.DeletedFlag == false)
                            .Select(s => new EmployeeModels.Detail
                            {
                                Id = s.Id,
                                CompanyId = s.CompanyId,
                                EmployeeNumber = s.EmployeeNumber,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
                                Email = s.Email,
                                PhotoURL = s.PhotoURL,
                                AdminFlag = s.AdminFlag,
                                Lang = s.Lang
                            }).FirstOrDefault();

                        if (employee != null)
                        {
                            var employeeTokenInfo = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                { "Id", employee.Id.ToString()},
                                { "CompanyId", employee.CompanyId.ToString()},
                                { "EmployeeNumber", (employee.EmployeeNumber!=null) ? employee.EmployeeNumber : ""},
                                { "FirstName", (employee.FirstName!=null) ? employee.FirstName.ToString() : ""},
                                { "LastName", (employee.LastName!=null) ? employee.LastName.ToString() : ""},
                                { "Email", employee.Email},
                                { "PhotoURL", (employee.PhotoURL!=null) ? employee.PhotoURL.ToString() : ""},
                                { "Lang", (employee.Lang!=null) ? employee.Lang.ToString() : ""},
                                { "AdminFlag", employee.AdminFlag.ToString()}
                            });
                            return employeeTokenInfo;
                        }
                    }
                    break;
                case "superadmin":
                    using (var ctx = new SFDatabaseEntities())
                    {
                        var superAdmin = ctx.SuperAdmin
                            .Where(s => s.Email == id && s.DeletedFlag == false)
                            .Select(s => new SuperAdminModels.Detail {
                                Id = s.Id,
                                FirstName = s.FirstName,
                                LastName = s.LastName,
                                Email = s.Email
                            }).FirstOrDefault();

                        if (superAdmin != null)
                        {
                            var superAdminTokenInfo = new AuthenticationProperties(new Dictionary<string, string>
                            {
                                { "Id", superAdmin.Id.ToString() },
                                { "FirstName", (superAdmin.FirstName!=null) ? superAdmin.FirstName.ToString() : "" },
                                { "LastName", (superAdmin.LastName!=null) ? superAdmin.LastName.ToString() : "" },
                                { "Email", superAdmin.Email}
                            });
                            return superAdminTokenInfo;
                        }
                    }
                    break;
            }
            return null;
        }
    }
}