using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class SuperAdminModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool DeletedFlag { get; set; }
        }

        public class Edit
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool DeletedFlag { get; set; }
        }
    }
}