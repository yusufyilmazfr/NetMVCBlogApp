using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class OptionModel
    {
        public int ID { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Lang { get; set; }
        public string HeaderText { get; set; }

    }
}