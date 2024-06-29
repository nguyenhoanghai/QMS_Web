using QMS_System.Data.BLL;
using QMS_System.Data.BLL.TienThu;
using QMS_System.Data.Enum;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class Xe_TTController : Controller
    {
        public ActionResult ThongTinKH()
        {
            string templateName = (ConfigurationManager.AppSettings["TenMauQuangCao"] != null ? ConfigurationManager.AppSettings["TenMauQuangCao"].ToString() : "Quảng cáo");
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring, templateName));
        }

        public JsonResult GetCustInfo()
        { 
            string templateName = (ConfigurationManager.AppSettings["TenMauQuangCao"] != null ? ConfigurationManager.AppSettings["TenMauQuangCao"].ToString() : "Quảng cáo");
            TimeSpan tgShowInfo = TimeSpan.Parse(ConfigurationManager.AppSettings["TimeShowInfo"] != null ? ConfigurationManager.AppSettings["TimeShowInfo"].ToString() : "00:02:00");
            return Json(BLLKhachHangInfo.Instance.showThongTinKH(AppGlobal.Connectionstring, templateName, tgShowInfo,new List<int>() { }));

        }
    }
}