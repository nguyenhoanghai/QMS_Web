using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class ReceiverSMSController : BaseController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Save(Q_RecieverSMS obj)
        {
            ResponseBase rs;
            try
            {
                rs = BLLRecieverSMS.Instance.InsertOrUpdate(obj);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { Message = rs.Errors[0].Message, MemberName = rs.Errors[0].MemberName });
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

        public JsonResult GetList(string keyword, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLRecieverSMS.Instance.Gets(keyword, jtStartIndex, jtPageSize, jtSorting);
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

        public JsonResult Delete(int Id)
        {
            ResponseBase rs;
            try
            {
                rs = BLLRecieverSMS.Instance.Delete(Id);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { Message = rs.Errors[0].Message, MemberName = rs.Errors[0].MemberName });
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