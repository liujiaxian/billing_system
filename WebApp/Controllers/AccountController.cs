using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Utility;

namespace WebApp.Controllers
{
    public class AccountController : BaseController
    {
        //
        // GET: /Account/

        #region 登录
        public ActionResult Login()
        {
            HttpCookie ck1 = Request.Cookies["loginid"];
            HttpCookie ck2 = Request.Cookies["loginpwd"];

            if (ck1 != null && ck2 != null)
            {
                string loginid = Request.Cookies["loginid"].Value.DecryptStr();
                string loginpwd = Request.Cookies["loginpwd"].Value.DecryptStr();

                if (!string.IsNullOrEmpty(loginid) && !string.IsNullOrEmpty(loginpwd))
                {
                    var model = db.sys_user.Where(c => (c.user_email == loginid || c.user_name == loginid) && c.user_password == loginpwd).FirstOrDefault();
                    if (model == null)
                    {
                        ck1.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(ck1);

                        ck2.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(ck2);
                    }
                    else
                    {
                        Session["billing_system_user_model"] = model;
                        return Redirect("/center/index");
                    }
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            string loginid = Request["name"].Trim();
            if (string.IsNullOrEmpty(loginid))
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号不能为空！", null));
            }

            string pwd = Request["pwd"].Trim();
            if (string.IsNullOrEmpty(pwd))
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录密码不能为空！", null));
            }

            if (pwd.Length < 6)
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号或密码错误！", null));
            }

            var model = db.sys_user.Where(c => c.user_email == loginid || c.user_name == loginid).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号或密码错误！", null));
            }

            if (model.user_password != pwd)
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号或密码错误！", null));
            }

            if (model.apply_status == (int)Enum_Member_Status.黑名单)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该账号已被禁用！", null));
            }
            if (model.apply_status == (int)Enum_Member_Status.申请中)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该账号等待管理员审核，审核通过后会通知到您的邮箱！", null));
            }

            bool check = Request["checked"].ToBoolean();
            if (check)
            {
                HttpCookie ck1 = new HttpCookie("loginid", loginid.EncryptStr());
                ck1.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(ck1);

                HttpCookie ck2 = new HttpCookie("loginpwd", pwd.EncryptStr());
                ck2.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(ck2);
            }

            Session["billing_system_user_model"] = model;

            model.login_count++;
            model.login_time = DateTime.Now;
            db.SaveChanges();

            return Content(ReturnMsg(Enum_Return.成功, "[" + loginid + "]登录成功,正在进入系统...", null));
        } 
        #endregion

        #region 注册
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            string loginid = Request["name"].Trim();
            if (string.IsNullOrEmpty(loginid))
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号不能为空！", null));
            }

            string email = Request["email"].Trim();
            if (string.IsNullOrEmpty(email))
            {
                return Content(ReturnMsg(Enum_Return.失败, "邮箱地址不能为空！", null));
            }

            string pwd = Request["pwd"].Trim();
            if (string.IsNullOrEmpty(pwd))
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录密码不能为空！", null));
            }
            if (pwd.Length < 6)
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录密码不能小于6个字符！", null));
            }

            string repwd = Request["repwd"].Trim();
            if (pwd != repwd)
            {
                return Content(ReturnMsg(Enum_Return.失败, "确认密码与密码不一致！", null));
            }

            var isloginid = db.sys_user.Where(c => c.user_name == loginid).FirstOrDefault();
            if (isloginid != null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "登录账号已存在！", null));
            }

            var isemail = db.sys_user.Where(c => c.user_email == email).FirstOrDefault();
            if (isemail != null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "邮箱地址已存在！", null));
            }

            sys_user model = new sys_user();
            model.user_name = loginid;
            model.user_nickname = loginid;
            model.user_email = email;
            model.user_password = pwd;
            model.user_role = (int)Enum_User.管理员;
            model.apply_status = (int)Enum_Member_Status.正常;
            model.create_time = DateTime.Now;
            model.update_time = DateTime.Now;
            model.user_face = "/Content/img/default_headpic.png";
            db.sys_user.Add(model);
            db.Configuration.ValidateOnSaveEnabled = false;
            int n = db.SaveChanges();
            db.Configuration.ValidateOnSaveEnabled = true;
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "注册失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "注册成功，等待管理员审核！", null));
        } 
        #endregion

        #region 忘记密码
        public ActionResult ForgetPwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPwd(FormCollection collection)
        {
            string email = Request["email"];
            if (string.IsNullOrEmpty(email))
            {
                return Content(ReturnMsg(Enum_Return.失败, "邮箱地址不能为空！", null));
            }

            var model = db.sys_user.Where(c => c.user_email == email).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "系统不存在此邮箱！", null));
            }

            if (model.apply_status != (int)Enum_Member_Status.正常)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该用户异常，不允许使用此功能！", null));
            }

            #region 发送邮件
            //生成激活码
            Guid guid = Guid.NewGuid();
            string activetoken = guid.ToString().Md5();
            //更新token和token时间
            model.email_token = activetoken;
            model.email_token_time = DateTime.Now;
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "发送失败，请重试或联系管理员！", null));
            }
            //拼接激活的url
            string host = Request.Url.Scheme + "://" + Request.Url.Authority;
            string url = host + "/account/forgetpwdreset?email=" + email + "&token=" + activetoken;
            //获取内容
            string htmlbody = MailHelper.CreateHtmlPage(email, url);
            //调用邮件类发送邮件
            bool flag = MailHelper.Send(email, "找回密码链接", htmlbody);
            #endregion

            if (!flag)
            {
                return Content(ReturnMsg(Enum_Return.失败, "发送失败，请重试或联系管理员！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "发送成功，请登录" + email + "邮箱查看重置密码邮件！", null));
        }

        public ActionResult ForgetPwdReset()
        {
            string email = Request["email"];
            string token = Request["token"];
            if (string.IsNullOrEmpty(email)||string.IsNullOrEmpty(token))
            {
                return RedirectToAction("forgetpwd");
            }

            var model = db.sys_user.Where(c => c.user_email == email && c.email_token == token).FirstOrDefault();
            if (model==null)
            {
                 return RedirectToAction("forgetpwd");
            }

            if (Convert.ToDateTime(model.email_token_time).AddMinutes(30) < DateTime.Now)
            {
                return RedirectToAction("forgetpwd");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPwdReset(FormCollection coolection)
        {
            string email = Request["email"];
            if (string.IsNullOrEmpty(email))
            {
                return Content(ReturnMsg(Enum_Return.失败, "邮箱地址不能为空！", null));
            }

            var model = db.sys_user.Where(c => c.user_email == email).FirstOrDefault();
            if (model==null)
            {
                 return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            if (Convert.ToDateTime(model.email_token_time).AddMinutes(30) < DateTime.Now)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该链接已失效！", null));
            }

            string pwd = Request["pwd"];
            if (string.IsNullOrEmpty(pwd))
            {
                return Content(ReturnMsg(Enum_Return.失败, "密码不能为空！", null));
            }
            if (pwd.Trim().Length<6)
            {
                 return Content(ReturnMsg(Enum_Return.失败, "密码不能小于6个字符！", null));
            }

            model.user_password = pwd;
            model.update_time = DateTime.Now;
            int n = db.SaveChanges();

            if (n<=0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "重置失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "重置成功！", null));
        }
        #endregion
    }
}
