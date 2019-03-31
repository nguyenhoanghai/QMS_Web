
using Microsoft.AspNet.SignalR;
using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
 
using QMS_Website.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GPRO_QMS_Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public ActionResult Index_1_tieuchi()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public JsonResult GetNumer()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                    return Json("-1");
                else
                {
                    var obj = BLLDailyRequire.Instance.GetCurrentProcess(User.Identity.Name.Trim().ToUpper());
                    return Json((obj == null ? "0,0" : (obj.Id + "," +  obj.Data)));
                }
            }
            catch (Exception ex)
            {

               // throw ex;
            }
            return Json("");

        }

        public ActionResult DanhGia_TienThu()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public ActionResult DanhGia_HangKhong()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public ActionResult DG_Teso_ThangLong()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }
        
        public JsonResult Evaluate(string name ,string value, int num, string isUseQMS)
        {
            try
            {
         //if (string.IsNullOrEmpty(User.Identity.Name))
            //    return Json("-1");
            //else
                return Json(BLLUserEvaluate.Instance.Evaluate(name.Trim().ToUpper(), value, num, isUseQMS));
            }
            catch (Exception ex)
            { 
            }
            return Json("");
        }

     
        public JsonResult GetDateTime()
        { 
            var connection = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            connection.Clients.All.sendDateTimeToPage(DateTime.Now.ToString("dd/MM/yyyy|HH : mm"));
            return Json("");
        }
    }
}