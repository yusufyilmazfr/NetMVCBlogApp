using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;
using NetMVCBlogApp.Models;

namespace NetMVCBlogApp.Controllers
{
    public class HomeController : Controller
    {
        BlogDBEntities context = new BlogDBEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View(context.Post.Where(i => i.isValid).OrderByDescending(i => i.ID).ToList());
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public string Contact(ContactMailModel model)
        {
            if (ModelState.IsValid)
            {
                Mail mail = new Mail();
                bool state = mail.ContactMail(model, context.Smtp.FirstOrDefault());

                if (state)
                    return "succeed";
                else
                    return "failed";
            }

            return "failed";
        }

        public ActionResult AboutMe()
        {
            AdminDetailsModel model = context.Admin.Select(i => new AdminDetailsModel
            {
                Name = i.Name,
                LastName = i.Image,
                AboutMe = i.AboutMe,
                Image = i.Image
            }).FirstOrDefault();

            return View(model);
        }
    }
}