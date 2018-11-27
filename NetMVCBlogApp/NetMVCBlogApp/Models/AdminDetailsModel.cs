using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class AdminDetailsModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string AboutMe { get; set; }
        public string ShortDescription { get; set; }
        public string Image { get; set; }
    }
}