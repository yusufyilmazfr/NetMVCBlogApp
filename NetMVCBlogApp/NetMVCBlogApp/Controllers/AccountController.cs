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
using System.Data.Entity.Infrastructure;
using System.Data.Entity;


namespace NetMVCBlogApp.Controllers
{
    public class AccountController : Controller
    {
        BlogDBEntities context = new BlogDBEntities();

        // GET: Account
        public ActionResult Index()
        {
            if (Session["admin"] == null) { return RedirectToAction("Login", "Account"); }

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
            ViewBag.ProfileImage = context.Admin.FirstOrDefault().Image;
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
                    Session["admin"] = context.Admin.Where(i => i.Username == model.username && i.Password == password).Select(i => new SessionAdminModel()
                    {
                        Id = i.ID,
                        Name = i.Name,
                        Lastname = i.LastName,
                        Password = i.Password,
                        Username = i.Username
                    }).FirstOrDefault();

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
            
            return View(context.Admin.First());
        }

        [HttpPost]
        public JsonResult User(Admin model, HttpPostedFileBase uploadPhoto)
        {
            if (ModelState.IsValid)
            {
                Admin admin = context.Admin.FirstOrDefault();

                if (uploadPhoto != null && uploadPhoto.ContentLength > 0)
                {
                    string ext = Path.GetExtension(uploadPhoto.FileName).ToUpper();

                    if (ext == ".JPEG" || ext == ".PNG" || ext == ".JPG")
                    {
                        MemoryStream ms = new MemoryStream();

                        uploadPhoto.InputStream.CopyTo(ms);

                        admin.Image = ms.ToArray();
                    }
                }

                admin.Username = model.Username;
                admin.Name = model.Name;
                admin.LastName = model.LastName;
                admin.AboutMe = model.AboutMe;
                admin.ShortDescription = model.ShortDescription;
                admin.AddedDate = model.AddedDate;
                admin.ModifiedDate = DateTime.Now;

                context.SaveChanges();

                return Json(model, JsonRequestBehavior.AllowGet);
            }

            return Json(null);
        }

        public ActionResult Posts()
        {
            return View(context.Post.ToList());
        }

        public ActionResult NewPost()
        {
            ViewBag.CategoryID = new SelectList(context.Category, "ID", "Name");

            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewPost(PostModel model)
        {
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
            Post model = context.Post.Find(ID);
            context.Post.Remove(model);
            context.SaveChanges();

            return RedirectToAction("Posts");
        }

        public ActionResult Categories()
        {
            return View(context.Category.ToList());
        }

        public ActionResult NewCategorie()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewCategorie(Category model)
        {
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
            return View(context.Comment.OrderByDescending(i => i.ID).ToList());
        }

        public ActionResult DeleteComment(int? ID)
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
        public ActionResult DeleteComment(int ID)
        {
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

        public ActionResult DeleteResponse(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CommentResponse model = context.CommentResponse.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteResponse(int ID)
        {
            CommentResponse comment = context.CommentResponse.Find(ID);
            context.CommentResponse.Remove(comment);

            context.SaveChanges();

            return RedirectToAction("Comments");
        }


        public ActionResult EditResponse(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CommentResponse model = context.CommentResponse.Find(ID);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult EditResponse(CommentResponse model)
        {
            if (ModelState.IsValid)
            {
                CommentResponse comment = context.CommentResponse.Find(model.ID);

                comment.Name = model.Name;
                comment.Mail = model.Mail;
                comment.Text = model.Text;
                comment.Image = model.Image;
                comment.isValid = model.isValid;
                comment.ModifiedDate = DateTime.Now;
                comment.CommentID = model.CommentID;
                comment.AddedDate = model.AddedDate;
                //COMMENT ID

                context.SaveChanges();

                return RedirectToAction("Comments");
            }

            return View(model);
        }

        public ActionResult SmtpSettings()
        {
            return View(context.Smtp.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult SmtpSettings(Smtp smtp)
        {
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

        public ActionResult General()
        {
            OptionModel optionModel = context.Options.Select(i => new OptionModel()
            {
                HeaderText = i.HeaderText,
                ID = i.ID,
                Lang = i.Lang,
                Title = i.Title
            }).FirstOrDefault();

            return View(optionModel);
        }

        [HttpPost]
        public ActionResult General(OptionModel model)
        {
            if (ModelState.IsValid || Request.Files.Get("Logo") != null)
            {

                if (ModelState.IsValid)
                {
                    Options option = context.Options.FirstOrDefault();

                    option.Lang = model.Lang;
                    option.Title = model.Title;
                    option.HeaderText = model.HeaderText;

                    context.SaveChanges();
                }

                HttpPostedFileBase logo = Request.Files.Get("Logo");

                if (logo != null && logo.ContentLength > 0)
                {
                    string ext = Path.GetExtension(logo.FileName).ToUpper();

                    if (ext == ".JPG" || ext == ".PNG" || ext == ".JPEG")
                    {
                        MemoryStream ms = new MemoryStream();
                        logo.InputStream.CopyTo(ms);

                        context.Options.FirstOrDefault(i => i.ID == model.ID).Logo = ms.ToArray();

                        context.SaveChanges();
                    }

                }
                return View("Index");

            }
            return View(model);
        }
    }

}