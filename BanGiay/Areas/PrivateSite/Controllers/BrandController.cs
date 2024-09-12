using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BanGiay.Models;
using PagedList;
namespace BanGiay.Areas.PrivateSite.Controllers
{
    public class BrandController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        static bool checkEdit = false;

        // GET: PrivateSite/Brand
        public ActionResult Index()
        {
            return View();
        }

        // Load brands and search by maTH
        [HttpGet]
        public JsonResult LoadData(int? keyword, int? page, int? pageSize)
        {
            var size = pageSize ?? 2;
            var pageIndex = page ?? 1;

            try
            {
                // Search by maTH if keyword is provided
                var brands = db.ThHieux
                    .Where(p => keyword == null || p.maTH == keyword)
                    .Select(d => new
                    {
                        d.maTH,
                        d.tenTH,
                        d.moTa
                    })
                    .ToList();

                // Pagination
                var totalPage = brands.Count;
                var numberPage = Math.Ceiling((float)totalPage / size);

                var start = (pageIndex - 1) * size;
                brands = brands.Skip(start).Take(size).ToList();

                return Json(new { status = true, Data = brands, CurrentPage = pageIndex, TotalItem = totalPage, NumberPage = numberPage, PageSize = size, message = "Đang load" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false, CurrentPage = pageIndex, TotalItem = 0, NumberPage = 0, PageSize = size, message = "Tải dữ liệu lên thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        // Add or edit brand
        [HttpPost]
        public JsonResult SaveBrand(ThHieu ThHieu)
        {
            if (!string.IsNullOrEmpty(ThHieu.tenTH) && ThHieu.maTH != 0)
            {
                try
                {
                    if (checkEdit)
                    {
                        // Update brand if editing
                        var existingBrand = db.ThHieux.Find(ThHieu.maTH);
                        if (existingBrand != null)
                        {
                            existingBrand.tenTH = ThHieu.tenTH;
                            existingBrand.moTa = ThHieu.moTa;
                            db.SaveChanges();
                            checkEdit = false;
                            return Json(new { status = true, message = "Đã sửa thành công" });
                        }
                        return Json(new { status = false, message = "Không tìm thấy thương hiệu để sửa" });
                    }
                    else
                    {
                        // Add new brand
                        db.ThHieux.Add(ThHieu);
                        db.SaveChanges();
                        return Json(new { status = true, message = "Đã thêm thương hiệu thành công" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Xảy ra lỗi: " + ex.Message });
                }
            }
            return Json(new { status = false, message = "Vui lòng nhập đầy đủ thông tin" });
        }


        // Get brand by maTH for editing
        [HttpGet]
        public JsonResult Edit(int id)
        {
            try
            {
                var brand = db.ThHieux
                              .Where(m => m.maTH == id)
                              .Select(d => new { d.tenTH, d.maTH, d.moTa })
                              .FirstOrDefault();

                if (brand != null)
                {
                    checkEdit = true;
                    return Json(new { status = true, data = brand }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = false, message = "Không tìm thấy thương hiệu" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Xảy ra lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        // Delete brand by maTH
        [HttpPost]
        public JsonResult Delete(int id)
        {
            ThHieu brand = db.ThHieux.FirstOrDefault(m => m.maTH == id);
            if (brand != null)
            {
                string name = brand.tenTH;
                db.ThHieux.Remove(brand);
                db.SaveChanges();
                return Json(new { status = true, message = "Đã xóa thành công thương hiệu: " + name });
            }
            return Json(new { status = false, message = "Không tìm thấy thương hiệu để xóa" });
        }

    }
}
