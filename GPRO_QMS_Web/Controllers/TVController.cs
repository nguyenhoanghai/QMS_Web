using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class TVController : Controller
    {
        public ActionResult TV1()
        {
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring));
        }

        public JsonResult GetTV1Info(string counters)
        {
            var objs = BLLTivi.Instance.Gets(App_Global.AppGlobal.Connectionstring, counters.Split(',').Select(x => Convert.ToInt32(x)).ToList());
            return Json(objs);
        }
    }
}