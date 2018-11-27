using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class BlogCommentModel
    {
        public int CommentID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Text { get; set; }
        public DateTime AddedDate { get; set; }

        public List<BlogCommentResponseModel> CommentResponses { get; set; }
    }
}