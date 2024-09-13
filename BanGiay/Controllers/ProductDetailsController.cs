using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Controllers
{
    public class ProductDetailsController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();

        // GET: ProductDetail
        public ActionResult Index(string idSanPham)
        {
            if (!string.IsNullOrEmpty(idSanPham))
            {
                if (int.TryParse(idSanPham, out int idSanPhamInt))
                {
                    var objmoviedetail = db.SanPhams.FirstOrDefault(n => n.maSanPham == idSanPhamInt);

                    if (objmoviedetail == null)
                    {
                        return HttpNotFound();
                    }
                    Size(idSanPham);
                    UpdateSite();
                    return View(objmoviedetail);
                }
                else
                {   
                    return HttpNotFound();
                }
            }
            return HttpNotFound();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //[CustomAuthentication]
        public JsonResult Create(string noiDung1, string idSanPham)
        {
            if (Session["InformationAccount"] != null)
            {
                var account = (Session["InformationAccount"] as TaiKhoan).idTaiKhoan;
                BinhLuan comment = new BinhLuan()
                {
                    idTaiKhoan = account,
                    idSanPham = idSanPham,
                    noiDung = noiDung1,
                    ngayDang = DateTime.Now,
                };
                db.BinhLuans.Add(comment);
                db.SaveChanges();

                // Retrieve the updated list of comments for the product
                var comments = db.BinhLuans
                    .Where(bl => bl.idSanPham == idSanPham)
                    .OrderBy(bl => bl.ngayDang) // Optional: Order comments by date
                    .ToList(); // Fetch data into memory

                // Transform data into the required format
                var formattedComments = comments.Select(c => new
                {
                    idTaiKhoan = c.idTaiKhoan,
                    noiDung = c.noiDung,
                    ngayDang = c.ngayDang.HasValue ? c.ngayDang.Value.ToString("dd/MM/yyyy HH:mm:ss") : "Chưa xác định"
                }).ToList();

                return Json(new { success = true, comments = formattedComments });
            }

            return Json(new { success = false });
        }



        private void UpdateSite()
        {
            List<BinhLuan> bl = db.BinhLuans.ToList();

            ViewData["dsbl"] = bl;
        }


        private void Size(string idSanPham)
        {
            if (int.TryParse(idSanPham, out int idSanPhamInt))
            {
                var sizesForProduct = db.SanPhamSizes
                                        .Where(sps => sps.maSanPham == idSanPhamInt)
                                        .Select(sps => sps.maSize)
                                        .ToList();

                List<SizeGiay> sz = db.SizeGiays
                                      .Where(s => sizesForProduct.Contains(s.maSize))
                                      .ToList();
                ViewData["dssz"] = sz;
            }
            else
            {
                ViewData["dssz"] = new List<SizeGiay>();
            }
        }
    }
}
