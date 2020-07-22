using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.BLL.HuuNghi;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QMS_Website.Controllers
{
    public class HuuNghiController : Controller
    {
        // GET: HuuNghi
        public ActionResult LCD1()
        {
            GetConfig("MH5");
            return View();
        }

        public ActionResult LCD2()
        {
            GetConfig("MH4");
            return View();
        }

        private void GetConfig(string pageId)
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageId)).FirstOrDefault();
            if (cStudent != null)
                ViewData["config"] = cStudent.Element("Value").Value;
            else
                ViewData["config"] = "{}";
        }

        public JsonResult GetDayInfo(bool GetKL, string counters)
        {
            int userId = int.Parse(ConfigurationManager.AppSettings["UserId"].ToString());
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            int equipCode = int.Parse(ConfigurationManager.AppSettings["EquipCode"].ToString());
            var obj = JsonConvert.SerializeObject(BLLHuuNghi.Instance.GetCounterDayInfo_Web(AppGlobal.Connectionstring, userId, equipCode, countersArr));
            return Json(obj);
        }

    }
}