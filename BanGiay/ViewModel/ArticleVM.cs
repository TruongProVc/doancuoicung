using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanGiay.ViewModel
{
    public class ArticleVM
    {
        public string id { set; get; }
        [Required]
        public string articleName { set; get; }
        [Required]
        public string summaryContent { set; get; }
        [Required]
        [AllowHtml]
        public string content { set; get; }
        [Required]
        public HttpPostedFileBase avatarArticle { set; get; }
    }
}