using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class ContactMailModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Mail { get; set; }

        [Required]
        public string Text { get; set; }
    }
}