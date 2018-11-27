using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetMVCBlogApp.Entity;

namespace NetMVCBlogApp.Controllers
{
    public class SubscriberController : Controller
    {
        BlogDBEntities context = new BlogDBEntities();

        // GET: Subscriber
        public string Add(string email)
        {
            if (email !=null && email != "")
            {
                context.Subscriber.Add(new Subscriber() { Mail = email });
                context.SaveChanges();
                return "yes";
            }
            return "no";
        }
    }
}