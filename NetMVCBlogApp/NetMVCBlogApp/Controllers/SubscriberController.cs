using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                context.Subscriber.Add(new Subscriber() { Mail = email });
                context.SaveChanges();
                return "yes";
            }
            else
            {
                return "no";
            }

        }
    }
}