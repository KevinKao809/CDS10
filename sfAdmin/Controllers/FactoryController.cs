using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sfAdmin.Controllers
{
    public class FactoryController : Controller
    {
        // GET: Factory
        public async Task<ActionResult> Index()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);
                ViewBag.CultureInfoList = await apiHelper.callAPIService("GET", Global._cultureInfoEndPoint, null);

                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;
                ViewBag.CompanyLat = compSession.lat;
                ViewBag.CompanyLng = compSession.lng;
                
                /* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;                
                ViewBag.MenuItem = "menuFactory";                
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

            return View();
        }

        public async Task<ActionResult> ReqAction()
        {
            ViewBag.Version = Global._sfAdminVersion;
            string jsonString = "";
            if (Request.QueryString["action"] != null)
            {
                string endPoint = Global._factoryEndPoint;
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    EmployeeSession empSession = null;
                    if (Session["empSession"] != null)
                        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "getfactory":
                            endPoint = Global._factoryInCompanyEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        case "deletefactory":
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            Global._sfAppLogger.Info("Delete Factory Completed; Factory ID:" + Request.QueryString["Id"] + ";Login Email:" + empSession.email);
                            break;
                        case "addfactory":
                            {
                                string postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);

                                if (Request.Files.Count > 0)
                                {
                                    //Get Object ID from API result
                                    dynamic jsonResult = JObject.Parse(jsonString);
                                    if (jsonResult.id != null)
                                    {
                                        string entityID = jsonResult.id;
                                        endPoint = endPoint + "/" + entityID + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                    }
                                }
                                Global._sfAppLogger.Info("Add Factory Completed; API Return:" + jsonString + ";Login Email:" + empSession.email);
                                break;
                            }
                        case "updatefactory":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                string postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                if (Request.Files.Count > 0)
                                {
                                    //admin-api/Factory/{id}/Image
                                    endPoint = endPoint + "/Image";

                                    byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                    Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                    jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                }
                                Global._sfAppLogger.Info("Update Factory Completed; Factory ID:" + Request.QueryString["Id"] + "; post Data:" + postData + ";Login Email:" + empSession.email);
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