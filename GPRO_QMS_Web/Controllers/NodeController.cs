using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using QMS_Website.Cors;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace QMS_Website.Controllers
{
     [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
   // [AllowCrossSiteJson]
    public class NodeController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;
        public SqlConnection sqlconnection = AppGlobal.sqlConnection;
        public List<ModelSelectItem> GetServices (int id)
        {
            return BLLService.Instance.GetLookUp(connectString, false);
        }

        public ViewModel GetDayInfo_BV(string counters, string services, int userId, int getLastFiveNumbers)
        {
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var servicesArr = services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var ss = BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, servicesArr, userId, (getLastFiveNumbers == 1));
            // var obj = JsonConvert.SerializeObject(ss);
            // return Json(obj);
            return ss;
        }

    }
}
