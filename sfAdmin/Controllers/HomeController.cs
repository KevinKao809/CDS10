
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using i18n;
using i18n.Helpers;

namespace sfAdmin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {            
            if (Session["loginMsgSession"] != null)
            {
                LoginMsgSession logMsgSession = LoginMsgSession.LoadByJsonString(Session["loginMsgSession"].ToString());
                ViewBag.LoginMessage = logMsgSession.message;            
                ViewBag.ToastLevel = logMsgSession.toastLevel;
            }

            ViewBag.Version = Global._sfAdminVersion;

            /* Read Cookie */
            if (Request.Cookies["rememberMe"] != null )
            {
                NameValueCollection qscoll = HttpUtility.ParseQueryString(Request.Cookies["rememberMe"].Value);                
                ViewBag.CookieEmail = qscoll["email"];
                ViewBag.CookiePassword = qscoll["password"];
                ViewBag.CookieRememberMe = "checked";
            }
            var jsonString = "";
            var cookie = Request.Cookies["i18n.langtag"];
            if (cookie != null) jsonString = cookie.Value;
            else { jsonString = ""; }
            return View("Login");
        }
        [AllowAnonymous]
        public ActionResult SetLanguage(string langtag, string returnUrl)
        {
            if (langtag == null) langtag = "en";
            // If valid 'langtag' passed.
            i18n.LanguageTag lt = i18n.LanguageTag.GetCachedInstance(langtag);

            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
            { 
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                empSession.Lang = langtag;
                Session["empSession"] = empSession.Serialize();
            }

            if (lt.IsValid())
            {
                // Set persistent cookie in the client to remember the language choice.
                Response.Cookies.Add(new HttpCookie("i18n.langtag")
                {
                    Value = lt.ToString(),
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddYears(1)
                });
            }
            // Owise...delete any 'language' cookie in the client.
            else
            {
                var cookie = Response.Cookies["i18n.langtag"];
                if (cookie != null)
                {
                    cookie.Value = null;
                    cookie.Expires = DateTime.UtcNow.AddMonths(-1);
                }
            }
            // Update PAL setting so that new language is reflected in any URL patched in the 
            // response (Late URL Localization).
            System.Web.HttpContext.Current.SetPrincipalAppLanguageForRequest(lt);
            // Patch in the new langtag into any return URL.
            if (returnUrl.IsSet())
            {
                returnUrl = LocalizedApplication.Current.UrlLocalizerForApp.SetLangTagInUrlPath(HttpContext, returnUrl, UriKind.RelativeOrAbsolute, lt == null ? null : lt.ToString()).ToString();
            }
            // Redirect user agent as approp.

            return Json(new { status = "success", NewURL = returnUrl });
        }

        public async Task<ActionResult> DoLogin()
        {
            if (Request.Form["email"] != null && Request.Form["password"] != null)
            {
                EmployeeSession empSession = new EmployeeSession();
                empSession.email = Request.Form["email"];
                empSession.password = Request.Form["password"];

                if (Request.Form["rememberMe"] != null)
                    empSession.rememberMe = true;
                else
                    empSession.rememberMe = false;

                Session["empSession"] = empSession.Serialize();
                return await GetAuthenticationToken();
            }
            return View("Login");
        }
        public async Task<ActionResult> ShowProfile()
        {
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {   
                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;
                ViewBag.CompanyID = compSession.id;

                /* Setup Employee Data on Page */                
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;
                ViewBag.Id = empSession.id;
                ViewBag.EmployeeNumber= empSession.employeeNumber;
                ViewBag.AdminFlag = empSession.adminFlag;
                ViewBag.Lang = empSession.Lang;
                KeyValuePair<string, i18n.LanguageTag>[] langx = LanguageHelpers.GetAppLanguages().OrderBy(x => x.Key).ToArray();
                ViewBag.langs = JsonConvert.SerializeObject(langx);

            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
                return RedirectToAction("Index", "Home");
            }
            return PartialView("Profile");
        }

        private async Task<ActionResult> GetAuthenticationToken()
        {
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string deviceList = await apiHelper.callAPIService("GET", Global._deviceTypeEndPoint, null);      //Just Pick up a light way Authentication API

                /* Set RememberMe Cookie or Destroy Cookie */
                HttpCookie rememberMeCookie = new HttpCookie("rememberMe");
                
                if (empSession.rememberMe)
                {
                    rememberMeCookie.Values.Add("email", empSession.email);
                    rememberMeCookie.Values.Add("password", empSession.password);
                    rememberMeCookie.Expires = DateTime.Now.AddYears(1);
                }
                else
                {                    
                    rememberMeCookie.Expires = DateTime.Now.AddYears(-1);
                }
                Response.Cookies.Add(rememberMeCookie);
                Models.Employee employee = new Models.Employee();
                string[] redirectPath = employee.getRedirectionPath().Split('/');
                return RedirectToAction(redirectPath[1], redirectPath[0]);
            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> LoginBySA()
        {
            EmployeeSession empSession = new EmployeeSession();
            try
            {
                string inputCredential = Request.Form["inputCredential"];
                var inputCredentialByte = System.Convert.FromBase64String(inputCredential);
                string inputCredentialString = System.Text.Encoding.UTF8.GetString(inputCredentialByte);
                string[] credential = inputCredentialString.Split(':');
                
                empSession.email = credential[0];
                empSession.password = credential[1];
                empSession.companyId = int.Parse(Request.Form["inputCompanyId"]);
                empSession.adminFlag = true;
                Session["empSession"] = empSession.Serialize();
                Session["loginBySA"] = true;

                /* Set Company Entity */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string CompanyEntiry = await apiHelper.callAPIService("GET", Global._companyEndPoint, null);
                dynamic companyObj = JObject.Parse(CompanyEntiry);

                CompanySession compSession = new CompanySession();
                if (companyObj.ShortName != null)
                    compSession.shortName= companyObj.ShortName;
                else
                    compSession.shortName = companyObj.Name;

                compSession.name = companyObj.Name;
                compSession.photoURL = companyObj.LogoURL;
                compSession.id = companyObj.Id;
                compSession.lat = companyObj.Latitude;
                compSession.lng = companyObj.Longitude;
                Session["compSession"] = compSession.Serialize();

                /* Get User Authentication */
                return await GetAuthenticationToken();
            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult DoLogout()
        {
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> ReqAction()
        {
            string jsonString = "", postData = "", endPoint = "";            
            if (Request.QueryString["action"] != null)
            {
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
             
                        case "changepassword":
                            endPoint = Global._employeeEndPoint + "/" + Request.QueryString["Id"] + "/changepassword";
                            postData = Request.Form.ToString();
                            jsonString = apiHelper.changePassword("put", endPoint, postData);
                            break;
                       
                        case "updateemployee":
                            {
                                endPoint = Global._employeeEndPoint;
                                if (Request.QueryString["Id"] != null)
                                {
                                    postData = Request.Form.ToString();
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                    jsonString = await apiHelper.callAPIService("put", endPoint, postData);

                                }

                                if (Request.Files.Count > 0)
                                {
                                    //admin-api/Company/{id}/Image
                                    var ImageEndPoint = endPoint + "/Image";
                                    byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                    Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                    jsonString = await apiHelper.putUploadFile(ImageEndPoint, byteFile, Request.Files[0].FileName);
                                }

                                break;
                            }

                      
                        default:

                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower() == "invalid session")
                        Response.StatusCode = 401;
                    else
                    {
                        StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                        logMessage.AppendLine("EndPoint:" + endPoint);
                        logMessage.AppendLine("Action:" + Request.QueryString["action"].ToString());
                        logMessage.AppendLine("PostData:" + Request.Form.ToString());
                        Global._sfAppLogger.Error(logMessage);
                        Response.StatusCode = 500;
                        jsonString = ex.Message;
                    }
                }
            }

            return Content(JsonConvert.SerializeObject(jsonString), "application/json");
        }
    }
    
}