using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanGiay.Models;

namespace BanGiay.Models
{
    public class CustomAuthorize : AuthorizeAttribute
    {
        static ShoesShopEntities db = new ShoesShopEntities();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var account = HttpContext.Current.Session["InformationAccount"] as TaiKhoan;
            if (account != null)
            {
                var roleArray = Roles.Split(',').ToList();
                foreach (var role in roleArray)
                {
                    if (account.maNhom == getIDAccountGroup(role.Trim()))
                        return true;
                }
            }
            HttpContext.Current.Response.Redirect("~/Account/DenielAccess");
            return false;
        }
        /// <summary>   
        /// Hàm này chuyển tên nhóm về mã nhóm
        /// </summary>  
        /// <returns></returns>
        private int getIDAccountGroup(string name)
        {
            List<NhomTaiKhoan> accountGroupList = db.NhomTaiKhoans.ToList();
            int result = 0;
            foreach (var accountGroup in accountGroupList)
            {
                if (name.ToLower().Equals(accountGroup.tenNhom))
                    result = accountGroup.maNhomTaiKhoan;
            }
            return result;
        }

    }
}