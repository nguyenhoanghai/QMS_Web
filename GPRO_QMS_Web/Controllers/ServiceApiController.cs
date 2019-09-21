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
        public AndroidModel GetAndroidInfo(string userName, int getSTT, int getSMS, int getUserInfo)
        {
            var info = BLLUserEvaluate.Instance.GetInfoForAndroid(userName,getSTT,getSMS,getUserInfo);
            if (getUserInfo==1 && info.UserInfo != null && !string.IsNullOrEmpty(info.UserInfo.Avatar))
                info.UserInfo.Avatar = (ConfigurationManager.AppSettings["imageFolder"].ToString() + info.UserInfo.Avatar);
            return info;
        }

        [HttpGet]
        public ModelSelectItem GetNumber(string userName)
        {
            return BLLDailyRequire.Instance.GetCurrentProcess(userName);
        }

        [HttpGet]
        public ResponseBaseModel Evaluate(string username, string value, int num, string isUseQMS)
        { 
            return BLLUserEvaluate.Instance.Evaluate(username.Trim().ToUpper(), value, num, isUseQMS );
        }

        [HttpGet]
        public List<string> GetRequireSendSMS( )
        {
            return BLLUserEvaluate.Instance.GetRequireSendSMSForAndroid( );
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
        public ResponseBase PrintNewTicket(string MaPhongKham, string thoigian)
        {
            var rs = new ResponseBase();
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 " + thoigian, "dd/MM/yyyy HH:mm:ss", provider);

            var require = new PrinterRequireModel()
            {
                PrinterId = printerId,
                SoXe = "",
                ServeTime = newDate,
                ServiceId = int.Parse( MaPhongKham)
            };
            rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.PrintTicket);
            return rs;

           // return BLLDailyRequire.Instance.PrintNewTicket(int.Parse(MaPhongKham ), 0, 0, DateTime.Now,null,null, "","",null,"","","","");
           // return BLLDailyRequire.Instance.API_PrintNewTicket("", null, null, "", MaPhongKham, "");
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

        [HttpGet]
        public ResponseBase CounterEvent(string counterId,string action, string param)
        {
            var rs = new ResponseBase(); 
            string content = "AA," + counterId + "," + action + ","+param;
            rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(content, (int)eCounterSoftRequireType.CounterEvent);
            return rs;
        }

        public string GetTicketStatus(int ticket)
        { 
                return  (BLLDailyRequire.Instance.CheckServeInformation(ticket).Data_1);
             
        }

        [HttpGet]
        public List<ModelSelectItem> GetEquipments ()
        {
            return BLLEquipment.Instance.GetsEquipments();
        }
    }
}
