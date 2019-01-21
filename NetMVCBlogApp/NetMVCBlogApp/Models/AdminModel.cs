using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class AdminModel
    {
        public int ID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Mail { get; set; }

        public string Image { get; set; }

        [Required]
        public string AboutMe { get; set; }

        [Required]
        public string ShortDescription { get; set; }
    }
}