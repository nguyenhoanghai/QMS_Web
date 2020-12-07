using GPRO.Core.Hai;
using QMS_System.Data.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Xml;

namespace QMS_Website.Controllers
{
    public class SQLConnectController : Controller
    {
        public string conString = "";

        public ActionResult Index()
        {
            try
            {
                ModelSelectItem item = new ModelSelectItem();
                string info = BaseCore.Instance.GetStringConnectInfo(Server.MapPath("~/Config_XML") + "\\DATA.XML");
                if (!string.IsNullOrEmpty(info))
                {
                    var infos = info.Split(',');
                    item.Data1 = infos[0]; //txtServerName.Text
                    item.Data2 = infos[1]; //cbDatabases.Text
                    item.Data3 = infos[2];//txtLogin.Text
                    item.Data4 = infos[3];//txtPass.Text
                    item.Data5 = bool.Parse(infos[4]);
                }
                ViewBag.info = item;
                ViewBag.dbs = getDatabases(item.Data1, item.Data3, item.Data4, item.Data5);
            }
            catch (Exception)
            { }
            return View();
        }

        public string getDatabases(string ip, string uname, string pass, bool isAuthen)
        {
            string _name = "";
            try
            {
                if (checkValid(ip, uname, pass, isAuthen))
                {
                    var conn = new SqlConnection(conString);
                    conn.Open();
                    var ds = new DataSet();
                    string query = "select name from sysdatabases";
                    var da = new SqlDataAdapter(query, conn);
                    da.Fill(ds, "databasenames");
                    // this.cbDatabases.DataSource = ds.Tables["databasenames"];
                    // this.cbDatabases.DisplayMember = "name";
                    var _tb = ds.Tables["databasenames"];
                    for (int i = 0; i < _tb.Rows.Count; i++)
                    {
                        if (i > 0)
                            _name += ",";
                        _name += _tb.Rows[i][0];

                    }

                }
            }
            catch
            {
                //  this.cbDatabases.DataSource = null;
            }
            return _name;
        }

        private bool checkValid(string ip, string uname, string pass, bool isAuthen)
        {
            bool isPass = false;

            // Open connection to the database 
            if (!string.IsNullOrEmpty(ip))
            {
                if (isAuthen)
                {
                    conString = string.Concat(new string[]
                     {
                    "Server = ",
                    ip,
                    ";Trusted_Connection=true;",
                     });
                    isPass = true;
                }
                else
                {
                    conString = string.Concat(new string[]
                    {
                    "Server = ",
                    ip,
                    " ; Uid = ",
                    uname,
                    " ;Pwd= ",
                    pass
                    });
                    if (!string.IsNullOrEmpty(uname) &&
                        !string.IsNullOrEmpty(pass))
                    {
                        isPass = true;
                    }
                }
            }
            return isPass;
        }

        public JsonResult SaveConfig(string ip, string uname, string pass, bool isAuthen, string dbname)
        {
            bool result = false;
            try
            {
                string filepath = (Server.MapPath("~/Config_XML") + "\\DATA.XML");
                if (!System.IO.File.Exists(filepath))
                    System.IO.File.Create(filepath);

                XmlDocument xmlDocument = new XmlDocument();
                XmlNode newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDocument.AppendChild(newChild);
                XmlNode xmlNode = xmlDocument.CreateElement("String_Connect");
                xmlDocument.AppendChild(xmlNode);

                XmlNode xmlNode2 = xmlDocument.CreateElement("SQLServer");
                XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("id");
                xmlAttribute.Value = "1";
                xmlNode2.Attributes.Append(xmlAttribute);
                xmlNode.AppendChild(xmlNode2);

                XmlNode xmlNode3 = xmlDocument.CreateElement("Server_Name");
                xmlNode3.AppendChild(xmlDocument.CreateTextNode(EncryptionHelper.Instance.Encrypt(ip)));
                xmlNode2.AppendChild(xmlNode3);

                XmlNode xmlNode4 = xmlDocument.CreateElement("Database");
                xmlNode4.AppendChild(xmlDocument.CreateTextNode(EncryptionHelper.Instance.Encrypt(dbname)));
                xmlNode2.AppendChild(xmlNode4);

                XmlNode xmlNode5 = xmlDocument.CreateElement("User");
                xmlNode5.AppendChild(xmlDocument.CreateTextNode(EncryptionHelper.Instance.Encrypt(uname)));
                xmlNode2.AppendChild(xmlNode5);

                XmlNode xmlNode6 = xmlDocument.CreateElement("Password");
                xmlNode6.AppendChild(xmlDocument.CreateTextNode(EncryptionHelper.Instance.Encrypt(pass)));
                xmlNode2.AppendChild(xmlNode6);

                XmlNode xmlNode7 = xmlDocument.CreateElement("Window_Authenticate");
                xmlNode7.AppendChild(xmlDocument.CreateTextNode(isAuthen.ToString()));
                xmlNode2.AppendChild(xmlNode7);

                xmlDocument.Save(filepath);
                //Environment.Exit(0);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Json(result);
        }
    }
}