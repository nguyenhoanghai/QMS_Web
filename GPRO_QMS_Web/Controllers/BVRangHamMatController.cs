using Newtonsoft.Json;
using QMS_System.Data.BLL.RangHamMat;
using QMS_Website.App_Global;
using QMS_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace QMS_Website.Controllers
{
    public class BVRangHamMatController : Controller
    { 
        //man hinh dieu tri
        public ActionResult LCD1()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("RHM_DT")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item; 
            return View();
        }

        public JsonResult GetDayInfo_BV(string counters, string services, int serType )
        {

            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var servicesArr = services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var ss = BLLRangHamMat.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, servicesArr, serType);
            var obj =  JsonConvert.SerializeObject(ss);
            return Json(obj);
        }

        //man hinh cho kham
        public ActionResult LCD2()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("RHM_KM")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            return View();
        }
    }
}