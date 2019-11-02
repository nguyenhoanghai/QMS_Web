using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GPRO_QMS_Web.Areas.Admin.Controllers;   
using GPRO.Core.Mvc;
using GPRO.Core.Generic; 
using PagedList;
using QMS_System.Data.BLL;
using Newtonsoft.Json;
using QMS_Website.App_Global;

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
            return Json( BLLDailyRequire.Instance.CheckServeInformation(AppGlobal.Connectionstring, ticket) );
        }

        //public JsonResult GetServiceSelect()
        //{
        //    try
        //    {
        //        JsonDataResult.Result = "OK";
        //        JsonDataResult.Data = BLLServiceInfo.Instance.GetServiceSelect();
        //    }
        //    catch (Exception ex)
        //    {
        //        JsonDataResult.Result = "ERROR";
        //        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
        //    }
        //    return Json(JsonDataResult);
        //}

        //public JsonResult GetServiceInfo()
        //{
        //    try
        //    {
        //        JsonDataResult.Result = "OK";
        //        JsonDataResult.Data = BLLServiceInfo.Instance.GetServiceInfo();
        //    }
        //    catch (Exception ex)
        //    {
        //        JsonDataResult.Result = "ERROR";
        //        JsonDataResult.ErrorMessages.Add(new Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
        //    }
        //    return Json(JsonDataResult);
        //}

        //[HttpPost]
        //public JsonResult InsertServiceRequire(YEUCAU yc)
        //{
        //    ResponseBase rs;
        //    try
        //    {
        //        rs = BLLServiceInfo.Instance.InsertServiceRequire(yc);
        //        if (!rs.IsSuccess)
        //        {
        //            JsonDataResult.Result = "ERROR";
        //            JsonDataResult.ErrorMessages.AddRange(rs.Errors);
        //        }
        //        else
        //        {
        //            JsonDataResult.Result = "OK";
        //            JsonDataResult.Data = rs.Data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(JsonDataResult);
        //}

        //public JsonResult Find(string Phone, int Service)
        //{
        //    ResponseBase rs;
        //    try
        //    {
        //        rs = BLLServiceInfo.Instance.Find(Phone, Service);
        //        if (!rs.IsSuccess)
        //        {
        //            JsonDataResult.Result = "ERROR";
        //            JsonDataResult.ErrorMessages.AddRange(rs.Errors);
        //        }
        //        else
        //        {
        //            JsonDataResult.Result = "OK";
        //            JsonDataResult.Data = rs.Data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(JsonDataResult);
        //}
    }
}