using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanGiay.Models
{
    public class Cart
    {
        public string maSP { get; set; }
        public string tenSP { get; set; }
        public int giaSP { get; set; }
        public int tongGia { get; set; }
        public int soLuong { get; set; }
        public int size { get; set; }
    }
}