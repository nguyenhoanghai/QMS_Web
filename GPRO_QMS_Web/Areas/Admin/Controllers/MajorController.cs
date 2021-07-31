using GPRO.Core.Generic;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class MajorController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Gets(string keyword = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLMajor.Instance.GetList(AppGlobal.Connectionstring, keyword, jtStartIndex, jtPageSize, jtSorting);
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
        public JsonResult Save(MajorModel model)
        {
            ResponseBase res = new ResponseBase();
            try
            {
                res = BLLMajor.Instance.InsertOrUpdate(AppGlobal.Connectionstring, model);
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
                result.IsSuccess = BLLMajor.Instance.Delete(AppGlobal.Connectionstring, Id);
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
                JsonDataResult.Data = BLLMajor.Instance.GetLookUp(AppGlobal.Connectionstring);
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