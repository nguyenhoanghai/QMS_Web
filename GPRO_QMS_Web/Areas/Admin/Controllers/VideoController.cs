using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class VideoController : BaseController
    { 
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Gets(string keyword = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLVideo.Instance.GetList(AppGlobal.Connectionstring, keyword, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Save(Q_Video  model)
        {
            ResponseBase res = new ResponseBase();
            try
            {
                int id = BLLVideo.Instance.AddFile(AppGlobal.Connectionstring, model);
                if (id == 0)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(res.Errors);
                }
                else
                    JsonDataResult.Result = "OK";

            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            ResponseBase result = new ResponseBase();
            try
            {
                //if (isAuthenticate)
                //{
                result.IsSuccess = BLLVideo.Instance.Delete(AppGlobal.Connectionstring, Id);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                }
                else
                {
                    JsonDataResult.Result = "OK";
                }

                // }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult GetSelectList()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLVideo.Instance.GetLookUp(AppGlobal.Connectionstring);
            }
            catch (Exception ex)
            {
                //add error
                JsonDataResult.Result = "ERROR";
            }
            return Json(JsonDataResult);
        }
    }
}