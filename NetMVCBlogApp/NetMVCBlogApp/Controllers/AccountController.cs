using NetMVCBlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;
using System.Net;

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
        public new JsonResult User(AdminModel model)
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

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }

        public ActionResult Posts()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Post.ToList());
        }

        public ActionResult NewPost()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            ViewBag.Categories = context.Category.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult NewPost(PostModel model)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ModelState.IsValid)
            {
                context.Post.Add(new Post()
                {
                    Name = model.Name,
                    Text = model.Text,
                    AddedDate = DateTime.Now,
                    CategoryID = model.CategoryID,
                    Image = "06-1-1440x1080.jpg"
                });

                context.SaveChanges();

                return RedirectToAction("Index");
            }


            ViewBag.Categories = context.Category.ToList();

            return View(model);
        }

        public ActionResult DeletePost(int? ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Post model = context.Post.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeletePost(int ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            Post model = context.Post.Find(ID);
            context.Post.Remove(model);
            context.SaveChanges();

            return RedirectToAction("Posts");
        }

        public ActionResult Categories()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Category.ToList());
        }

        public ActionResult NewCategorie()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View();
        }

        [HttpPost]
        public ActionResult NewCategorie(Category model)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ModelState.IsValid)
            {
                context.Category.Add(model);
                context.SaveChanges();

                return RedirectToAction("Categories");
            }

            return View(model);
        }

        public ActionResult DeleteCategorie(int? ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category model = context.Category.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCategorie(int ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            Category model = context.Category.Find(ID);
            context.Category.Remove(model);
            context.SaveChanges();

            return RedirectToAction("Categories");
        }

        public ActionResult Comments()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Comment.ToList());
        }

    }


}