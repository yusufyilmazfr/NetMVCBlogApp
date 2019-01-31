using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class SessionAdminModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
    }
}