using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Newtonsoft.Json;
using Utility;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        public billing_systemEntities db = new billing_systemEntities();
        public string ReturnMsg(Enum_Return type, string msg, string data)
        {
            return JsonConvert.SerializeObject(new { status = (int)type, msg = msg, data = data });
        }
    }
}
