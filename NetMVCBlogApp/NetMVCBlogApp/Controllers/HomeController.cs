using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;

namespace NetMVCBlogApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            BlogDBEntities context = new BlogDBEntities();


            return View(context.Post.ToList());
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult AboutMe()
        {
            return View();
        }
    }
}