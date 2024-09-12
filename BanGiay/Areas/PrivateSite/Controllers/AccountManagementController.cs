using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BanGiay.Models;

namespace BanGiay.Areas.PrivateSite.Controllers
{
    [CustomAuthentication]
    [CustomAuthorize(Roles = "quản trị")]
    public class AccountManagementController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        static string idAccountLoadIP = "";

        [HttpGet]
        public ActionResult ListOfAccount()
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
                var accounts = db.TaiKhoans
                    .Where(ac => string.IsNullOrEmpty(keyword) || ac.email.ToLower().Contains(keyword.ToLower()))
                    .Select(m => new
                    {
                        m.idTaiKhoan,
                        m.ho,
                        m.ten,
                        m.email,
                        m.sttTrangThai,
                        m.maNhom
                    }).ToList();

                var totalPage = accounts.Count;
                var numberPage = Math.Ceiling((float)totalPage / size);

                var start = (pageIndex - 1) * size;
                accounts = accounts.Skip(start).Take(size).ToList();

                return Json(new
                {
                    status = true,
                    Data = accounts,
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
        public JsonResult Delete(string idAccount)
        {
            TaiKhoan account = db.TaiKhoans.Find(idAccount);
            if (account == null) return Json(new { status = false, message = "Tài khoản không tồn tại" });

            string userName = account.email;    

            // Xóa các bản ghi liên quan
            var activateCodes = db.MaKichHoatTaiKhoans.Where(m => m.idTaiKhoan.Equals(idAccount)).ToList();

            db.MaKichHoatTaiKhoans.RemoveRange(activateCodes);
            db.TaiKhoans.Remove(account);
            db.SaveChanges();   

            return Json(new { status = true, message = "Đã xóa thành công tài khoản: " + userName });
        }

        [HttpPost]
        public JsonResult AccountStatus(string idAccount, int newStatus)
        {
            TaiKhoan account = db.TaiKhoans.Find(idAccount);
            if (account == null) return Json(new { status = false, message = "Tài khoản không tồn tại" });

            account.sttTrangThai = newStatus;
            db.SaveChanges();

            return Json(new { status = true, message = $"Bạn đã thay đổi trạng thái tài khoản {account.email} thành công" });
        }

        [HttpGet]
        public ActionResult DetailAccount(string id)
        {
            idAccountLoadIP = "";
            TaiKhoan account = db.TaiKhoans.FirstOrDefault(d => d.idTaiKhoan.Equals(id));
            if (account != null)
            {
                idAccountLoadIP = account.idTaiKhoan;
                return View(account);
            }
            return View("ListOfAccount");
        }

       

        [HttpPost]
        public async Task<JsonResult> ResetPassword(string id)
        {
            TaiKhoan account = db.TaiKhoans.Find(id);
            if (account != null)
            {
                string newPasswd = Common.RandomCharacter(10);
                account.matKhau = HashPassword.SHA512HashPass(newPasswd);
                bool check = await Common.Sendmail("TitansCinema", "Mật khẩu mới ", newPasswd, account.email);
                if (check)
                {
                    db.SaveChanges();
                    return Json(new { status = true, message = "Reset mật khẩu thành công" });
                }
            }
            return Json(new { status = false, message = "Reset mật khẩu thất bại" });
        }

        [HttpPost]
        public JsonResult Detail()
        {
            return Json(new { status = true });
        }

        [HttpPost]
        public JsonResult ChangePermission(string id, int newPermission)
        {
            TaiKhoan account = db.TaiKhoans.Find(id);
            if (account != null && newPermission != 1)
            {
                account.maNhom = newPermission;
                db.SaveChanges();
                return Json(new { status = true, message = "Thay đổi thành công" });
            }
            return Json(new { status = false, message = "Thay đổi thất bại" });
        }
    }
}
