using NetMVCBlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;

namespace NetMVCBlogApp.Controllers
{
    public class AccountController : Controller
    {
        BlogDBEntities context = new BlogDBEntities();

        // GET: Account
        public ActionResult Index()
        {
            if (Session["admin"] == null) { return RedirectToAction("Login"); }

            return View();
        }

        public ActionResult Login()
        {
            if (Session["admin"] != null) { return RedirectToAction("Index"); }

            return View();
        }

        [HttpPost]
        public string Login(AdminLoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (context.Admin.Where(i => i.Username == model.username && i.Password == model.password).Any())
                {
                    Session["admin"] = context.Admin.Where(i => i.Username == model.username && i.Password == model.password).First();

                    return "yes";
                }
            }
            return "no";
        }

        public ActionResult Logout()
        {
            Session["admin"] = null;
            return RedirectToAction("Login");
        }

        public new ActionResult User()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            AdminModel model = context.Admin.Select(i => new AdminModel()
            {
                Username = i.Username,
                Name = i.Name,
                Mail = i.Mail,
                LastName = i.LastName,
                AboutMe = i.AboutMe,
                Image = i.Image,
                ShortDescription = i.ShortDescription

            }).First();

            return View(model);
        }

        [HttpPost]
        public new ActionResult User(AdminModel model)
        {
            if (ModelState.IsValid)
            {
                Admin admin = context.Admin.First();

                admin.Username = model.Username;
                admin.Name = model.Name;
                admin.LastName = model.LastName;
                admin.Mail = model.Mail;
                admin.ShortDescription = model.ShortDescription;
                admin.AboutMe = model.AboutMe;
                admin.Image = "admin.jpg";

                context.SaveChanges();

                return View(model);
            }

            return View(model);
        }

        public ActionResult Blogs()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Post.ToList());
        }

        public ActionResult Comments()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Comment.ToList());
        }
    }
}