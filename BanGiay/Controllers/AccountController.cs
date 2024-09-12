using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;
using BanGiay.ViewModel;

namespace BanGiay.Controllers
{
    public class AccountController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["InformationAccount"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        /// <summary>
        /// Hàm này dùng để đăng nhập tài khoản
        /// </summary>
        /// <param name="email">Nhận tên tài khoản từ người dùng khi nhập vào</param>
        /// <param name="password">Mật khẩu của tài khoản</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            ViewBag.displayError = "";
            string passhash = HashPassword.SHA512HashPass(password);
            TaiKhoan accountLogin = db.TaiKhoans.Where(m => m.email.Equals(email.ToLower().Trim()) && m.matKhau.Equals(passhash)).FirstOrDefault();
            if (accountLogin != null)
            {
                if (accountLogin.sttTrangThai == 3)
                {
                    ViewBag.displayError = "Tài khoản của bạn có thể đã bị khóa vui lòng liên hệ Admin";
                    return View();
                }
                else
                {
                    Session["InformationAccount"] = accountLogin;

                    if (accountLogin.sttTrangThai == 2)
                    {
                        return RedirectToAction("ActivateAccount", "Account");
                    }
                    if (accountLogin.maNhom == 4)
                        return RedirectToAction("Index", "Home");
                    else
                        return RedirectToAction("Index", "Dashboard", new { area = "PrivateSite" });
                }
            }
            ViewBag.displayError = "Thông tin không chính xác";
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["InformationAccount"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        /// <summary>
        /// Hàm này dùng để đăng ký tài khoản
        /// </summary>
        /// <param name="acc">Nhận các thông tin sau khi người dùng nhập vào form đăng ký tài khoản</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterVM acc)
        {
            bool checkPhoneAndEmail = (db.TaiKhoans.Where(m => m.email.Equals(acc.email.ToLower().Trim()) || m.soDienThoai.Equals(acc.numberPhone)).Count() > 0); //Kiểm tra số điện thoại và email đã được đăng ký trong hệ thống chưa
            if (ModelState.IsValid && !checkPhoneAndEmail)
            {
                Random random = new Random();
                TaiKhoan registerAcc = new TaiKhoan()
                {
                    idTaiKhoan = random.Next(20000).ToString(),
                    email = acc.email.Trim().ToLower(),
                    soDienThoai = acc.numberPhone,
                    ho = acc.lastName,
                    ten = acc.firstName,
                    sttTrangThai = 1, // Đặt trạng thái tài khoản thành 1 (đã kích hoạt)
                    matKhau = HashPassword.SHA512HashPass(acc.passWord),
                    maNhom = 4,
                };
                db.TaiKhoans.Add(registerAcc);
                Session["InformationAccount"] = registerAcc; // Lưu trữ phiên sau khi đăng ký

                db.SaveChanges();
                return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ sau khi đăng ký thành công
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session["InformationAccount"] = null;
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult ActivateAccount()
        {
            if (Session["InformationAccount"] == null)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        /// <summary>
        /// Hàm này dùng để lấy lại mật khẩu
        /// </summary>
        /// <param name="email">Email hàm cần gửi mã xác nhận lấy lại tài khoản</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            Random random = new Random();
            TaiKhoan account = db.TaiKhoans.FirstOrDefault(d => d.sttTrangThai != 4 && d.email.Equals(email.ToLower().Trim()));

            if (account != null)
            {
                // Tạo mật khẩu mới ngẫu nhiên
                string newPassword = Common.RandomCharacter(10);
                account.matKhau = HashPassword.SHA512HashPass(newPassword);

                db.SaveChanges();

                // Gửi mật khẩu mới qua email
                bool check = await Common.Sendmail("H&CS", "Mật khẩu mới", newPassword, account.email);
                if (check)
                {
                    ViewBag.displayE = "Mật khẩu mới đã được gửi đến email của bạn.";
                    return View();
                }
                ViewBag.displayE = "Gửi mật khẩu mới thất bại.";
                return View();
            }
            ViewBag.displayE = "Không có tài khoản này.";
            return View();
        }

        [HttpGet]
        public ActionResult NewPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewPassword(string email)
        {

            return View();
        }
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DenielAccess()
        {
            return View();
        }


    }
}