﻿using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class DanhGiaController : Controller
    {
         
        public ActionResult TenButton()
        {
            ViewData["user"] = User.Identity.Name;
            if (!string.IsNullOrEmpty(User.Identity.Name))
                return View(BLLUser.Instance.Get(AppGlobal.Connectionstring, User.Identity.Name));
            else
                return RedirectToAction("Login", "DangNhap");
        }
    }
}