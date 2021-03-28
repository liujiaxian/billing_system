using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;

namespace WebApp.Controllers
{
    public class FilterController : BaseController
    {
        public sys_user UserInfo { get; set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["billing_system_user_model"] == null)
            {
                if (String.Equals(Request.RequestType, "POST", StringComparison.CurrentCultureIgnoreCase))
                {
                    Response.Write(ReturnMsg(Utility.Enum_Return.失败,"登录失效，请重新登录！",null));
                    Response.End();
                }
                else
                {
                    Response.Redirect("/account/login");
                }
            }
            else
            {
                sys_user user = Session["billing_system_user_model"] as sys_user;
                if (user == null)
                {
                    Response.Redirect("/account/login");
                }

                UserInfo = user;
            }

        }
    }
}
