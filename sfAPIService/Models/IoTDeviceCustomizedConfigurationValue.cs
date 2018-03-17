using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTDeviceCustomizedConfigurationValueModel
    {
        public class Detail
        {
            public string IoTHubDeviceId { get; set; }
            public int CustomizedConfigurationId { get; set; }
            public string Value { get; set; }
        }
        public class Edit
        {
            public List<ConfigurationModel> Configuration { get; set; }
        }
        public class ConfigurationModel
        {
            [Required]
            public int Id { get; set; }
            public string Value { get; set; }
        }
        
    }
}