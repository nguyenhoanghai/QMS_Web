using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_Website.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class ServiceApiController : ApiController
    {
        [HttpGet]
        public string GetUserAvatar(string userName)
        {
            var img = BLLUser.Instance.GetUserAvatar(userName);
            if (!string.IsNullOrEmpty(img))
                return (ConfigurationManager.AppSettings["imageFolder"].ToString() + img);
            return null;
        }

        [HttpGet]
        public UserModel GetUserInfo(string userName)
        {
            var user = BLLUser.Instance.GetByUserName(userName);
            if (user != null &&  !string.IsNullOrEmpty(user.Avatar))
                user.Avatar =  (ConfigurationManager.AppSettings["imageFolder"].ToString() + user.Avatar);
            return user;
        }


        [HttpGet]
        public ModelSelectItem GetNumber(string userName)
        {
            return BLLDailyRequire.Instance.GetCurrentProcess(userName);
        }

        [HttpGet]
        public ResponseBaseModel Evaluate(string username, string value, int num, string isUseQMS)
        {
            return BLLUserEvaluate.Instance.Evaluate(username.Trim().ToUpper(), value, num, isUseQMS);
        }

        [HttpGet]
        public string ResetDayInfo()
        {
            var connection = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            bool isTienthu = bool.Parse(ConfigurationManager.AppSettings["IsTIENTHU"].ToString());
            int maNVThuNgan = int.Parse(ConfigurationManager.AppSettings["ThuNgan"].ToString());
            var counterIds = ConfigurationManager.AppSettings["NotShowCounterIds"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();
            int[] distanceWarning = ConfigurationManager.AppSettings["DistanceWarning"].ToString().Split('|').Select(x => Convert.ToInt32(x)).ToArray();

            //var obj = JsonConvert.SerializeObject();
            connection.Clients.All.sendDayInfoToPage(BLLDailyRequire.Instance.GetDayInfo(isTienthu, maNVThuNgan, counterIds, distanceWarning));
            return string.Empty;
        }

        [HttpGet]
        public ResponseBase PrintNewTicket(string TenBenhNhan, string MaBenhNhan, string MaPhongKham, string STT_PhongKham)
        {
            return BLLDailyRequire.Instance.API_PrintNewTicket(TenBenhNhan, null, null, MaBenhNhan, MaPhongKham, STT_PhongKham);
        }

        [HttpGet]
        public ResponseBase UpdateTicketInfo(string TenBenhNhan, string MaBenhNhan, string STT_PhongKham)
        {
            return BLLDailyRequire.Instance.API_UpdateTicketInfo(TenBenhNhan, MaBenhNhan, STT_PhongKham);
        }

        [HttpGet]
        public List<ModelSelectItem> GetServices()
        {
            return BLLService.Instance.GetLookUp();
        }

        [HttpGet]
        public ResponseBase PrintTicket(string soxe, string thoigian, int dichvuId)
        {
            var rs = new ResponseBase();
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 " + thoigian, "dd/MM/yyyy HH:mm:ss", provider);

            var require = new PrinterRequireModel()
            {
                PrinterId = printerId,
                SoXe = soxe,
                ServeTime = newDate,
                ServiceId = dichvuId
            };
            rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.PrintTicket);
             return rs;
        }

    }
}
