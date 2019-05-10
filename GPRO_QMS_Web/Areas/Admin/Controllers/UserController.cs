using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GPRO_QMS_Web.Controllers;
using GPRO.Core.Mvc;
using GPRO.Core.Generic;
using PagedList;
using QMS_System.Data.BLL;
using QMS_System.Data;
using QMS_System.Data.Model;

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
                var objs = BLLUser.Instance.GetList(keyword, searchBy, jtStartIndex, jtPageSize, jtSorting);
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
                rs = BLLUser.Instance.CreateOrUpdate(nv);
                if (!rs.IsSuccess)
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error(){ MemberName =rs.Errors[0].MemberName, Message = rs.Errors[0].Message } );
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
                ResponseBase responseResult;
                BLLUser.Instance.Delete(manv);
                //responseResult = BLLUser.Instance.Delete(manv);
                //if (!responseResult.IsSuccess)
                //{
                //    JsonDataResult.Result = "ERROR";
                //    JsonDataResult.ErrorMessages.AddRange(responseResult.Errors);
                //}
                //else
                //{
                JsonDataResult.Result = "OK";
                // }
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
                var nv = BLLUser.Instance.Get(MaNV);
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
                JsonDataResult.Data = BLLUser.Instance.GetLookUp();
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