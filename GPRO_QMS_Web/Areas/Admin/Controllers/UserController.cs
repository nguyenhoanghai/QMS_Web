using GPRO.Core.Generic;
using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public JsonResult GetList(string keyword, int searchBy = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLUser.Instance.GetList(AppGlobal.Connectionstring, keyword, searchBy, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Result = "OK";
                JsonDataResult.Records = objs;
                JsonDataResult.TotalRecordCount = objs.TotalItemCount; // TotalItemCount la mot property cua PagedList<> dem tong so item
                return Json(JsonDataResult);
            }
            catch (Exception ex)
            {
                throw ex;
                //CatchError(ex);
            }
        }

        [HttpPost]
        public JsonResult Save(Q_User nv)
        {
            ResponseBase rs;
            try
            {
                if (nv.Counters == null)
                    nv.Counters = "0";
                rs = BLLUser.Instance.CreateOrUpdate(AppGlobal.Connectionstring,nv);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = rs.Errors[0].MemberName, Message = rs.Errors[0].Message });
                }
                else
                    JsonDataResult.Result = "OK";
            }
            catch (Exception ex)
            {
                throw ex;
                //CatchError(ex);
            }
            return Json(JsonDataResult);
        }

        [HttpPost]
        public JsonResult Delete(int manv)
        {
            try
            {
                if (BLLUser.Instance.Delete(AppGlobal.Connectionstring,manv))
                    JsonDataResult.Result = "OK";
                else
                    JsonDataResult.Result = "ERROR";
                return Json(JsonDataResult);
            }
            catch (Exception ex)
            {
                throw ex;
                //CatchError(ex);
            }
        }

        public JsonResult GetById(int MaNV)
        {
            try
            {
                var nv = BLLUser.Instance.Get(AppGlobal.Connectionstring,MaNV);
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = nv;
                return Json(JsonDataResult);
            }
            catch (Exception ex)
            {
                throw ex;
                //CatchError(ex);
            }
        }

        public JsonResult GetUserSelect()
        {
            try
            {
                JsonDataResult.Result = "OK";
                JsonDataResult.Data = BLLUser.Instance.GetLookUp(AppGlobal.Connectionstring);
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Get Area", Message = "Lỗi: " + ex.Message });
            }
            return Json(JsonDataResult);
        }
    }
}