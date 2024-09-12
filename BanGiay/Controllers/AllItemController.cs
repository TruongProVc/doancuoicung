using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
using PagedList;
namespace BanGiay.Controllers
{
    public class AllItemController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();

        public ActionResult Index(int? page, int? maThuongHieu)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;

            var products = db.SanPhams.AsQueryable();

            if (maThuongHieu.HasValue)
            {
                products = products.Where(p => p.maTH == maThuongHieu.Value);
                ViewBag.maThuongHieu = maThuongHieu.Value;
            }

            var model = products.OrderBy(p => p.tenSanPham).ToPagedList(pageNumber, pageSize);

            ViewBag.ThHieu = db.ThHieux.ToList();

            ViewBag.Sizes = db.SizeGiays.ToList();

            return View(model);
        }

        //load lại sản phẩm khi dùng ajax
        [HttpGet]
        public ActionResult LoadProducts(int? page, int? maThuongHieu)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;

            var products = db.SanPhams.AsQueryable();

            if (maThuongHieu.HasValue)
            {
                products = products.Where(p => p.maTH == maThuongHieu.Value);
            }

            var sp = products.OrderBy(p => p.tenSanPham).ToPagedList(pageNumber, pageSize);

            if (!sp.Any())
            {
                return PartialView("_NoProductsPartial");
            }
            else
            {
                return PartialView("_ProductListPartial", sp);
            }
        }
        // Lọc giá tiền 
        public ActionResult LoadProductsByPriceRange(int minPrice, int maxPrice, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;

            var filteredProducts = db.SanPhams
                .Where(sp => sp.giaTien >= minPrice && sp.giaTien <= maxPrice)
                .OrderBy(sp => sp.tenSanPham)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_ProductListPartial", filteredProducts);
        }
        //Lọc theo size
        public ActionResult FilterBySize(int size, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 9;

            var products = from spSize in db.SanPhamSizes
                           join sizeGiay in db.SizeGiays on spSize.maSize equals sizeGiay.maSize
                           where sizeGiay.size == size
                           select spSize.maSanPham;

            var filteredProducts = db.SanPhams
                                     .Where(p => products.Contains(p.maSanPham))
                                     .OrderBy(p => p.tenSanPham)
                                     .ToPagedList(pageNumber, pageSize);
            if (!filteredProducts.Any() && !products.Any())
            {
                return PartialView("_NoProductsPartial");
            }

            return PartialView("_ProductListPartial", filteredProducts);
        }

    }

}
