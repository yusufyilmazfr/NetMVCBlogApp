using NetMVCBlogApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class BlogDetailsModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public DateTime addedDate { get; set; }
        public DateTime modifiedDate { get; set; }


        public AdminDetailsModel adminShortDetails { get; set; }
        public List<BlogCommentModel> Comments { get; set; }
    }
}