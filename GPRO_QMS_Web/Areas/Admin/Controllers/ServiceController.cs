using GPRO_QMS_Web.Areas.Admin.Controllers;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class ServiceController : BaseController
    { 
        public ActionResult Index()
        {
            return View();
        }

       
        public JsonResult Gets(string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLService.Instance.GetList(AppGlobal.Connectionstring, keyword, jtStartIndex, jtPageSize, jtSorting);
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
        public JsonResult Save(ServiceModel model)
        {
            ResponseBase res = new ResponseBase();
            try
            {
                res = BLLService.Instance.InsertOrUpdate(AppGlobal.Connectionstring, model);
                if (!res.IsSuccess)
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
                result.IsSuccess = BLLService.Instance.Delete(AppGlobal.Connectionstring, Id);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                }
                else
                    result.IsSuccess = true;
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
                JsonDataResult.Data = BLLService.Instance.GetLookUp(AppGlobal.Connectionstring, false);
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