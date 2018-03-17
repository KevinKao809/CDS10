using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace sfAdmin.Models
{
    public class CompanySession
    {
        public int id;
        public string name;
        public string shortName;
        public string photoURL;
        public string allowDomain;
        public double lat;
        public double lng;

        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        public static CompanySession LoadByJsonString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;

            return new JavaScriptSerializer().Deserialize<CompanySession>(jsonString);
        }
    }

    //public class Location
    //{
    //    public double Latitude;
    //    public double Longitude;
    //}

    public class CompanyModel
    {         
        public async Task<CompanySession> GetCompanySessionData()
        {
            if (HttpContext.Current.Session["compSession"] == null)
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string CompanyEntiry = await apiHelper.callAPIService("GET", Global._companyEndPoint, null);
                dynamic companyObj = JObject.Parse(CompanyEntiry);

                CompanySession compSession = new CompanySession();
                if (companyObj.ShortName != null)
                    compSession.shortName = companyObj.ShortName;
                else
                    compSession.shortName = companyObj.Name;

                compSession.name = companyObj.Name;
                compSession.photoURL = companyObj.LogoURL;
                compSession.allowDomain = companyObj.AllowDomain;
                compSession.id = companyObj.Id;
                compSession.lat = companyObj.Latitude;
                compSession.lng = companyObj.Longitude;

                HttpContext.Current.Session["compSession"] = compSession.Serialize();
            }

            return CompanySession.LoadByJsonString(HttpContext.Current.Session["compSession"].ToString());
        }        
    }
}