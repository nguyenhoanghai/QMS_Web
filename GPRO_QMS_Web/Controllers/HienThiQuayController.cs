using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_Website.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace GPRO_QMS_Web.Controllers
{
    public class HienThiQuayController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BenhVien()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("1")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }
        public ActionResult BenhVien_2()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("2")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public JsonResult GetDayInfo()
        {
            bool isTienthu = bool.Parse(ConfigurationManager.AppSettings["IsTIENTHU"].ToString());
            int maNVThuNgan = int.Parse(ConfigurationManager.AppSettings["ThuNgan"].ToString());
            var counterIds = ConfigurationManager.AppSettings["NotShowCounterIds"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();
            int[] distanceWarning = ConfigurationManager.AppSettings["DistanceWarning"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();

            var obj = JsonConvert.SerializeObject(BLLDailyRequire.Instance.GetDayInfo(isTienthu, maNVThuNgan, counterIds, distanceWarning));
            return Json(obj);
        }

        public JsonResult GetDayInfo_BV(string counters, string services)
        {
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var servicesArr =  services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();

            var obj = JsonConvert.SerializeObject(BLLDailyRequire.Instance.GetDayInfo(countersArr,servicesArr));
            return Json(obj);
        }

        public JsonResult GetConfig(int pageType)
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageType.ToString())).FirstOrDefault();
            return Json(cStudent.Element("Value").Value);
        }

        public JsonResult SaveBVConfig(string configStr, int pageType)
        {
            try
            {
                var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
                XDocument testXML = XDocument.Load(path);
                XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageType.ToString())).FirstOrDefault();
                cStudent.Element("Value").Value = configStr;
                testXML.Save(path);


                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                switch (pageType)
                {
                    case 1: // bv mau 1
                        config.AppSettings.Settings["ConfigHoanMy_1"].Value = configStr;
                        break;
                    case 2: // bv mau 2
                        config.AppSettings.Settings["ConfigHoanMy_2"].Value = configStr;
                        break;
                }

                config.Save();

                //ConfigurationManager.AppSettings.Remove("TD_BV");
                //ConfigurationManager.AppSettings.Add("TD_BV", title);
                //ConfigurationManager.AppSettings.Remove("TD_BV_CSS");
                //ConfigurationManager.AppSettings.Add("TD_BV_CSS", titlecss);
                //ConfigurationManager.AppSettings.Remove("TD_S_BV");
                //ConfigurationManager.AppSettings.Add("TD_S_BV", sub);
                //ConfigurationManager.AppSettings.Remove("TD_S_BV_CSS");
                //ConfigurationManager.AppSettings.Add("TD_S_BV_CSS", subcss);
                //ConfigurationManager.AppSettings.Remove("TD_BV_R1_CSS");
                //ConfigurationManager.AppSettings.Add("TD_BV_R1_CSS", r1css);
                //ConfigurationManager.AppSettings.Remove("TD_BV_R2_CSS");
                //ConfigurationManager.AppSettings.Add("TD_BV_R2_CSS", r2css);
                //ConfigurationManager.AppSettings.Remove("TD_BV_ROW");
                //ConfigurationManager.AppSettings.Add("TD_BV_ROW", row);
                //ConfigurationManager.AppSettings.Remove("TD_BV_TIME");
                //ConfigurationManager.AppSettings.Add("TD_BV_TIME", time);
                //ConfigurationManager.AppSettings.Remove("TD_BV_RUN_CSS");
                //ConfigurationManager.AppSettings.Add("TD_BV_RUN_CSS", runcss);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json("OK");
        }

        public ActionResult TienThu()
        {
            return View();
        }

        public ActionResult ManHinhCoVideo()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("3")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            //ViewData["user"] = User.Identity.Name;
            //ViewData["userInfo"] = BLLUser.Instance.Get(User.Identity.Name);
            //if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLVideoTemplate.Instance.GetPlaylist());
            //else
            //    return RedirectToAction("Login", "DangNhap");
        }

    }
}