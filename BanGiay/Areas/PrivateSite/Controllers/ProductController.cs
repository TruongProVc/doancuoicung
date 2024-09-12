using BanGiay.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanGiay.Areas.PrivateSite.Controllers
{
    public class ProductController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        static bool checkEdit = false;
        static string idProductEdit = "";
        static Random random = new Random(); // Random instance

        public ActionResult Index()
        {
                ViewData["BrandList"] = db.ThHieux.ToList();
            ViewData["CateList"] = db.DMucs.ToList();
            var products = db.SanPhams.ToList();
            if (products == null)
            {
                // Log error or handle the case where products are null
                products = new List<SanPham>(); // Ensure the view receives a non-null list
            }
            return View(products);
        }


        [HttpGet]
        public JsonResult LoadData(string keyword, int? page, int? pageSize)
        {
            var size = pageSize ?? 10; // Adjust page size as needed
            var pageIndex = page ?? 1;

            try
            {
                var products = db.SanPhams
                    .Where(p => string.IsNullOrEmpty(keyword) || p.tenSanPham.ToLower().Contains(keyword.ToLower()))
                    .Select(d => new
                    {
                        d.maSanPham,
                        d.tenSanPham,
                        d.giaTien,
                        d.soLuong,
                        d.trangthai,
                        d.hinhSanPham,
                        d.maTH,
                        d.maDM,
                        d.moTa
                    })
                    .ToList();

                var totalPage = products.Count;
                var numberPage = Math.Ceiling((float)totalPage / size);

                var start = (pageIndex - 1) * size;
                products = products.Skip(start).Take(size).ToList();

                return Json(new
                {
                    status = true,
                    Data = products,
                    CurrentPage = pageIndex,
                    TotalItem = totalPage,
                    NumberPage = numberPage,
                    PageSize = size,
                    message = "Đang load"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    status = false,
                    CurrentPage = pageIndex,
                    TotalItem = 0,
                    NumberPage = 0,
                    PageSize = size,
                    message = "Tải dữ liệu thất bại"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Insert(ProductVM mVM, HttpPostedFileBase hinh)
        {
            if (ModelState.IsValid)
            {
                if (checkEdit)
                {
                    // Ensure idProductEdit is correctly handled as an integer
                    if (int.TryParse(idProductEdit, out int productId))
                    {
                        var product = db.SanPhams.Find(productId);
                        if (product == null)
                        {
                            return Json(new { status = false, message = "Sản phẩm không tồn tại" });
                        }

                        product.tenSanPham = mVM.name;
                        product.moTa = mVM.description;
                        product.maDM = mVM.category;
                        product.maTH = mVM.brand;
                        product.soLuong = mVM.quality;
                        product.giaTien = mVM.price;
                        product.trangthai = mVM.status;

                        if (hinh != null)
                        {
                            SaveImage(mVM.imageProduct, product);
                        }

                        checkEdit = false;
                        idProductEdit = "";
                        db.SaveChanges();
                        return Json(new { status = true, message = "Sửa thành công" });
                    }
                    else
                    {
                        return Json(new { status = false, message = "ID sản phẩm không hợp lệ" });
                    }
                }
                else
                {
                    SanPham product = new SanPham
                    {
                        tenSanPham = mVM.name,
                        maTH = mVM.brand,
                        maDM = mVM.category,
                        moTa = mVM.description,
                        giaTien = mVM.price,
                        soLuong = mVM.quality,
                        trangthai = mVM.status
                        ,
                      
                    };

                    if (hinh != null)
                    {
                        SaveImage(hinh, product);
                    }

                    db.SanPhams.Add(product);
                    db.SaveChanges();
                    return Json(new { status = true, message = "Thêm thành công" });
                }
            }

            return Json(new { status = false, message = "Dữ liệu không hợp lệ" });
        }

        [HttpGet]
        public JsonResult Edit(string id)
        {
            idProductEdit = "";
            if (int.TryParse(id, out int productId))
            {
                var product = db.SanPhams.Select(d => new
                {
                    d.maSanPham,
                    d.tenSanPham,
                    d.hinhSanPham,
                    d.moTa,
                    d.giaTien,
                    d.soLuong,
                    d.trangthai,
                 
                }).FirstOrDefault(d => d.maSanPham == productId);

                if (product != null)
                {
                    idProductEdit = product.maSanPham.ToString();
                    checkEdit = true;
                    return Json(new { status = true, data = product }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Delete(string id)
        {
            if (int.TryParse(id, out int productId))
            {
                var product = db.SanPhams.Find(productId);
                if (product != null)
                {
                    db.SanPhams.Remove(product);
                    db.SaveChanges();
                    return Json(new { status = true, message = "Xóa thành công" });
                }
                return Json(new { status = false, message = "Sản phẩm không tồn tại" });
            }
            return Json(new { status = false, message = "Xóa thất bại, ID không hợp lệ" });
        }




        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            var product = db.SanPhams.Find(id);
            if (product != null)
            {
                product.trangthai = !product.trangthai;
                db.SaveChanges();
                return Json(new { status = true, message = "Thay đổi trạng thái thành công" });
            }
            return Json(new { status = false, message = "Thay đổi trạng thái thất bại" });
        }



        private void SaveImage(HttpPostedFileBase image, SanPham sp)
        {
            if (image != null && image.ContentLength > 0)
            {
                string virtualPath = "/Asset/assets/images/product/";
                string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string physicalPath = Server.MapPath("~" + virtualPath);

                Directory.CreateDirectory(physicalPath);

                image.SaveAs(Path.Combine(physicalPath, fileName));
                sp.hinhSanPham = virtualPath + fileName;
            }
            else
            {
                sp.hinhSanPham = "";
            }
        }

        // Method to generate a random integer ID
        private int GenerateRandomID()
        {
            Random random = new Random();
            int id = random.Next(100000000, 999999999); // 9-digit random number
            return id;
        }
    }
}