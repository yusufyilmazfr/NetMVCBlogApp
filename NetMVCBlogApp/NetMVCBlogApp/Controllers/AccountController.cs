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
            if (Session["admin"] == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult Login()
        {
            if (Session["admin"] != null)
            {
                return RedirectToAction("Index");
            }
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

    }
}