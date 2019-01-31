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
        BlogDBEntities context;
        public string title { get; set; }

        public HomeController()
        {
            context = new BlogDBEntities();
            title = context.Options.Select(i => i.Title).FirstOrDefault();
        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = string.Format("{0}", title);

            return View(context.Post.Where(i => i.isValid).OrderByDescending(i => i.ID).ToList());
        }

        [Route("Contact")]
        public ActionResult Contact()
        {
            ViewBag.Title = string.Format("{0} | {1}", title, "Contact");

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

        [Route("About-me")]
        public ActionResult AboutMe()
        {
            AdminDetailsModel model = context.Admin.Select(i => new AdminDetailsModel
            {
                Name = i.Name,
                LastName = i.LastName,
                AboutMe = i.AboutMe,
                Image = i.Image
            }).FirstOrDefault();

            ViewBag.Title = string.Format("{0} | {1}", title, "About Me");

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult HeaderItem()
        {
            OptionModel optionModel = context.Options.Select(i => new OptionModel()
            {
                HeaderText = i.HeaderText,
                Title = i.Title
            }).FirstOrDefault();

            ViewBag.Logo = context.Options.Select(i => i.Logo).FirstOrDefault();

            return PartialView(optionModel);
        }

        [ChildActionOnly]
        public PartialViewResult NavbarItem()
        {
            return PartialView(context.Social.FirstOrDefault());
        }

        [ChildActionOnly]
        public PartialViewResult ContactPageSocial()
        {
            return PartialView(context.Social.FirstOrDefault());
        }

    }
}