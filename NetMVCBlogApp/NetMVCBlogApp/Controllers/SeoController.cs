using NetMVCBlogApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace NetMVCBlogApp.Controllers
{
    public class SeoController : Controller
    {
        public BlogDBEntities context { get; set; }

        public SeoController()
        {
            context = new BlogDBEntities();
        }

        [Route("sitemap.xml")]
        public ActionResult SitemapXml()
        {
            string siteUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            Response.Clear();
            Response.ContentType = "text/xml";

            XmlTextWriter xmlTextWriter = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("urlset");
            xmlTextWriter.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xmlTextWriter.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlTextWriter.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");



            xmlTextWriter.WriteStartElement("url");
            xmlTextWriter.WriteElementString("loc", siteUrl + Url.Action("Index", "Home"));
            xmlTextWriter.WriteElementString("lastmod", DateTime.Now.ToString("yyyy-MM-dd"));
            xmlTextWriter.WriteElementString("priority", "0.5");
            xmlTextWriter.WriteElementString("changefreq", "daily");
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("url");
            xmlTextWriter.WriteElementString("loc", siteUrl + Url.Action("AboutMe", "Home"));
            xmlTextWriter.WriteElementString("lastmod", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"));
            xmlTextWriter.WriteElementString("priority", "0.5");
            xmlTextWriter.WriteElementString("changefreq", "weekly");
            xmlTextWriter.WriteEndElement();

            xmlTextWriter.WriteStartElement("url");
            xmlTextWriter.WriteElementString("loc", siteUrl + Url.Action("Contact", "Home"));
            xmlTextWriter.WriteElementString("lastmod", DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"));
            xmlTextWriter.WriteElementString("priority", "0.5");
            xmlTextWriter.WriteElementString("changefreq", "weekly");
            xmlTextWriter.WriteEndElement();



            foreach (var post in context.Post.Where(i => i.isValid).ToList())
            {
                xmlTextWriter.WriteStartElement("url");
                xmlTextWriter.WriteElementString("loc", siteUrl + "/" + post.SeoLink + "-" + post.ID);
                xmlTextWriter.WriteElementString("lastmod", post.AddedDate.ToString("yyyy-MM-dd"));
                xmlTextWriter.WriteElementString("priority", "0.5");
                xmlTextWriter.WriteElementString("changefreq", "monthly");
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndDocument();

            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            Response.End();

            return View();
        }

        [Route("RSS")]
        public ActionResult Rss()
        {
            string siteUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            Response.Clear();
            Response.ContentType = "text/xml";

            XmlTextWriter xmlTextWriter = new XmlTextWriter(Response.OutputStream, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("rss");
            xmlTextWriter.WriteAttributeString("version", "1.0");
            xmlTextWriter.WriteStartElement("channel");

            foreach (var post in context.Post.Where(i => i.isValid).ToList())
            {
                xmlTextWriter.WriteElementString("title", post.Name);
                xmlTextWriter.WriteElementString("link", siteUrl + "/" + post.SeoLink + "-" + post.ID);
                xmlTextWriter.WriteElementString("description", HttpUtility.HtmlDecode(post.Text));
                xmlTextWriter.WriteElementString("pubDate", post.AddedDate.ToString("dd.MM.yyyy"));
                xmlTextWriter.WriteElementString("language", "tr-TR");
            }

            xmlTextWriter.WriteEndDocument();

            xmlTextWriter.Flush();
            xmlTextWriter.Close();
            Response.End();

            return View();
        }

        [ChildActionOnly]
        public PartialViewResult MetaTags(string value)
        {
            Options options = context.Options.FirstOrDefault();

            if (value != null)
            {
                options.Description = value;
            }

            return PartialView(options);
        }
    }
}