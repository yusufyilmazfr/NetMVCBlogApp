using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class PostModel
    {
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int CategoryID { get; set; }
    }
}