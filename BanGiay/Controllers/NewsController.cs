using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Controllers
{
    public class NewsController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        // GET: News
        public ActionResult Index(string idBaiViet)
        {
            var baiViet = db.BaiViets.FirstOrDefault(bv => bv.idBaiViet == idBaiViet);

            if (baiViet == null)
            {
                return HttpNotFound();
            }

            return View(baiViet);
        }
    }
}