using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
namespace BanGiay.Controllers

{
    public class UserProfileController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Index(TaiKhoan acc)
        {
            TaiKhoan account = db.TaiKhoans.FirstOrDefault(d => d.idTaiKhoan.Equals(acc.idTaiKhoan) && d.email.Equals(acc.email) && acc.soDienThoai.Equals(d.soDienThoai));
            if (account != null)
            {
                account.ho = acc.ho;
                account.ten = acc.ten;
                db.SaveChanges();
                Session["InformationAccount"] = account;
                return Json(new { status = true, message = "Thay đổi thông tin thành công" });
            }
            return Json(new { status = false, message = "Thay đổi thông tin thất bại" });
        }


        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string id, string currentPasswd, string newPasswd, string comfirmNewPw)
        {

            TaiKhoan account = Session["InformationAccount"] as TaiKhoan;
            if (newPasswd.Equals(comfirmNewPw))
            {
                string hp = HashPassword.SHA512HashPass(currentPasswd);
                TaiKhoan acc = db.TaiKhoans.FirstOrDefault(d => d.idTaiKhoan.Equals(id) && d.matKhau.Equals(hp));
                if (acc != null)
                {
                    acc.matKhau = HashPassword.SHA512HashPass(newPasswd);
                    db.SaveChanges();
                    ViewBag.displayNote = "ok";//Khi đổi mật khẩu thành công
                    return View();
                }
            }
            else
            {
                ViewBag.displayNote = "er";// Đổi mk thấtbaij
                return View();
            }
            ViewBag.displayNote = "";
            return View();
        }
    }
}