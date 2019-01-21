using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class CreateFriendlyUrl
    {

        private string RemoveAccent(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public string GenerateSlug(string Url)
        {
            string phrase = string.Format("{0}", Url);

            string str = RemoveAccent(phrase).ToLower();
            //invalid chars 
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            //convert multiple into one space 
            str = Regex.Replace(str, @"\s+", " ").Trim();
            //cut and trim
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            return str;

        }
    }
}