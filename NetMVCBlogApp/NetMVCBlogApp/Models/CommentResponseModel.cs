using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class CommentResponseModel
    {
        [Required]
        public int CommentID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Text { get; set; }
    }
}