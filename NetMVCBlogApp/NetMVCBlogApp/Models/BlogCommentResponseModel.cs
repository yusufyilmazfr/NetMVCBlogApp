using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class BlogCommentResponseModel
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public DateTime AddedDate { get; set; }
    }
}