using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//coi chung khac manespace
namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class UserMajorController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Gets(int userId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLUserMajor.Instance.GetList(AppGlobal.Connectionstring, userId, jtStartIndex, jtPageSize, jtSorting);
                JsonDataResult.Result = "OK";
                JsonDataResult.Records = objs;
                JsonDataResult.TotalRecordCount = objs.TotalItemCount;
                return Json(JsonDataResult);
            }
            catch (Exception ex)
            {
                throw ex;
                //CatchError(ex);
            }
        }

        [HttpPost]
        public JsonResult Save(UserMajorModel model)
        {
            ResponseBase rs;
            try
            { 
                rs = BLLUserMajor.Instance.InsertOrUpdate(AppGlobal.Connectionstring, model);
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
        public JsonResult Delete(int id)
        {
            try
            {
                if (BLLUserMajor.Instance.Delete(AppGlobal.Connectionstring, id))
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

    }
}