using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace sfAPIService.Models
{
    public class RefCultureInfoModels
    {
        public string CultureCode { get; set; }
        public string Name { get; set; }

        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }
    }
}