using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTDeviceSystemConfigurationValueModel
    {
        public class Edit_ConfigurationValue
        {
            [Required]
            public int Id { get; set; }
            [Required]
            public bool EnableFlag { get; set; }
            [Required]
            public string Value { get; set; }
        }
        
        public class Edit
        {
            public Edit_ConfigurationValue[] configurationList { get; set; }
        }               
        
    }
}