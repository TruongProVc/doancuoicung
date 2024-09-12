using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Controllers
{
    public class ThanhToanController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();

        // GET: ThanhToan
        public ActionResult Index()
        {
            List<Cart> cartItems = Session["GioHang"] as List<Cart> ?? new List<Cart>();

            // Calculate the total number of products in the cart
            ViewBag.CartItemCount = cartItems.Sum(item => item.soLuong);

            return View();
        }



        // Thêm sản phẩm vào giỏ hàng
        public ActionResult addItem(string maSP)
        {
            int maSanPhamInt = int.Parse(maSP);
            SanPham sp = db.SanPhams.FirstOrDefault(n => n.maSanPham == maSanPhamInt);
            List<Cart> gh = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Cart cc = gh.FirstOrDefault(n => n.maSP.Equals(maSP));

            if (cc == null)
            {
                Cart cc1 = new Cart
                {
                    maSP = maSP,
                    tenSP = sp.tenSanPham,
                    giaSP = (int)sp.giaTien,
                    tongGia = (int)sp.giaTien,
                    soLuong = 1,
                };
                gh.Add(cc1);
            }
            else
            {
                cc.soLuong += 1;
                cc.tongGia = (int)(sp.giaTien * cc.soLuong);
            }
            Session["GioHang"] = gh;
            return RedirectToAction("Index", "Home");
        }

        // Tăng số lượng sản phẩm trong giỏ hàng
        public ActionResult UpdateItem(string maSP, int? sl)
        {
            if (sl == null || sl <= 0)
            {
                return Json(new { success = false, message = "Vui lòng nhập 1 con số nhất định." }, JsonRequestBehavior.AllowGet);
            }

            List<Cart> cart = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Cart item = cart.FirstOrDefault(m => m.maSP.Equals(maSP));
            if (item != null)
            {
                item.soLuong = sl.Value;
                item.tongGia = item.giaSP * item.soLuong;
            }

            // Recalculate the total cart amount
            decimal updatedCartTotal = cart.Sum(m => m.tongGia);

            Session["GioHang"] = cart;

            // Return JSON with the updated information
            return Json(new
            {
                success = true,
                updatedItemTotal = string.Format("{0:0,0 vnđ}", item.tongGia),
                updatedCartTotal = string.Format("{0:0,0 vnđ}", updatedCartTotal)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveItem(string maSP)
        {
            List<Cart> cart = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Cart item = cart.FirstOrDefault(m => m.maSP.Equals(maSP));

            if (item != null)
            {
                cart.Remove(item);
                Session["GioHang"] = cart;

                // Calculate the updated cart total
                decimal updatedCartTotal = cart.Sum(m => m.tongGia);

                return Json(new { success = true, updatedCartTotal = string.Format("{0:0,0 vnđ}", updatedCartTotal) });
            }

            return Json(new { success = false, message = "Item not found in cart." });
        }
        [HttpGet]
        public ActionResult Payment()
        {
            KhachHang x = new KhachHang();
            List<Cart> gh = Session["GioHang"] as List<Cart> ?? new List<Cart>();
            Session["GioHang"] = gh;
            return View(x);
        }

        [HttpPost]
        public ActionResult Payment(KhachHang x)
        {
            if (x != null)
            {
                try
                {
                    // Tạo mã khách hàng và đơn hàng
                    int maKhachHang;
                    int maDonHang;

                    if (!int.TryParse(DateTime.Now.ToString("yyMMddHHmmss"), out maKhachHang))
                    {
                        throw new Exception("Không thể tạo mã khách hàng.");
                    }
                    x.maKH = maKhachHang;

                    // Thêm khách hàng vào cơ sở dữ liệu
                    db.KhachHangs.Add(x);
                    db.SaveChanges();

                    // Tạo đơn hàng
                    if (!int.TryParse(DateTime.Now.ToString("yyMMddHHmmss"), out maDonHang))
                    {
                        throw new Exception("Không thể tạo mã đơn hàng.");
                    }

                    DonDatHang d = new DonDatHang
                    {
                        maDonHang = maDonHang,
                        idTaiKhoan = Session["InformationAccount"]?.ToString() ?? "minh",
                        ngayDatHang = DateTime.Now,
                        diaChiGiaoHang = x.diaChi
                    };

                    db.DonDatHangs.Add(d);

                    // Thêm chi tiết đơn hàng
                    foreach (var i in Session["GioHang"] as List<Cart>)
                    {
                        ChiTietDonHang ct = new ChiTietDonHang
                        {
                            maSanPham = int.TryParse(i.maSP, out int maSanPham) ? maSanPham : 0,
                            giaTien = i.giaSP,
                            soLuong = i.soLuong,
                            Size = i.size,
                            maDonHang = d.maDonHang // Liên kết với đơn hàng vừa tạo
                        };
                        db.ChiTietDonHangs.Add(ct);
                    }

                    db.SaveChanges();

                    // Xóa giỏ hàng sau khi hoàn tất thanh toán
                    Session["GioHang"] = new List<Cart>();

                    // Chuyển hướng đến trang cảm ơn hoặc xác nhận đơn hàng
                    return RedirectToAction("ThankYou");
                }
                catch (Exception ex)
                {
                    // Ghi lại lỗi nếu cần thiết và chuyển hướng đến trang lỗi
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
            }

            return View("Error");
        }

        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}