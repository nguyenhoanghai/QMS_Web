using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QMS_Website.Controllers
{
    public class ManHinhGoiSoController : Controller
    {
        // GET: ManHinhGoiSo
        public ActionResult Index()
        {
            GetConfig("MH3");
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
    }
}