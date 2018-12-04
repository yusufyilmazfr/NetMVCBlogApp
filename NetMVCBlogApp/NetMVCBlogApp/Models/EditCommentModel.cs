using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class EditCommentModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public int PostID { get; set; }

        [Required]
        public string Image { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Mail { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool isValid { get; set; }
    }
}