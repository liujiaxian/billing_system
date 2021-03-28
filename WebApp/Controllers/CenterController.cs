using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Model;
using Utility;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    public class CenterController : FilterController
    {
        //
        // GET: /Center/

        #region 主页
        public ActionResult Index()
        {
            if (UserInfo == null)
            {
                return Redirect("/Account/Login");
            }
            string useridstr = UserInfo.user_id.ToString();
            IQueryable<vyw_billing_log_team> result = db.vyw_billing_log_team.Where(c => c.is_delete == false && c.participant_user_id.Contains(useridstr));

            //用户数
            var usercount = db.sys_user.Where(c => c.apply_status == (int)Enum_Member_Status.正常).Count();
            //支出数
            decimal moneycount = 0;
            if (result.Where(c => c.trade_type == true).Count() > 0)
            {
                moneycount = result.Where(c => c.trade_type == true).Select(c => c.money).Sum();
            }

            //收入数
            decimal moneyincome = 0;
            if (result.Where(c => c.trade_type == false).Count() > 0)
            {
                moneyincome = result.Where(c => c.trade_type == false).Select(c => c.money).Sum();
            }

            //登录数
            var logincount = db.sys_user.Where(c => true).Select(c => c.login_count).Sum();

            //最近账单
            var billinglist = result.OrderByDescending(c => c.trade_time).Skip(0).Take(5).ToList();
            ViewData["billinglist"] = billinglist;

            string day = DateTime.Now.ToString("yyyy-MM-dd");
            string month = DateTime.Now.ToString("yyyy-MM");
            string year = DateTime.Now.ToString("yyyy");

            //本日支出
            var dayMoney = (from d in result.ToList()
                            where d.trade_time.ToString("yyyy-MM-dd") == day
                            select d).Sum(c => c.money);
            //一周的时间
            DateTime weekTime = DateTime.Now.AddDays(-7);
            //本周支出
            var weekAllMoney = (from d in result.ToList()
                                where d.trade_time >= weekTime
                                select d).Sum(c => c.money);

            //本月支出
            var monthMoney = (from d in result.ToList()
                              where d.trade_time.ToString("yyyy-MM") == month
                              select d).Sum(c => c.money);
            //本年支出
            var yearMoney = (from d in result.ToList()
                             where d.trade_time.ToString("yyyy") == year
                             select d).Sum(c => c.money);

            string userid = UserInfo.user_id.ToString();

            var monthResult = from d in result.ToList()
                              where d.trade_time.ToString("yyyy-MM") == month
                              select d;

            //本月总交易金额
            var allTradeMoney = monthResult.Sum(c => c.money);
            //当前用户本月交易金额
            decimal userTradeMoney = 0;
            var userTradeMoneyModel = monthResult.Where(c => c.user_id == UserInfo.user_id).Count();
            if (userTradeMoneyModel > 0)
            {
                userTradeMoney = monthResult.Where(c => c.user_id == UserInfo.user_id).Sum(d => d.money);
            }
            //用户交易率
            var tradeRate = userTradeMoney == 0 ? 0 : (int)(Math.Ceiling((userTradeMoney / allTradeMoney) * 100));



            //总参与数
            var allParticipantCount = monthResult.Count();
            //本月用户总参与数
            var userParticipantCount = monthResult.Where(c => c.participant_user_id.Contains(userid)).Count();
            //本月用户参与率
            var participantRate = allParticipantCount == 0 ? 0 : (int)(Math.Ceiling((userParticipantCount * 1.0 / allParticipantCount) * 100));

            //总支出金额           
            var allMoney = result.Count() > 0 ? result.Sum(c => c.money) : 0;
            //本月用户支出率
            int moneyRate = allMoney == 0 ? 0 : (int)(Math.Ceiling((allTradeMoney / allMoney) * 100));

            //上月
            string lastMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");

            var lastMonthResult = from d in result.ToList()
                                  where d.trade_time.ToString("yyyy-MM") == lastMonth
                                  select d;

            //上月总交易金额
            var lastAllTradeMoney = lastMonthResult.Sum(c => c.money);
            //当前用户交易金额
            decimal lastUserTradeMoney = 0;
            var lastUserTradeMoneyModel = lastMonthResult.Where(c => c.user_id == UserInfo.user_id).Count();
            if (lastUserTradeMoneyModel > 0)
            {
                lastUserTradeMoney = lastMonthResult.Where(c => c.user_id == UserInfo.user_id).Sum(d => d.money);
            }
            //用户交易率
            var lastTradeRate = lastUserTradeMoney == 0 ? 0 : (int)(Math.Ceiling((lastUserTradeMoney / lastAllTradeMoney) * 100));

            //上月用户参与数
            var lastParticipantCount = lastMonthResult.Count() <= 0 ? 0 : lastMonthResult.Count();
            //上月用户总参与数
            var lastUserParticipantCount = lastMonthResult.Where(c => c.participant_user_id.Contains(userid)).Count();
            //上月用户参与率
            var lastParticipantRate = lastParticipantCount == 0 ? 0 : (int)(Math.Ceiling((lastUserParticipantCount * 1.0 / lastParticipantCount) * 100));


            //上月用户支出率
            var lastMoneyRate = allMoney == 0 ? 0 : (int)Math.Ceiling((lastAllTradeMoney / allMoney) * 100);

            ViewBag.LastTradeRate = tradeRate - lastTradeRate;
            ViewBag.LastParticipantRate = participantRate - lastParticipantRate;
            ViewBag.LastMoneyRate = moneyRate - lastMoneyRate;

            //近一周的数据
            var weekMoney = result.Where(c => c.trade_time >= weekTime).Count() > 0 ? result.Where(c => c.trade_time >= weekTime).Sum(c => c.money) : 0;
            decimal weekUserTrade = 0;
            var weekUserTradeModel = result.Where(c => c.user_id == UserInfo.user_id && c.trade_time >= weekTime).Count();
            if (weekUserTradeModel > 0)
            {
                weekUserTrade = result.Where(c => c.user_id == UserInfo.user_id && c.trade_time >= weekTime).Sum(c => c.money);
            }
            var weekUserParticipant = result.Where(c => c.participant_user_id.Contains(userid) && c.trade_time >= weekTime).Count();

            ViewBag.UserCount = usercount;
            ViewBag.MoneyCount = moneycount == 0 ? "0" : Convert.ToDecimal(moneycount).ToString("0.00");
            ViewBag.MoneyIncome = moneyincome == 0 ? "0" : Convert.ToDecimal(moneyincome).ToString("0.00");
            ViewBag.LoginCount = logincount;

            ViewBag.DayMoney = dayMoney == 0 ? "0" : dayMoney.ToString("0.00");
            ViewBag.WeekAllMoney = weekAllMoney == 0 ? "0" : weekAllMoney.ToString("0.00");
            ViewBag.MonthMoney = monthMoney == 0 ? "0" : monthMoney.ToString("0.00");
            ViewBag.YearMoney = yearMoney == 0 ? "0" : yearMoney.ToString("0.00");

            ViewBag.TradeRate = tradeRate;
            ViewBag.MoneyRate = moneyRate;
            ViewBag.ParticipantRate = participantRate;

            ViewBag.WeekUserTrade = weekUserTrade == 0 ? "0" : weekUserTrade.ToString("0.00");
            ViewBag.WeekMoney = weekMoney == 0 ? "0" : weekMoney.ToString("0.00");
            ViewBag.WeekUserParticipant = weekUserParticipant;

            ViewBag.MonthTradeCount = userParticipantCount;

            //最新的消息
            DateTime time = DateTime.Now.AddDays(-5);
            var noticeModel = db.sys_notice.Where(c => c.type_id == (int)Enum_Notice_Type.公告 && c.is_delete == false && c.create_time > time).OrderByDescending(c => c.notice_id).Skip(0).Take(1).FirstOrDefault();
            if (noticeModel != null)
            {
                ViewBag.NoticeContent = noticeModel.msg;
                ViewBag.NoticeID = noticeModel.notice_id;
            }

            return View();
        }
        #region 按年月日分组
        [HttpPost]
        public ActionResult GetData()
        {
            string timeFormat = Request["timeFormat"];

            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;

            if (string.IsNullOrEmpty(timeFormat))
            {
                timeFormat = year + "-" + month;
            }

            IQueryable<billing_log> result = db.billing_log.Where(c => true);

            if (timeFormat.IndexOf('-') != -1)//查询月份
            {
                var newlist = from d in result.ToList()
                              where d.trade_time.ToString("yyyy-MM") == timeFormat
                              group d by d.trade_time.ToString("yyyy-MM-dd") into g
                              select new
                              {
                                  time = g.Key,
                                  money = g.Sum(e => e.money)
                              };
                return Content(ReturnMsg(Enum_Return.成功, "获取成功", JsonConvert.SerializeObject(newlist)));
            }
            else
            {
                var newlist = from d in result.ToList()
                              where d.trade_time.ToString("yyyy") == timeFormat
                              group d by d.trade_time.ToString("yyyy-MM") into g
                              select new
                              {
                                  time = g.Key,
                                  money = g.Sum(e => e.money)
                              };
                return Content(ReturnMsg(Enum_Return.成功, "获取成功", JsonConvert.SerializeObject(newlist)));
            }
        }
        #endregion
        #endregion

        #region 用户管理
        public ActionResult UserList()
        {
            int page;
            if (!int.TryParse(Request["page"], out page))
            {
                page = 1;
            }

            IQueryable<sys_user> result = db.sys_user.Where(c => c.user_role != (int)Utility.Enum_User.游客);
            int totalCount = result.Count();

            int pageSize = 20;

            int pageCount = (int)Math.Ceiling(totalCount * 1.0 / pageSize);

            page = page <= 0 ? 1 : page;
            page = page > totalCount ? totalCount : page;

            List<sys_user> list = null;

            if (totalCount > 0)
            {
                list = result.OrderByDescending(c => c.create_time).Skip((Convert.ToInt32(page) - 1) * pageSize).Take(pageSize).ToList();
            }

            ViewData["list"] = list;
            ViewBag.Page = page;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageCount = pageCount;
            ViewBag.PageSize = pageSize;
            ViewBag.Params = "/center/userlist?page=";
            return View();
        }

        public ActionResult UserSave(int? id)
        {
            sys_user userModel = new sys_user();
            if (!string.IsNullOrEmpty(id.ToString()))
            {
                userModel = db.sys_user.Where(c => c.user_id == id).FirstOrDefault();
            }

            ViewData["user"] = userModel;
            return View();
        }
        [HttpPost]
        public ActionResult UserSave()
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
            if (!string.IsNullOrEmpty(pwd))
            {
                if (pwd.Length < 6)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "登录密码不能小于6个字符！", null));
                }
            }
            

            int userID = Request["userID"].ToInt32();
            if (userID == 0)
            {
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
            }
            else
            {
                var model = db.sys_user.Where(c => c.user_id == userID).FirstOrDefault();

                var isloginid = db.sys_user.Where(c => c.user_name == loginid&&c.user_id!=userID).FirstOrDefault();
                if (isloginid != null)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "登录账号已存在！", null));
                }

                var isemail = db.sys_user.Where(c => c.user_email == email && c.user_id != userID).FirstOrDefault();
                if (isemail != null)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "邮箱地址已存在！", null));
                }

                model.user_name = loginid;
                model.user_email = email;
                if (!string.IsNullOrEmpty(pwd))
                {
                    model.user_password = pwd;
                }
                model.update_time = DateTime.Now;
            }

            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "操作失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "操作成功！", null));
        }

        [HttpPost]
        public ActionResult UserDelete()
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }
            int id;
            if (!int.TryParse(Request["id"], out id))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            var model = db.sys_user.Where(c => c.user_id == id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }

            //删除团队
            var teamuser = db.team_user.Where(c => c.userID == id).ToList();
            if (teamuser.Count > 0)
            {
                foreach (var item in teamuser)
                {
                    db.Entry(item).State = System.Data.EntityState.Deleted;
                }
            }

            db.Entry(model).State = System.Data.EntityState.Deleted;
            int n = db.SaveChanges();

            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "删除失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "删除成功！", null));
        }
        #endregion

        #region 账单记录
        public ActionResult Billing()
        {
            long teamID = Request["teamid"].ToInt64();
            if (teamID == 0)
            {
                return RedirectToAction("index");
            }

            int page;
            if (!int.TryParse(Request["page"], out page))
            {
                page = 1;
            }
           
            IQueryable<vyw_billing_log_team> result = db.vyw_billing_log_team.Where(c => true);

            if (UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                result = result.Where(c => c.is_delete == false && c.status == (int)Utility.Enum_Inviter_Status.正常);
            }

            if (teamID > 0)
            {
                result = result.Where(c => c.team_id == teamID);
            }

            string tuser = Request["tuser"];
            if (!string.IsNullOrEmpty(tuser))
            {
                result = result.Where(c => c.trade_user.Contains(tuser));
            }

            string puser = Request["puser"];
            if (!string.IsNullOrEmpty(puser))
            {
                result = result.Where(c => c.participant_user_name.Contains(puser));
            }

            string remark = Request["remark"];
            if (!string.IsNullOrEmpty(remark))
            {
                result = result.Where(c => c.remark.Contains(remark));
            }

            string stime = Request["stime"];
            string etime = Request["etime"];
            if (!string.IsNullOrEmpty(stime) && !string.IsNullOrEmpty(etime))
            {
                DateTime starttime = (stime + " 0:0:1").ToDateTime();
                DateTime endtime = (etime + " 23:59:59").ToDateTime();
                result = result.Where(c => c.trade_time >= starttime && c.trade_time <= endtime);
            }

            long userID = Request["userid"].ToInt64();
            if (userID > 0)
            {
                result = result.Where(c => c.user_id == userID);
            }


            int totalCount = result.Count();

            int pageSize = 20;

            int pageCount = (int)Math.Ceiling(totalCount * 1.0 / pageSize);

            page = page <= 0 ? 1 : page;
            page = page > totalCount ? totalCount : page;

            List<vyw_billing_log_team> list = null;

            if (totalCount > 0)
            {
                list = result.OrderByDescending(c => c.trade_time).Skip((Convert.ToInt32(page) - 1) * pageSize).Take(pageSize).ToList();
            }

            //团队
            var teamList = db.team.Where(c => c.userID == UserInfo.user_id).ToList();
            ViewData["teamList"] = teamList;

            ViewData["list"] = list;
            ViewBag.Page = page;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageCount = pageCount;
            ViewBag.PageSize = pageSize;
            ViewBag.Params = "/Center/Billing?teamid=" + teamID + "&tuser=" + tuser + "&puser=" + puser + "&remark=" + remark + "&stime=" + stime + "&etime=" + etime + "&userid=" + userID + "&page=";
            ViewBag.UserId = UserInfo.user_id;
            ViewBag.UserRoleId = UserInfo.user_role;

            return View();
        }
        #endregion

        #region 添加账单
        public ActionResult BillingAdd()
        {
            int teamID = Request["teamid"].ToInt32();
            if (teamID == 0)
            {
                return RedirectToAction("index");
            }
         
            var userlist = db.vyw_team_user.Where(c => c.apply_status == (int)Enum_Member_Status.正常 && c.user_role != (int)Enum_User.游客&&c.teamID==teamID).ToList();
            if (userlist.Count <= 0)
            {
                return RedirectToAction("index");
            }
            ViewData["userlist"] = userlist;

            ViewBag.UserID = UserInfo.user_id;
            ViewBag.TeamName = userlist.Count > 0 ? userlist[0].teamName : "";

            return View();
        }
        [HttpPost]
        public ActionResult BillingAdd(FormCollection collection)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }


            bool type = Request["type"].ToBoolean();

            string tradeuser = Request["tradeuser"];

            int tradeuserid = Convert.ToInt32(Request["tradeuserid"]);

            string participant_user_id = Request["participant"];

            if (string.IsNullOrEmpty(participant_user_id))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请选择参与人员！", null));
            }

            string participant_user_name = Request["participant_name"];

            string trademoney = Request["trademoney"].Trim();
            if (string.IsNullOrEmpty(trademoney))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请输入交易金额！", null));
            }

            decimal money;

            if (!Decimal.TryParse(trademoney, out money))
            {
                return Content(ReturnMsg(Enum_Return.失败, "交易金额格式不正确！", null));
            }

            string tradedetail = Request["tradedetail"];

            string tradetime = Request["tradetime"];
            if (string.IsNullOrEmpty(tradetime))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请输入交易时间！", null));
            }
            DateTime time;
            if (!DateTime.TryParse(tradetime, out time))
            {
                return Content(ReturnMsg(Enum_Return.失败, "交易时间格式不正确！", null));
            }
            var teamid = Request["teamid"].ToInt64();

            billing_log model = new billing_log();
            model.trade_type = type;
            model.participant_user_id = participant_user_id;
            model.money = type == true ? -money : money;
            model.remark = tradedetail;
            model.is_delete = false;
            model.trade_user = tradeuser;
            model.user_id = tradeuserid;
            model.participant_user_name = participant_user_name;
            model.trade_time = time;
            model.create_time = DateTime.Now;
            model.update_time = DateTime.Now;
            model.team_id = teamid;
            db.billing_log.Add(model);
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "添加失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "添加成功！", null));
        }
        #endregion

        #region 编辑账单
        public ActionResult BillingEdit(int? id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return RedirectToAction("billing");
            }

            var model = db.billing_log.Where(c => c.is_delete == false && c.billing_id == id).FirstOrDefault();
            if (model == null)
            {
                return RedirectToAction("billing");
            }

            var userlist = db.sys_user.Where(c => c.apply_status == (int)Enum_Member_Status.正常 && c.user_role != (int)Enum_User.游客).ToList();
            ViewData["userlist"] = userlist;

            var teamlist = db.vyw_team_user.Where(c => c.userID == UserInfo.user_id).ToList();
            ViewData["teamlist"] = teamlist;

            ViewData["billingmodel"] = model;

            return View();
        }
        [HttpPost]
        public ActionResult BillingEdit(FormCollection collection)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }
            int id;
            if (!int.TryParse(Request["id"], out id))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }

            var model = db.billing_log.Where(c => c.is_delete == false && c.billing_id == id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }
            if (model.user_id != UserInfo.user_id && UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                return Content(ReturnMsg(Enum_Return.失败, "不能修改别人的数据，特别情况请联系管理员！", null));
            }

            bool type = Request["type"].ToBoolean();

            string tradeuser = Request["tradeuser"];

            int tradeuserid = Convert.ToInt32(Request["tradeuserid"]);

            string participant_user_id = Request["participant"];

            if (string.IsNullOrEmpty(participant_user_id))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请选择参与人员！", null));
            }

            string participant_user_name = Request["participant_name"];

            string trademoney = Request["trademoney"].Trim();
            if (string.IsNullOrEmpty(trademoney))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请输入交易金额！", null));
            }

            decimal money;

            if (!Decimal.TryParse(trademoney, out money))
            {
                return Content(ReturnMsg(Enum_Return.失败, "交易金额格式不正确！", null));
            }

            string tradedetail = Request["tradedetail"];

            string tradetime = Request["tradetime"];
            if (string.IsNullOrEmpty(tradetime))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请输入交易时间！", null));
            }
            DateTime time;
            if (!DateTime.TryParse(tradetime, out time))
            {
                return Content(ReturnMsg(Enum_Return.失败, "交易时间格式不正确！", null));
            }

            var teamid = Request["teamid"].ToInt64();

            model.trade_type = type;
            model.participant_user_id = participant_user_id;
            model.money = type == true ? (-money) : money;
            model.remark = tradedetail;
            model.trade_user = tradeuser;
            model.user_id = tradeuserid;
            model.participant_user_name = participant_user_name;
            model.trade_time = time;
            model.team_id = teamid;
            model.update_time = DateTime.Now;
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "编辑失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "编辑成功！", null));
        }
        #endregion

        #region 删除账单
        [HttpPost]
        public ActionResult BillingDelete(int? id)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }

            var model = db.billing_log.Where(c => c.billing_id == id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }
            if (model.user_id != UserInfo.user_id && UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                return Content(ReturnMsg(Enum_Return.失败, "不能删除别人的数据，特别情况请联系管理员！", null));
            }

            string str = model.is_delete == true ? "点击恢复" : "点击删除";

            model.is_delete = model.is_delete == true ? false : true;
            model.update_time = DateTime.Now;
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "操作失败，请重试！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, str, null));
        }
        #endregion

        #region 个人信息
        public ActionResult Personal()
        {
            if (UserInfo == null)
            {
                return Redirect("/account/login");
            }

            //活跃度
            int userlogincount = (int)db.sys_user.Where(c => true).Select(c => c.login_count).Sum();
            int starcount = 0;
            if (userlogincount != 0)
            {
                starcount = (int)Math.Floor(((double)UserInfo.login_count * 1.0 / userlogincount) * 5);
            }
            ViewBag.StarCount = starcount;

            string userid = UserInfo.user_id.ToString();

            //交易数
            var tradecount = db.billing_log.Where(c => c.is_delete == false && c.user_id == UserInfo.user_id).Count();
            //参与数
            var participantcount = db.billing_log.Where(c => c.is_delete == false && c.participant_user_id.Contains(userid)).Count();
            //交易额
            var trademoneymodel = db.billing_log.Where(c => c.is_delete == false && c.user_id == UserInfo.user_id).ToList();
            decimal trademoney = 0;
            if (trademoneymodel.Count > 0)
            {
                trademoney = trademoneymodel.Select(c => c.money).Sum();
            }

            ViewBag.TradeCount = tradecount;
            ViewBag.ParticipantCount = participantcount;
            ViewBag.TradeMoney = Convert.ToDecimal(trademoney).ToString("0.00");

            ViewData["userinfo"] = db.sys_user.Where(c => c.user_id == UserInfo.user_id).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public ActionResult PersonalUpdate()
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }

            var model = db.sys_user.Where(c => c.apply_status == (int)Enum_Member_Status.正常 && c.user_id == UserInfo.user_id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }
            string userface = Request["userface"];
            if (string.IsNullOrEmpty(userface))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请上传用户头像！", null));
            }
            string username = Request["username"];
            if (string.IsNullOrEmpty(username))
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户名称不能为空！", null));
            }

            var model2 = db.sys_user.Where(c => c.user_name == username && c.user_name != model.user_name).FirstOrDefault();
            if (model2 != null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户名称已存在！", null));
            }

            string email = Request["email"];
            if (string.IsNullOrEmpty(email))
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户邮箱不能为空！", null));
            }

            var model1 = db.sys_user.Where(c => c.user_email == email && c.user_email != model.user_email).FirstOrDefault();
            if (model1 != null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户邮箱已存在！", null));
            }

            string nickname = Request["nickname"];
            if (string.IsNullOrEmpty(nickname))
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户昵称不能为空！", null));
            }

            model.user_face = userface;
            model.user_name = username;
            model.user_nickname = nickname;
            model.user_email = email;
            model.update_time = DateTime.Now;
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "更新失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "更新成功！", null));
        }
        #endregion

        #region 修改密码
        public ActionResult UpdatePwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePwd(FormCollection collection)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }

            var model = db.sys_user.Where(c => c.user_id == UserInfo.user_id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }

            string oldpwd = Request["pwd"].Trim();
            if (string.IsNullOrEmpty(oldpwd))
            {
                return Content(ReturnMsg(Enum_Return.失败, "原来密码不能为空！", null));
            }
            if (oldpwd != model.user_password)
            {
                return Content(ReturnMsg(Enum_Return.失败, "原来密码不正确！", null));
            }

            string newpwd = Request["newpwd"].Trim();
            if (string.IsNullOrEmpty(newpwd))
            {
                return Content(ReturnMsg(Enum_Return.失败, "新的密码不能为空！", null));
            }
            if (newpwd.Length < 6)
            {
                return Content(ReturnMsg(Enum_Return.失败, "新的密码长度不能小于6个字符！", null));
            }

            model.user_password = newpwd;
            model.update_time = DateTime.Now;
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "更新失败！", null));
            }

            Layout();
            return Content(ReturnMsg(Enum_Return.成功, "更新成功！", null));
        }
        #endregion

        #region 消息管理
        public ActionResult MyNotice(int? id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                id = 1;
            }

            int pageSize = 5;

            int count = 0;

            if (UserInfo.user_role == (int)Enum_User.超级管理员)
            {
                count = db.vyw_notice_user.Where(c => c.type_id != (int)Enum_Notice_Type.公告).Count();
            }
            else
            {
                count = db.vyw_notice_user.Where(c => c.user_id == UserInfo.user_id && c.type_id != (int)Enum_Notice_Type.公告 && c.is_delete == false).Count();
            }

            int pageCount = (int)Math.Ceiling(count * 1.0 / pageSize);

            List<vyw_notice_user> list = null;
            if (count > 0)
            {
                if (UserInfo.user_role == (int)Enum_User.超级管理员)
                {
                    list = db.vyw_notice_user.Where(c => c.type_id != (int)Enum_Notice_Type.公告).OrderByDescending(c => c.notice_id).Skip((Convert.ToInt32(id) - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    list = db.vyw_notice_user.Where(c => c.user_id == UserInfo.user_id && c.type_id != (int)Enum_Notice_Type.公告 && c.is_delete == false).OrderByDescending(c => c.notice_id).Skip((Convert.ToInt32(id) - 1) * pageSize).Take(pageSize).ToList();
                }

            }

            ViewData["list"] = list;
            ViewBag.Page = id;
            ViewBag.PageCount = pageCount;
            ViewBag.Params = "/center/mynotice/";
            ViewBag.UserRoleId = UserInfo.user_role;

            return View();
        }

        //删除消息
        [HttpPost]
        public ActionResult MyNoticeDelete(int? id)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }


            var model = db.sys_notice.Where(c => c.notice_id == id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }
            if (UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                return Content(ReturnMsg(Enum_Return.失败, "不能删除别人的数据，特别情况请联系管理员！", null));
            }

            if (model.type_id == (int)Enum_Notice_Type.公告 && UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                return Content(ReturnMsg(Enum_Return.失败, "你没有权限删除！", null));
            }

            string str = model.is_delete == true ? "恢复成功！" : "删除成功！";

            model.is_delete = model.is_delete == true ? false : true;
            model.update_time = DateTime.Now;


            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "操作失败，请重试！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, str, null));
        }

        public ActionResult SysNotice(int? id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                id = 1;
            }

            int pageSize = 5;

            int count = db.sys_notice.Where(c => true && c.type_id == (int)Enum_Notice_Type.公告 && c.is_delete == false).Count();

            int pageCount = (int)Math.Ceiling(count * 1.0 / pageSize);

            List<sys_notice> list = null;
            if (count > 0)
            {
                list = db.sys_notice.Where(c => true && c.type_id == (int)Enum_Notice_Type.公告 && c.is_delete == false).OrderByDescending(c => c.notice_id).Skip((Convert.ToInt32(id) - 1) * pageSize).Take(pageSize).ToList();
            }

            ViewData["list"] = list;
            ViewBag.Page = id;
            ViewBag.PageCount = pageCount;
            ViewBag.Params = "/center/sysnotice/";
            ViewBag.UserRole = UserInfo.user_role;

            return View();
        }
        #endregion

        #region 发送信息
        [HttpPost]
        public ActionResult SendMsg()
        {

            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }

            int id;
            if (!int.TryParse(Request["id"], out id))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            int typeid;
            if (!int.TryParse(Request["typeid"], out typeid))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            int userid;
            if (!int.TryParse(Request["userid"], out userid))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            string msg = Request["msg"];
            if (string.IsNullOrEmpty(msg))
            {
                return Content(ReturnMsg(Enum_Return.失败, "发送内容不能为空！", null));
            }

            if (typeid == (int)Enum_Notice_Type.公告 && UserInfo.user_role != (int)Enum_User.超级管理员)
            {
                return Content(ReturnMsg(Enum_Return.失败, "您没有权限操作！", null));
            }


            sys_notice model = new sys_notice();
            model.create_time = DateTime.Now;
            model.is_delete = false;
            model.msg = msg;
            model.billing_id = id;
            model.type_id = typeid;
            model.update_time = DateTime.Now;

            if (userid <= 0)
            {
                model.user_id = UserInfo.user_id;
            }
            else
            {
                model.user_id = userid;
            }


            db.sys_notice.Add(model);
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "发送失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "发送成功！", null));
        }
        #endregion

        #region 结算
        public ActionResult Calculation()
        {
            var teamlist = db.vyw_team_user.Where(c => c.userID == UserInfo.user_id).ToList();
            ViewData["teamlist"] = teamlist;
            return View();
        }
        [HttpPost]
        public ActionResult Calculation(FormCollection collection)
        {
            DateTime starttime;
            if (!DateTime.TryParse(Request["starttime"]+" 00:00:01", out starttime))
            {
                return Content(ReturnMsg(Enum_Return.失败, "开始时间格式不正确！", null));
            }

            DateTime endtime;
            if (!DateTime.TryParse(Request["endtime"]+" 23:59:59", out endtime))
            {
                return Content(ReturnMsg(Enum_Return.失败, "结束时间格式不正确！", null));
            }

            long teamID = Request["teamID"].ToInt64();

            //查询符合日期的数据
            IQueryable<vyw_billing_log_team> result = db.vyw_billing_log_team.Where(c => c.trade_time >= starttime && c.trade_time <= endtime && c.is_delete == false&&c.team_id==teamID);

            if (teamID > 0)
            {
                result = result.Where(c => c.team_id == teamID);
            }

            //分别查询不同的交易用户数据

            String newstr = null;
            //计算不同交易用户的金额
            var userlist = db.vyw_team_user.Where(c => c.apply_status == (int)Enum_Member_Status.正常 && c.user_role != (int)Enum_User.游客&&c.teamID==teamID).ToList();
            if (userlist.Count > 0)
            {
                foreach (var item in userlist)
                {
                    var ismodel = result.Where(c => c.user_id == item.userID).ToList();
                    if (ismodel == null)
                        continue;
                    decimal money1 = ismodel.Select(c => c.money).Sum();
                    //记录到字典中
                    String str1 = item.user_nickname + ":" + Convert.ToDecimal(money1).ToString("0.00");

                    //对当前用户的数据按照参与用户分组
                    var list2 = ismodel.GroupBy(c => c.participant_user_name).Select(d => new { User = d.Key, Money = d.Sum(e => e.money) }).ToList();
                    String str2 = null;
                    foreach (var item2 in list2)
                    {
                        int length = 0;

                        if (item2.User.IndexOf(',') != -1)
                        {
                            string[] strarray = item2.User.Split(',');
                            length = strarray.Length;
                        }

                        if (length > 0)
                        {
                            str2 += JsonConvert.SerializeObject(new { user = item2.User, usermoney = Convert.ToDecimal(item2.Money).ToString("0.00") + "/" + length + "=" + Convert.ToDecimal(item2.Money / length).ToString("0.00") });
                        }
                        else
                        {
                            str2 += JsonConvert.SerializeObject(new { user = item2.User, usermoney = item2.Money });
                        }

                        if (list2.IndexOf(item2) != list2.Count - 1)
                        {
                            str2 += ",";
                        }
                    }

                    newstr += JsonConvert.SerializeObject(new { tradeuser = str1, participantuser = "[" + str2 + "]" });

                    if (userlist.IndexOf(item) != userlist.Count - 1)
                    {
                        newstr += ",";
                    }
                }
            }

            return Content(ReturnMsg(Enum_Return.成功, "获取成功！", "[" + newstr + "]"));
        }


        public List<string> GetGroup(List<string> strs)
        {
            List<string> strlist = new List<string>();
            //List<string> strs = array.Split(',').OrderBy(t => t).ToList();
            string temp = "";
            //5位
            for (int i = 0; i < strs.Count; i++)
            {
                temp += strs[i] + ",";
            }
            strlist.Add(temp);
            //4位数
            for (int i = 0; i < strs.Count - 3; i++)
            {
                for (int j = i + 1; j < strs.Count - 2; j++)
                {
                    for (int k = j + 1; k < strs.Count - 1; k++)
                    {
                        for (int n = k + 1; n < strs.Count; n++)
                        {
                            temp = strs[i] + "," + strs[j] + "," + strs[k] + "," + strs[n];
                            strlist.Add(temp);
                        }
                    }
                }
            }
            //3位数

            for (int j = 0; j < strs.Count - 2; j++)
            {
                for (int k = j + 1; k < strs.Count - 1; k++)
                {
                    for (int n = k + 1; n < strs.Count; n++)
                    {
                        temp = strs[j] + "," + strs[k] + "," + strs[n];
                        strlist.Add(temp);
                    }
                }
            }
            //2位
            for (int k = 0; k < strs.Count - 1; k++)
            {
                for (int n = k + 1; n < strs.Count; n++)
                {
                    temp = strs[k] + "," + strs[n];
                    strlist.Add(temp);
                }
            }
            //1位
            for (int n = 0; n < strs.Count; n++)
            {
                temp = strs[n];
                strlist.Add(temp);
            }

            return strlist.OrderBy(t => t).ToList();
        }
        #endregion

        #region 团队管理
        public ActionResult Team()
        {
            var list = db.vyw_team_user.Where(c => c.userID == UserInfo.user_id).ToList();

            ViewData["list"] = list;

            ViewBag.UserID = UserInfo.user_id;

            return View();
        }

        public ActionResult TeamSave(long? id)
        {
            team teamModel = new team();
            var model = db.team.Where(c => c.teamID == id).FirstOrDefault();
            if (model != null)
            {
                teamModel = model;
            }

            ViewData["teamModel"] = teamModel;
            return View();
        }
        [HttpPost]
        public ActionResult TeamSave()
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }

            long id = Request["id"].ToInt64();

            string teamname = Request["teamname"];
            if (string.IsNullOrEmpty(teamname))
            {
                return Content(ReturnMsg(Enum_Return.失败, "请输入团队名称！", null));
            }
            if (teamname.Trim().Length > 20)
            {
                return Content(ReturnMsg(Enum_Return.失败, "团队名称不能超过20个字符！", null));
            }

            var model = db.team.Where(c => c.teamID == id).FirstOrDefault();
            if (model == null)
            {
                team teamModel = new team();
                teamModel.teamName = teamname;
                teamModel.userID = UserInfo.user_id;
                teamModel.addTime = DateTime.Now;
                teamModel.updateTime = DateTime.Now;
                db.team.Add(teamModel);
                int n = db.SaveChanges();
                if (n <= 0)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "添加失败！", null));
                }

                team_user teamUserModel = new team_user();
                teamUserModel.parentID = 0;
                teamUserModel.status = (int)Enum_Inviter_Status.正常;
                teamUserModel.teamID = teamModel.teamID;
                teamUserModel.userID = UserInfo.user_id;
                teamUserModel.addTime = DateTime.Now;
                teamUserModel.updateTime = DateTime.Now;
                db.team_user.Add(teamUserModel);
                int m = db.SaveChanges();
                if (m <= 0)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "添加失败！", null));
                }
            }
            else
            {
                var teamUserModel = db.team_user.Where(c => c.teamID == id && c.userID == UserInfo.user_id).FirstOrDefault();
                if (teamUserModel.parentID != 0)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "您不是创建人，无法修改！", null));
                }
                model.teamName = teamname;
                model.updateTime = DateTime.Now;
                int n = db.SaveChanges();
                if (n <= 0)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "更新失败！", null));
                }
            }

            return Content(ReturnMsg(Enum_Return.成功, "操作成功！", null));
        }

        public ActionResult SendInviter()
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }

            long teamID = Request["teamID"].ToInt64();
            if (teamID <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }

            var teamModel = db.team.Where(c => c.teamID == teamID).FirstOrDefault();
            if (teamModel == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }

            string userName = Request["userName"];
            if (string.IsNullOrEmpty(userName))
            {
                return Content(ReturnMsg(Enum_Return.失败, "用户信息不能为空！", null));
            }

            var userModel = db.sys_user.Where(c => c.user_name == userName || c.user_email == userName).FirstOrDefault();
            if (userModel == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该用户暂未注册本系统！", null));
            }
            if (userModel.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该用户属于游客，不能被邀请！", null));
            }
            if (userModel.user_id == UserInfo.user_id)
            {
                return Content(ReturnMsg(Enum_Return.失败, "不能自己邀请自己！", null));
            }
            var exist = db.team_user.Where(c => c.teamID == teamID && c.userID == userModel.user_id).FirstOrDefault();
            if (exist != null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该用户已经属于该团队成员了！", null));
            }

            team_user newTeamModel = new team_user();
            newTeamModel.parentID = UserInfo.user_id;
            newTeamModel.userID = userModel.user_id;
            newTeamModel.status = (int)Enum_Inviter_Status.邀请中;
            newTeamModel.teamID = teamID;
            newTeamModel.addTime = DateTime.Now;
            newTeamModel.updateTime = DateTime.Now;
            db.team_user.Add(newTeamModel);

            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "邀请失败，请重试！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "邀请成功，等待好友确认！", null));
        }

        public ActionResult TeamMember(long? id)
        {
            var list = db.vyw_team_user.Where(c => c.teamID == id).ToList();
            ViewData["list"] = list;

            ViewBag.UserID = UserInfo.user_id;
            return View();
        }

        #region 删除团队
        [HttpPost]
        public ActionResult TeamDelete(int? id)
        {
            if (UserInfo.user_role == (int)Enum_User.游客)
            {
                return Content(ReturnMsg(Enum_Return.失败, "对不起，您没有操作的权限！", null));
            }
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return Content(ReturnMsg(Enum_Return.失败, "参数错误！", null));
            }
            long teamID = Request["teamid"].ToInt64();

            var model = db.team_user.Where(c => c.userID == UserInfo.user_id && c.teamID == teamID).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }

            if (model.parentID == 0)//创建者
            {
                var list = db.team_user.Where(c => c.teamID == teamID).ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        db.Entry(item).State = System.Data.EntityState.Deleted;
                    }
                }

                var billinglist = db.billing_log.Where(c => c.team_id == id).ToList();
                if (billinglist.Count > 0)
                {
                    foreach (var item in billinglist)
                    {
                        item.is_delete = true;
                        item.update_time = DateTime.Now;
                    }
                }

                var teamModel = db.team.Where(c => c.teamID == teamID).FirstOrDefault();
                if (teamModel != null)
                {
                    db.Entry(teamModel).State = System.Data.EntityState.Deleted;
                }
            }
            else
            {
                db.Entry(model).State = System.Data.EntityState.Deleted;
            }
            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "删除失败，请重试！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "删除成功", null));
        }
        #endregion

        #region 邀请
        [HttpPost]
        public ActionResult Invition()
        {
            bool flag = Request["flag"].ToBoolean();

            long teamID = Request["teamID"].ToInt64();

            var model = db.team_user.Where(c => c.teamID == teamID && c.userID == UserInfo.user_id).FirstOrDefault();
            if (model == null)
            {
                return Content(ReturnMsg(Enum_Return.失败, "该数据不存在或已被删除！", null));
            }

            if (flag)
            {
                model.status = (int)Enum_Inviter_Status.正常;
            }
            else
            {
                db.Entry(model).State = System.Data.EntityState.Deleted;
            }

            model.updateTime = DateTime.Now;

            int n = db.SaveChanges();
            if (n <= 0)
            {
                return Content(ReturnMsg(Enum_Return.失败, "操作失败！", null));
            }

            return Content(ReturnMsg(Enum_Return.成功, "操作成功！", null));
        }
        #endregion
        #endregion

        #region 退出
        [HttpPost]
        public ActionResult Layout()
        {
            if (Session["billing_system_user_model"] != null)
            {
                HttpCookie ck1 = Request.Cookies["loginid"];
                HttpCookie ck2 = Request.Cookies["loginpwd"];

                if (ck1 != null && ck2 != null)
                {
                    ck1.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(ck1);

                    ck2.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(ck2);
                }

                Session.Remove("billing_system_user_model");
            }

            return Content(ReturnMsg(Enum_Return.成功, "退出成功！", null));
        }
        #endregion
    }
}
