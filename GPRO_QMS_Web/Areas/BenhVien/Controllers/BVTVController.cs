using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using QMS_Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace QMS_Website.Areas.BenhVien.Controllers
{
    public class BVTVController : Controller
    {
        // GET: BenhVien/TV
        public ActionResult MH1()
        {
            var path = Server.MapPath(@"~\Config_XML\hien_thi_quay_config.xml");
            XDocument testXML = XDocument.Load(path);
            XElement cStudent = testXML.Descendants("View").Where(c => c.Attribute("ID").Value.Equals("1")).FirstOrDefault();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            BV_ConfigModel item = serializer.Deserialize<BV_ConfigModel>(cStudent.Element("Value").Value);
            ViewData["config"] = item;
            ViewData["user"] = User.Identity.Name;
            return View();
            //if (!string.IsNullOrEmpty(User.Identity.Name))
            //    return View(BLLUser.Instance.Get(AppGlobal.Connectionstring, User.Identity.Name));
            //else
            //    return RedirectToAction("Login", "DangNhap");
        }
    }
}