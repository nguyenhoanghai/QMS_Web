using GPRO.Core.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_Website.App_Global;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class EvaluateController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(Q_Evaluate obj)
        {
            int result = 0;
            try
            {
                if (obj.Id == 0)
                    result = BLLEvaluate.Instance.Insert(AppGlobal.Connectionstring,obj);
                else
                    result = BLLEvaluate.Instance.Update(AppGlobal.Connectionstring,obj);
                if (result == 0)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new Error() {  Message="Lỗi SQL"});
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Add-Update", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetSelectList()
        {
            JsonDataResult.Records = BLLEvaluate.Instance.Gets(AppGlobal.Connectionstring);
            JsonDataResult.Result = "OK";
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult PagedList(string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLEvaluate.Instance.GetList(AppGlobal.Connectionstring,keyword, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get List  ", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        {
            bool result;
            try
            {
                result = BLLEvaluate.Instance.Delete(AppGlobal.Connectionstring,Id);
                if (!result)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new Error() {  Message="Lỗi SQL"});
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }


        public JsonResult GetList()
        { 
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLEvaluate.Instance.GetWithChild(AppGlobal.Connectionstring);
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Delete", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
    }
}