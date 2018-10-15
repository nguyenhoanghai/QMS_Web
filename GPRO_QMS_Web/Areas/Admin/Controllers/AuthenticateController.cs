using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Areas.Admin.Controllers
{
    public class AuthenticateController : Controller
    {
        // GET: Admin/Authenticate
        public ActionResult Login()
        {
            return View();
        }
    }

    
}