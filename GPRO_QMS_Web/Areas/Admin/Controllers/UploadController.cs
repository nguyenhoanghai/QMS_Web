using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class UploadController : BaseController
    {
        public String UploadFile()
        {
            string imgPath = AppGlobal.userAvatarPath;
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
                fname = Path.Combine(imgPath, fname);
                file.SaveAs(fname);
                return file.FileName.Replace(' ', '-');
            }
            return "";
        }

        public void DeleteFile(string filename, string type)
        {
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    string path = "";
                    switch (type)
                    {
                        case "image": path = AppGlobal.userAvatarPath; break;
                        case "video": path = AppGlobal.videoPath; break;
                    }
                    string file = Path.Combine(path, filename); // lay ra full path cua hinh
                    if (System.IO.File.Exists(file))  // kiem tra xem trong thu muc chua hinh co file hinh can xoa ko
                        System.IO.File.Delete(file);   //neu co, thi xoa hinh
                }
            }
            catch (Exception ex)
            {
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadVideo()
        {
            string path = AppGlobal.videoPath;
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
                Guid g;
                g = Guid.NewGuid();
                var str = g.ToString().Replace('-', 'a') + file.FileName.Substring(file.FileName.LastIndexOf('.')) + "|" + fname;
                returnName = (path + str);

                
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                fname = Path.Combine(path, str.Split('|')[0]);

                if (BLLVideo.Instance.AddFile(AppGlobal.Connectionstring, new QMS_System.Data.Q_Video() { FileName = str.Split('|')[1], FakeName = str.Split('|')[0], Duration = new TimeSpan(0, 0, 0) }) > 0)
                {
                    file.SaveAs(fname);
                    return Json(returnName);
                }
            }
            return Json("");
        }



    }
}