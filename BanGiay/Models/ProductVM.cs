using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanGiay.Models
{
    public class ProductVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public int category { get; set; }
        public int brand { get; set; }
        public double price { get; set; }
        public string description { get; set; }
        public HttpPostedFileBase imageProduct { get; set; }  // For uploading an image
        public int quality { get; set; }
        public bool status { get; set; }  // Assuming this is a bit field in your database
    }
}