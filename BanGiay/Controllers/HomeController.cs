using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        static ShoesShopEntities db = new ShoesShopEntities();

        // GET: Home
        public ActionResult Index()
        {
            ViewData["dssp"] = db.SanPhams.ToList();
            return View();
        }

        [HttpGet]
        public JsonResult AutoComplete(string keyword)
        {
            try
            {
                // Kiểm tra từ khóa có hợp lệ không
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return Json(new { status = false, message = "Keyword cannot be empty" }, JsonRequestBehavior.AllowGet);
                }

                // Tìm kiếm sản phẩm có tên chứa từ khóa
                var sp = db.SanPhams
                           .Where(c => c.tenSanPham != null && c.tenSanPham.Contains(keyword))
                           .Select(d => new
                           {
                               d.tenSanPham,
                               d.maSanPham,
                               d.hinhSanPham,
                               d.giaTien
                           })
                           .ToList();

                // Kiểm tra nếu có sản phẩm
                if (sp.Any())
                {
                    return Json(new { status = true, data = sp, count = sp.Count }, JsonRequestBehavior.AllowGet);
                }

                // Trả về nếu không có kết quả
                return Json(new { status = false, message = "No products found" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                // Log.Error("Error in AutoComplete", ex);

                // Trả về thông tin lỗi chi tiết hơn
                return Json(new { status = false, message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Inde1x()
        {
            return View();
        }

    }
}