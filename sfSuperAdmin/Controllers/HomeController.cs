using sfSuperAdmin.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sfSuperAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["loginMessage"] != null)
                ViewBag.LoginMessage = Session["loginMessage"].ToString();
            if (Session["toastLevel"] != null)
                ViewBag.ToastLevel = Session["toastLevel"].ToString();

            /* Read Cookie */
            if (Request.Cookies["rememberMe"] != null)
            {
                NameValueCollection qscoll = HttpUtility.ParseQueryString(Request.Cookies["rememberMe"].Value);
                ViewBag.CookieEmail = qscoll["email"];
                ViewBag.CookiePassword = qscoll["password"];
                ViewBag.CookieRememberMe = "checked";
            }

            return View("Login");
        }

        public async Task<ActionResult> DoLogin()
        {
            if (Request.Form["email"] != null && Request.Form["password"] != null)
            {
                Session["email"] = Request.Form["email"];
                Session["password"] = Request.Form["password"];

                if (Request.Form["rememberMe"] != null)
                    Session["rememberMe"] = true;
                else
                    Session["rememberMe"] = false;

                return await GetAuthenticationToken();                        
            }

            return View("Login");
        }

        private async Task<ActionResult> GetAuthenticationToken()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._deviceTypeEndPoint, null);      //Just Pick up a light way Authentication API

                /* Set RememberMe Cookie or Destroy Cookie */
                HttpCookie rememberMeCookie = new HttpCookie("rememberMe");
                if ((Session["rememberMe"] != null) && (bool.Parse(Session["rememberMe"].ToString())))
                {
                    rememberMeCookie.Values.Add("email", Session["email"].ToString());
                    rememberMeCookie.Values.Add("password", Session["password"].ToString());
                    rememberMeCookie.Expires = DateTime.Now.AddYears(1);
                }
                else
                {
                    rememberMeCookie.Values.Add("email", Session["email"].ToString());
                    rememberMeCookie.Values.Add("password", Session["password"].ToString());
                    rememberMeCookie.Expires = DateTime.Now.AddYears(-1);
                }
                Response.Cookies.Add(rememberMeCookie);
                
                return RedirectToAction("Index", "Company");
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "invalid session")
                {
                    Session["toastLevel"] = "warning";
                    Session["loginMessage"] = "Please Login";
                }
                else
                {
                    Session["toastLevel"] = "error";
                    Session["loginMessage"] = "Authentication Fail.";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + Session["email"]);
                    logMessage.AppendLine("password:" + Session["password"]);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DoLogout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
    }
}