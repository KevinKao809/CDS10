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
    public class EquipmentController : Controller
    {
        // GET: Equipment
        public async Task<ActionResult> Index()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.EquipmentList = await apiHelper.callAPIService("GET", Global._equipmentInCompanyEndPoint, null);
                ViewBag.EquipmentClassList = await apiHelper.callAPIService("GET", Global._equipmentClassInCompanyEndPoint, null);
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);
                ViewBag.IoTDeviceList = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);
                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyId = compSession.id;
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                /* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;
                ViewBag.MenuItem = "menuEquipment";
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
            string jsonString = "", postData = "", endPoint = "";            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                if (Request.QueryString["action"] != null)
                {
                    switch (Request.QueryString["action"].ToLower())
                    {
                        case "getequipment":
                            {
                                endPoint = Global._equipmentInCompanyEndPoint;
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "getequipbyfactory":
                            {
                                string factoryId = Request.QueryString["factoryId"];
                                if (factoryId != null)
                                {
                                    if(factoryId.ToLower()== "all")
                                        endPoint = Global._equipmentInCompanyEndPoint;
                                    else
                                        endPoint = Global._equipmentInFactoryEndPoint +"/"+ factoryId;
                                }
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;

                            }
                        case "addequipment":
                            {
                                endPoint = Global._equipmentEndPoint;
                                postData = Request.Form.ToString();

                                var queryString = HttpUtility.ParseQueryString(postData);
                                if (queryString["Location"] != null && queryString["Location"] == "Factory")
                                {
                                    string factoryEndPoint = Global._factoryEndPoint + "/" + queryString["FactoryId"];
                                    string factoryString = await apiHelper.callAPIService("get", factoryEndPoint, null);
                                    dynamic factoryResult = JObject.Parse(factoryString);
                                    postData = postData + "&Latitude=" + factoryResult.Latitude;
                                    postData = postData + "&Longitude=" + factoryResult.Longitude;
                                }

                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                dynamic jsonResult = JObject.Parse(jsonString);
                                if (Request.Files.Count > 0)
                                {

                                    if(jsonResult.id != null)
                                    {
                                        string entityID = jsonResult.id;
                                        endPoint = endPoint + "/" + entityID + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                    }
                                    if (Request.Files.Count > 0)
                                    {
                                        //admin-api/Company/{id}/Image
                                        var ImageEndPoint = endPoint + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(ImageEndPoint, byteFile, Request.Files[0].FileName);
                                    }

                                }
                                break;
                            }
                        case "deleteequipment":
                            {
                                endPoint = Global._equipmentEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, postData);
                                break;
                            }
                        case "updateequipment":
                            {

                                endPoint = Global._equipmentEndPoint;
                                if(Request.QueryString["Id"]!=null)
                                {
                                    postData = Request.Form.ToString();

                                    var queryString = HttpUtility.ParseQueryString(postData);
                                    if (queryString["Location"] != null && queryString["Location"] == "Factory")
                                    {
                                        string factoryEndPoint = Global._factoryEndPoint + "/" + queryString["FactoryId"];
                                        string factoryString = await apiHelper.callAPIService("get", factoryEndPoint, null);
                                        dynamic factoryResult = JObject.Parse(factoryString);
                                        postData = postData + "&Latitude=" + factoryResult.Latitude;
                                        postData = postData + "&Longitude=" + factoryResult.Longitude;
                                    }

                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                    jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                    if (Request.Files.Count > 0)
                                    {
                                        //admin-api/Company/{id}/Image
                                        var ImageEndPoint = endPoint + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(ImageEndPoint, byteFile, Request.Files[0].FileName);
                                    }
                                    // Update Meta-Data
                                    string metaDataPost = "";
                                    string[] phrases = postData.Split('&');
                                    foreach (var input in phrases)
                                        if (input.StartsWith("metaDatas"))
                                            metaDataPost = metaDataPost + input + "&";

                                    if (!string.IsNullOrEmpty(metaDataPost))
                                    {
                                        var MetaDataEndPoint = endPoint + "/MetaData";
                                        jsonString = await apiHelper.callAPIService("put", MetaDataEndPoint, metaDataPost);
                                    }                                    
                                }
                                break;
                            }
                        case "getmetadata":
                            {
                                endPoint = Global._equipmentEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/MetaData";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                    }
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
                    System.Diagnostics.Debug.Print("EndPoint:" + endPoint);
                    System.Diagnostics.Debug.Print("Action:" + Request.QueryString["action"].ToString());
                    System.Diagnostics.Debug.Print("PostData:" + Request.Form.ToString());
                    System.Diagnostics.Debug.Print("Message:" + ex.Message);
                    Global._sfAppLogger.Error(logMessage);
                    Response.StatusCode = 500;
                    jsonString = ex.Message;
                }
            }            
            return Content(JsonConvert.SerializeObject(jsonString), "application/json");
        }
    }
}