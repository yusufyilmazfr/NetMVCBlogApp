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

            ViewBag.CategoryID = new SelectList(context.Category, "ID", "Name");

            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewPost(PostModel model)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ModelState.IsValid)
            {
                context.Post.Add(new Post()
                {
                    Name = model.Name,
                    Text = HttpUtility.HtmlEncode(model.Text),
                    AddedDate = DateTime.Now,
                    CategoryID = model.CategoryID,
                    Image = "06-1-1440x1080.jpg"
                });

                context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(context.Category, "ID", "Name");

            return View(model);
        }


        public ActionResult EditPost(int? ID)
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

            ViewBag.CategoryID = new SelectList(context.Category, "ID", "Name");


            model.Text = HttpUtility.HtmlDecode(model.Text);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditPost(Post model)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ModelState.IsValid)
            {
                Post post = context.Post.Find(model.ID);
                post.Text = HttpUtility.HtmlEncode(model.Text);
                post.Name = model.Name;
                post.CategoryID = model.CategoryID;
                post.isValid = model.isValid;

                context.SaveChanges();
                return RedirectToAction("Posts");
            }

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
                if (context.Category.Any(i => i.Name == model.Name))
                {
                    return View(model);
                }
                context.Category.Add(model);
                context.SaveChanges();

                return RedirectToAction("Categories");
            }

            return View(model);
        }

        public ActionResult EditCategorie(int? ID)
        {
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
        public ActionResult EditCategorie(Category model)
        {
            if (ModelState.IsValid)
            {
                context.Entry(model).State = System.Data.Entity.EntityState.Modified;
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

            var posts = context.Post.Where(i => i.CategoryID == ID).ToList();

            foreach (Post item in posts)
            {
                item.CategoryID = context.Category.Where(i => i.Name == "General").Select(i => i.ID).FirstOrDefault();
            }


            Category selectedCategory = context.Category.Find(ID);
            context.Category.Remove(selectedCategory);

            context.SaveChanges();

            return RedirectToAction("Categories");
        }

        public ActionResult Comments()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Comment.ToList());
        }

        public ActionResult DeleteComment(int? ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment model = context.Comment.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteComment(int ID)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            Comment comment = context.Comment.Find(ID);
            context.Comment.Remove(comment);

            context.SaveChanges();

            return RedirectToAction("Comments");
        }

        public ActionResult EditComment(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment model = context.Comment.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditComment(EditCommentModel model)
        {
            if (ModelState.IsValid)
            {
                Comment comment = context.Comment.Find(model.ID);

                comment.Name = model.Name;
                comment.Mail = model.Mail;
                comment.Text = model.Text;
                comment.Image = model.Image;
                comment.isValid = model.isValid;
                comment.ModifiedDate = DateTime.Now;
                comment.PostID = model.PostID;

                context.SaveChanges();

                return RedirectToAction("Comments");
            }

            return View(model);
        }

    }


}