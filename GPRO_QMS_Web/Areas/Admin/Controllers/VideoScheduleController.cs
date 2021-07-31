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
    public class VideoScheduleController : BaseController
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Gets(string keyword = "", int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLVideoTemplate.Instance.GetList(AppGlobal.Connectionstring, keyword, jtStartIndex, jtPageSize, jtSorting);
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
        public JsonResult Save(Q_VideoTemplate  model)
        {
            ResponseBase res = new ResponseBase();
            try
            {
                res = BLLVideoTemplate.Instance.InsertOrUpdate(AppGlobal.Connectionstring, model);
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
                result.IsSuccess = BLLVideoTemplate.Instance.Delete(AppGlobal.Connectionstring, Id);
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



        public JsonResult _Gets(int tmpId, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = "")
        {
            try
            {
                var objs = BLLVideoTemplate_De.Instance.GetList(AppGlobal.Connectionstring, tmpId, jtStartIndex, jtPageSize, jtSorting);
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
        public JsonResult _Save(Q_VideoTemplate_De model)
        {
            ResponseBase res = new ResponseBase();
            try
            {
                res = BLLVideoTemplate_De.Instance.InsertOrUpdate(AppGlobal.Connectionstring, model);
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
        public JsonResult _Delete(int Id)
        {
            ResponseBase result = new ResponseBase();
            try
            {
                //if (isAuthenticate)
                //{
                result.IsSuccess = BLLVideoTemplate_De.Instance.Delete(AppGlobal.Connectionstring, Id);
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
    }
}