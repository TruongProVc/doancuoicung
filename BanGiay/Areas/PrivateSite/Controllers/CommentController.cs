using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Areas.PrivateSite.Controllers
{
    [CustomAuthentication]
    [CustomAuthorize(Roles = "quản trị, nhân viên, quản lý")]
    public class CommentController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult LoadData(string keyword, int? page, int? pageSize)    
        {
            var size = pageSize ?? 2;
            var pageIndex = page ?? 1;
            try
            {
                var comment = db.BinhLuans.Where(ac => string.IsNullOrEmpty(keyword) || ac.idTaiKhoan.ToLower().Contains(keyword.ToLower())).Select(m => new {
                    m.idSanPham,
                    m.idTaiKhoan,
                    emailCM = m.idTaiKhoan,
                    //nameMovie = m.Phim.tenPhim,
                    m.noiDung,
                    m.ngayDang,
                    m.sttBinhLuan
                }).ToList();

                var totalPage = comment.Count;
                var numberPage = Math.Ceiling((float)totalPage / size);

                var start = (pageIndex - 1) * size;
                comment = comment.Skip(start).Take(size).ToList();

                return Json(new { status = true, Data = comment, CurrentPage = pageIndex, TotalItem = totalPage, NumberPage = numberPage, PageSize = size, message = "Đang load" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false, CurrentPage = pageIndex, TotalItem = 0, NumberPage = 0, PageSize = size, message = "Tải dữ liệu thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            BinhLuan cm = db.BinhLuans.Find(id);
            if (cm != null)
            {
                db.BinhLuans.Remove(cm);
                db.SaveChanges();
                return Json(new { status = true, message = "Xóa thành công" });
            }
            return Json(new { status = true, message = "Xóa thất bại" });
        }
        [HttpGet]
        public ActionResult Detail(int id)
        {
            BinhLuan cm = db.BinhLuans.Find(id);
            if (cm != null)
            {
                return View(cm);
            }
            return View();
        }
    }
}