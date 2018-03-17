using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sfAdmin.Models
{    
    public class MessageElementModel
    {
        public int Id;
        public int ParentId;
        public string Name;
        public string DataType;
        public bool MandatoryFlag;
    }
}