using NetMVCBlogApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;
using System.Net;
using System.IO;
using System.Data.Entity.Validation;

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

        [HttpPost]
        public string ForgetPassword()
        {
            try
            {
                string tempPassword = new Random().Next(88888888, 99999999).ToString();

                context.Admin.FirstOrDefault().Password = CreateMD5.Create(tempPassword);
                context.SaveChanges();

                Mail mail = new Mail();
                bool result = mail.SendNewPassword(context.Smtp.FirstOrDefault(), context.Admin.Select(i => i.Mail).FirstOrDefault(), tempPassword);

                if (result)
                    return "succeed";

                return "failed";
            }
            catch (Exception)
            {
                return "failed";
            }
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
                string password = CreateMD5.Create(model.password);

                if (context.Admin.Where(i => i.Username == model.username && i.Password == password).Any())
                {
                    Session["admin"] = context.Admin.Where(i => i.Username == model.username && i.Password == password).First();

                    return "succeed";
                }
            }
            return "failed";
        }

        public ActionResult Logout()
        {
            Session["admin"] = null;
            return RedirectToAction("Login");
        }

        public new ActionResult User()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Admin.First());
        }

        [HttpPost]
        public new JsonResult User(Admin model)
        {

            Admin admin = context.Admin.First();

            model.ModifiedDate = DateTime.Now;


            HttpPostedFileBase adminImage = Request.Files.Get("uploadPhoto");

            if (ModelState.IsValid)
            {
                if (adminImage != null && adminImage.ContentLength > 0)
                {
                    string ext = Path.GetExtension(adminImage.FileName).ToUpper();

                    if (ext == ".JPEG" || ext == ".PNG" || ext == ".JPG")
                    {
                        string path = Server.MapPath("~/Content/img/admin") + ext.ToLower();

                        adminImage.SaveAs(path);

                        model.Image = "admin" + ext.ToLower();
                    }
                }
                else
                {
                    model.Image = "admin.jpg";
                }


                context.Entry<Admin>(model).State = System.Data.Entity.EntityState.Modified;

                context.Entry<Admin>(model).Property("Password").IsModified = false;

                //context.Entry<Admin>(model).State = System.Data.Entity.EntityState.Modified;

                //context.Entry<Admin>(model).Property(i => i.Password).IsModified = false;
                //context.Entry<Admin>(model).Property(i => i.AddedDate).IsModified = false;

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

                HttpPostedFileBase file = Request.Files.Get("uploadPhoto");

                if (file != null && file.ContentLength > 0)
                {
                    string ext = Path.GetExtension(file.FileName).ToUpper();
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);

                    string path = string.Empty;

                    if (ext == ".JPEG" || ext == ".PNG" || ext == ".JPG")
                    {
                        path = Server.MapPath(string.Format("~/Content/img/post/{0}{1}", fileName, ext));

                        while (System.IO.File.Exists(path))
                        {
                            path += new Random().Next(8888);
                        }

                        file.SaveAs(path);
                    }


                    context.Post.Add(new Post()
                    {
                        Name = model.Name,
                        Text = HttpUtility.HtmlEncode(model.Text),
                        AddedDate = DateTime.Now,
                        CategoryID = model.CategoryID,
                        Image = file.FileName,
                        SeoLink = new CreateFriendlyUrl().GenerateSlug(model.Name)
                    });


                    context.SaveChanges();

                    return RedirectToAction("Index");
                }

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
        public ActionResult EditComment(Comment model)
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

                //model.ModifiedDate = DateTime.Now;

                //context.Entry<Comment>(model).State = System.Data.Entity.EntityState.Modified;

                //context.Entry<Comment>(model).Property(i => i.Image).IsModified = false;
                //context.Entry<Comment>(model).Property(i => i.Mail).IsModified = false;
                //context.Entry<Comment>(model).Property(i => i.Post).IsModified = false;
                //context.Entry<Comment>(model).Property(i => i.PostID).IsModified = false;


                context.SaveChanges();

                return RedirectToAction("Comments");
            }

            return View(model);
        }

        public ActionResult SmtpSettings()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            return View(context.Smtp.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult SmtpSettings(Smtp smtp)
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Entry<Smtp>(smtp).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return View(smtp);
                }
            }
            return View(smtp);
        }

        public ActionResult PasswordSettings()
        {
            if (Session["admin"] == null) { return RedirectToAction("Index"); }

            PasswordModel passwordModel = new PasswordModel()
            {
                LastPassword = context.Admin.Select(i => i.Password).FirstOrDefault()
            };

            return View();
        }

        [HttpPost]
        public string PasswordSettings(PasswordModel model)
        {

            string adminPassword = context.Admin.Select(i => i.Password).FirstOrDefault();
            string lastPassword = CreateMD5.Create(model.LastPassword);

            if (ModelState.IsValid)
            {
                if (adminPassword.Equals(lastPassword) && model.NewPassword.Equals(model.ConfirmPassword))
                {
                    Admin admin = context.Admin.FirstOrDefault();
                    admin.Password = CreateMD5.Create(model.NewPassword);

                    Session["admin"] = admin;

                    context.SaveChanges();

                    return "succeed";
                }
            }

            return "failed";
        }
    }


}