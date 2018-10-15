using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class EmployeeController : BaseController
    { 
        public ActionResult Index()
        {
            Session["User"] =  new Employee() {  Name = "QMS"};
            return View();
        }

       // [HttpPost]
       // public JsonResult LoginAction(string userName, string password, string captcha)
        //{
        //    var user = BLLUser.Instance.Get(userName.Trim(), password.Trim());
        //    if (user != null)
        //    {
        //        string authId = Guid.NewGuid().ToString();
        //        Session["AuthID"] = authId;

        //        var cookie = new HttpCookie("AuthID");
        //        cookie.Value = authId;
        //        Response.Cookies.Add(cookie);

        //        Session["User"] = user;
        //        JsonDataResult.Result = "OK";
        //    }
        //    else
        //    {
        //        JsonDataResult.Result = "ERROR";
        //        JsonDataResult.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
        //    }
        //    return Json(JsonDataResult);
        //}

    }

    public class Employee
    {
        public string Name { get; set; }
    }
}