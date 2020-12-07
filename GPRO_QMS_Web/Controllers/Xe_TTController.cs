using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_Website.App_Global;
using System.Configuration;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class Xe_TTController : Controller
    {
        public ActionResult ThongTinKH()
        {
            string templateName = (ConfigurationManager.AppSettings["PrinterId"] != null ? ConfigurationManager.AppSettings["PrinterId"].ToString() : "Quảng cáo");
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring, templateName));
        }

        public JsonResult GetCustInfo()
        {
            var ss = BLLCounterSoftRequire.Instance.Gets(AppGlobal.Connectionstring, (int)eCounterSoftRequireType.ShowCustDetail_TT);
            if (ss.Count > 0)
            {
                return Json(ss[ss.Count-1].Content);
            }
            return Json("NO");

        }
    }
}