using GPRO.Core.Generic;
using GPRO.Core.Mvc;
using GPRO_QMS_Web.Areas.Admin.Controllers;
using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Controllers
{
    public class DangKyOnlineController : BaseController
    {
        public ActionResult FindInformation()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckServeInformation(int ticket)
        {
            return Json(BLLDailyRequire.Instance.CheckServeInformation(AppGlobal.Connectionstring, ticket));
        }

        
        public JsonResult GetServices()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLService.Instance.GetLookUp(AppGlobal.Connectionstring, false);
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        public JsonResult GetServiceInfo()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLServiceInfo.Instance.GetServiceInfo(AppGlobal.Connectionstring);
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult InsertServiceRequire(Q_DailyRequire model)
        {
            ResponseBase rs;
            try
            {
                rs = BLLServiceInfo.Instance.InsertServiceRequire(model, AppGlobal.Connectionstring);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                {
                    JsonDataResult.Result = "OK";
                    JsonDataResult.Data = rs.Data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }

        public JsonResult Find(string Phone, int Service)
        {
            ResponseBase rs;
            try
            {
                rs = BLLServiceInfo.Instance.Find(AppGlobal.Connectionstring, Phone, Service);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.AddRange(rs.Errors);
                }
                else
                {
                    JsonDataResult.Result = "OK";
                    JsonDataResult.Data = rs.Data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonDataResult);
        }
    }
}