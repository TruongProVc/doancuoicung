using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.ViewModel;
using BanGiay.Models;
namespace BanGiay.Areas.PrivateSite.Controllers
{
    //[CustomAuthentication]
    //[CustomAuthorize(Roles = "quản trị, quản lý, nhân viên")]
    public class ArticleController : Controller
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        static bool checkEdit = false;
        static string idArticleEdit = "";
        public ActionResult ArticleList()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Insert(ArticleVM aVM)
        {
            TaiKhoan account = Session["InformationAccount"] as TaiKhoan;
            if (aVM != null)
            {
                if (checkEdit)
                {
                    BaiViet article = db.BaiViets.Find(aVM.id);
                    checkEdit = false;
                    if (article != null)
                    {
                        article.tenBaiViet = aVM.articleName;
                        article.noiDungTomTat = aVM.summaryContent;
                        article.noiDung = aVM.content;
                        if (aVM.avatarArticle != null)
                        {
                            SaveImage(aVM.avatarArticle, article);
                        }
                        db.SaveChanges();
                        return Json(new { status = true, message = "Sửa thành công" });
                    }
                    return Json(new { status = false, message = "Lưu thất bại" });
                }
                else
                {
                    BaiViet article = new BaiViet()
                    {
                        idBaiViet = Common.CreateID(10),
                        tenBaiViet = aVM.articleName,
                        noiDungTomTat = aVM.summaryContent,
                        noiDung = aVM.content,
                        luotXem = 0,
                        ngayDang = DateTime.Now,
                        trangThai = true,
                        idTaiKhoanDang = account.idTaiKhoan
                    };
                    SaveImage(aVM.avatarArticle, article);

                    db.BaiViets.Add(article);
                    db.SaveChanges();

                    return Json(new { status = true, message = "Đã thêm bài viết thành công" });
                }
            }
            return Json(new { status = false, message = "Vui lòng điền đầy đủ các thông tin" });

        }
        [HttpGet]
        public JsonResult LoadData(string keyword, int? page, int? pageSize)
        {
            var size = pageSize ?? 2;
            var pageIndex = page ?? 1;
            try
            {
                var articles = db.BaiViets.Where(a => string.IsNullOrEmpty(keyword) || a.tenBaiViet.ToLower().Contains(keyword.ToLower())).Select(m => new {
                    m.idBaiViet,
                    m.tenBaiViet,
                    m.luotXem,
                    m.ngayDang,
                    taiKhoanDang = (db.TaiKhoans.Where(k => k.idTaiKhoan.Equals(m.idTaiKhoanDang)).Select(k => k.email)),
                    m.noiDung,
                    m.noiDungTomTat,
                    m.trangThai
                }).ToList();

                var totalPage = articles.Count;
                var numberPage = Math.Ceiling((float)totalPage / size);

                var start = (pageIndex - 1) * size;
                articles = articles.Skip(start).Take(size).ToList();

                return Json(new { status = true, Data = articles, CurrentPage = pageIndex, TotalItem = totalPage, NumberPage = numberPage, PageSize = size, message = "Đang load" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
            }
            return Json(new { status = true, CurrentPage = pageIndex, TotalItem = 0, NumberPage = 0, PageSize = size, message = "Đang load" }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Edit(string id)
        {
            checkEdit = false;
            idArticleEdit = "";
            var article = db.BaiViets.Select(d => new {
                d.idBaiViet,
                d.tenBaiViet,
                d.hinhDaiDien,
                d.luotXem,
                d.noiDung,
                d.noiDungTomTat
            }).FirstOrDefault(d => d.idBaiViet.Equals(id));
            if (article != null)
            {
                checkEdit = true;
                idArticleEdit = article.idBaiViet;
                return Json(new { status = true, data = article }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            BaiViet article = db.BaiViets.Find(id);
            if (article != null)
            {
                db.BaiViets.Remove(article);
                db.SaveChanges();
                return Json(new { status = true, message = "Bạn đã xóa thành công" });
            }
            return Json(new { status = false, message = "Xóa thất bại" });
        }
        [HttpPost]
        public JsonResult ActiveArticle(string id)
        {
            BaiViet article = db.BaiViets.Where(m => m.idBaiViet.Equals(id)).FirstOrDefault();
            if (article != null)
            {
                if (article.trangThai == true)
                    article.trangThai = false;
                else
                    article.trangThai = true;
                db.SaveChanges();
                return Json(new { status = true, message = "Đã thay đổi trạng thái bài viết thành công " });
            }
            return Json(new { status = false, message = "Đã thay đổi trạng thái bài viết thất bại " });
        }
        private void SaveImage(HttpPostedFileBase image, BaiViet article)
        {
            if (image != null && image.ContentLength > 0)
            {
                string virtualPath = "/Asset/image/article/";
                string physicalPath = Server.MapPath("~" + virtualPath);
                string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                byte[] avatarImage = new byte[image.ContentLength];
                image.InputStream.Read(avatarImage, 0, image.ContentLength);

                image.SaveAs(physicalPath + fileName);
                article.hinhDaiDien = virtualPath + fileName;
            }
            else
            {
                article.hinhDaiDien = "";
            }
        }

    }
}