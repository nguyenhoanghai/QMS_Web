﻿using GPRO_QMS_Web.Areas.Admin.Controllers;
using QMS_System.Data.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GPRO_QMS_Web.Controllers
{
    public class DangNhapController : BaseController
    {
        public ActionResult Login()
        {
            return View();
        }

        public JsonResult Get(string UserName, string Password)
        {
            try
            {
                var user = BLLUser.Instance.Get(UserName.Trim().ToUpper(), Password.Trim().ToUpper());
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(UserName, false);
                    JsonDataResult.Result = "OK";
                }
                else
                {
                    JsonDataResult.Result = "ERROR";
                    JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Loi", Message = "Loi thong tin dang nhap khong dung" });
          
                }
            }
            catch (Exception ex)
            {
                JsonDataResult.Result = "ERROR";
                JsonDataResult.ErrorMessages.Add(new GPRO.Core.Mvc.Error() { MemberName = "Loi", Message = "Loi :" + ex.Message });
            }

            return Json(JsonDataResult);
        }
    }
}