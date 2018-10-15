using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class UploadController : BaseController
    {
        public String UploadFile()
        {
            string imgPath = ConfigurationManager.AppSettings["imageFolder"].ToString();
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                string fname, returnName;
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1].Replace(' ', '-');
                }
                else
                    fname = file.FileName.Replace(' ', '-');
                returnName = (imgPath + fname);
                fname = Path.Combine(Server.MapPath(("~"+imgPath)), fname);
                file.SaveAs(fname);
                return file.FileName;
            }
            return "";
        }

        //Xóa Hình Ảnh
        public void DeleteFile(string filename)
        {
            try
            {
                string file = Path.Combine(Server.MapPath("~/Areas/Admin/Content/User/Images/"), filename); // lay ra full path cua hinh
                if (System.IO.File.Exists(file))  // kiem tra xem trong thu muc chua hinh co file hinh can xoa ko
                    System.IO.File.Delete(file);   //neu co, thi xoa hinh

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}