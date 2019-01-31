using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class AdminLoginModel
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }

        public byte[] ProfileImage { get; set; }
    }
}