using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;
using NetMVCBlogApp.Models;

namespace NetMVCBlogApp.Controllers
{
    public class AsideController : Controller
    {
        BlogDBEntities context = new BlogDBEntities();

        // GET: Aside

        [ChildActionOnly]
        public PartialViewResult Index()
        {
            return PartialView();
        }

        [ChildActionOnly]
        public PartialViewResult AboutMe()
        {
            AdminDetailsModel admin = context.Admin.Select(i => new AdminDetailsModel
            {
                Image = i.Image,
                Name = i.Name,
                AboutMe = i.AboutMe
            }).FirstOrDefault();

            return PartialView(admin);
        }

        [ChildActionOnly]
        public PartialViewResult CategoryList()
        {
            return PartialView(context.Category.ToList());
        }

        [ChildActionOnly]
        public PartialViewResult RecentlyPost()
        {
            return PartialView(context.Post.OrderByDescending(i => i.ID).Where(i=>i.isValid).Take(6));
        }

        [ChildActionOnly]
        public PartialViewResult WrittenBy()
        {
            AdminDetailsModel admin = context.Admin.Select(i => new AdminDetailsModel
            {
                Image = i.Image,
                Name = i.Name,
                LastName = i.LastName,
                ShortDescription = i.ShortDescription,
                AboutMe = i.AboutMe
            }).FirstOrDefault();

            return PartialView(admin);
        }
    }
}