﻿using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_Website.App_Global;
using QMS_Website.Models;
using System;
using System.Collections.Generic;
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
                return View(BLLUser.Instance.Get(AppGlobal.Connectionstring,User.Identity.Name));
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
                return View(BLLUser.Instance.Get(AppGlobal.Connectionstring,User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }

        public JsonResult GetDayInfo()
        {
            bool isTienthu = bool.Parse(ConfigurationManager.AppSettings["IsTIENTHU"].ToString());
            int maNVThuNgan = int.Parse(ConfigurationManager.AppSettings["ThuNgan"].ToString());
            var counterIds = ConfigurationManager.AppSettings["NotShowCounterIds"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();
            int[] distanceWarning = ConfigurationManager.AppSettings["DistanceWarning"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();

            var obj = JsonConvert.SerializeObject(BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring,isTienthu, maNVThuNgan, counterIds, distanceWarning));
            return Json(obj);
        }

        //public JsonResult GetDayInfo_BV(string counters, string services)
        //{
        //    var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
        //    var servicesArr = services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
        //    var ss = BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, servicesArr);
        //    var obj = JsonConvert.SerializeObject(ss);
        //    return Json(obj);
        //}

        public JsonResult GetDayInfo_BV(string counters, string services,int userId, bool getLastFiveNumbers)
        {
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var servicesArr = services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var ss = BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, servicesArr,userId,   getLastFiveNumbers);
            var obj = JsonConvert.SerializeObject(ss);
            return Json(obj);
        }

        public JsonResult GetConfig(string pageType)
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageType)).FirstOrDefault();
            return Json(cStudent.Element("Value").Value);
        }
        
        public JsonResult SaveBVConfig(string configStr, string pageType)
        {
            try
            {
                var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
                XDocument testXML = XDocument.Load(path);
                XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageType )).FirstOrDefault();
                if (cStudent != null)
                {
                    cStudent.Element("Value").Value = configStr;
                    testXML.Save(path);
                } 
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

        public ActionResult TienThu_CoVideo()
        {
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring));
        }

        public ActionResult VinhCat()
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
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring));
            //else
            //    return RedirectToAction("Login", "DangNhap");
        }

        public ActionResult ManHinhCoVideo_Doc()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("6")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring)); 
        }

        /// <summary>
        /// BV RHM khoa covid
        /// </summary>
        /// <returns></returns>
        public ActionResult ManHinhCoVideo_Doc_Covid()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("7")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            return View(BLLVideoTemplate.Instance.GetPlaylist(AppGlobal.Connectionstring));
        }

        public ActionResult LCD1()
        { 
            return View();
        }

        private void GetConfig(int pageId)
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals(pageId)).FirstOrDefault();
            if (cStudent != null)
                ViewData["config"] = cStudent.Element("Value").Value;
            else
                ViewData["config"] = "{}";
        }
    }
}