using GPRO.Core.Mvc;
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
    public class EvaluateDetailController : BaseController
    {
        public JsonResult Save( EvaluateDetailModel obj)
        {
            ResponseBase result;
            try
            {
                result = BLLEvaluateDetail.Instance.InsertOrUpdate(AppGlobal.Connectionstring,obj);
                if (!result.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error(){ MemberName =result.Errors[0].MemberName, Message = result.Errors[0].Message } );
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Add-Update", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult PagedList(int type, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLEvaluateDetail.Instance.Gets(AppGlobal.Connectionstring,type, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Records = objs;
                JsonDataResult.Result = "OK";
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Get List  ", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int Id)
        { 
            try
            { 
                if (! BLLEvaluateDetail.Instance.Delete(AppGlobal.Connectionstring,Id)   )
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() {  Message ="Lỗi thực hiện", MemberName="Lỗi"});
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Delete", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
    }
}