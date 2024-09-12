using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanGiay.Models
{
    public class CustomAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var account = HttpContext.Current.Session["InformationAccount"] as TaiKhoan;
            if (account == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}