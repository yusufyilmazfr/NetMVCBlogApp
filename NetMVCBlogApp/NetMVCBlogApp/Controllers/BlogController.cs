using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;
using NetMVCBlogApp.Models;

namespace NetMVCBlogApp.Controllers
{
    public class BlogController : Controller
    {

        BlogDBEntities context = new BlogDBEntities();

        // GET: Blog
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            BlogDetailsModel blog = context.Post.Where(i => i.ID == id).Select(i => new BlogDetailsModel
            {
                ID = i.ID,
                Name = i.Name,
                Image = i.Image,
                Text = i.Text,
                addedDate = i.AddedDate,

                Comments = i.Comment.Select(j => new BlogCommentModel
                {
                    CommentID = j.ID,
                    Name = j.Name,
                    Text = j.Text,
                    Image = j.Image,
                    AddedDate = j.AddedDate,

                    CommentResponses = j.CommentResponse.Select(k => new BlogCommentResponseModel
                    {
                        Name = k.Name,
                        Text = k.Text,
                        Image = k.Image,
                        AddedDate = k.AddedDate

                    }).ToList()

                }).ToList(),

            }).FirstOrDefault();

            if (blog == null)
                return HttpNotFound();


            return View(blog);

        }

        public string AddComment(CommentModel model)
        {
            if (ModelState.IsValid)
            {
                context.Comment.Add(new Comment()
                {
                    PostID = model.BlogID,
                    Name = model.Name,
                    Text = model.Text,
                    Mail = model.Email,
                    AddedDate = DateTime.Now,
                    Image = "he" + new Random().Next(1, 5) + ".png"
                });

                context.SaveChanges();
                return "yes";
            }
            return "no";
        }

        public string AddCommentResponse(CommentResponseModel model)
        {
            if (ModelState.IsValid)
            {
                context.CommentResponse.Add(new CommentResponse()
                {
                    CommentID = model.CommentID,
                    Mail = model.Email,
                    Name = model.Name,
                    Text = model.Text,
                    AddedDate = DateTime.Now,
                    Image = "he" + new Random().Next(1, 5) + ".png"
                });

                context.SaveChanges();
                return "yes";
            }
            return "no";
        }

    }
}