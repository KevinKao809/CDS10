using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace sfAdmin.Models
{
    public class LoginMsgSession
    {        
        public string message;
        public string toastLevel;

        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        public static LoginMsgSession LoadByJsonString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;

            return new JavaScriptSerializer().Deserialize<LoginMsgSession>(jsonString);
        }
    }
}