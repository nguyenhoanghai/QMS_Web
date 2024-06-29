using GPRO.Core.Mvc;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using QMS_System.Data;
using QMS_System.Data.BLL;
using QMS_System.Data.BLL.VietThaiQuan;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_System.ThirdApp.Enum;
using QMS_Website.App_Global;
using QMS_Website.Helper;
using QMS_Website.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    //[EnableCors(origins: "http://localhost:3000/", headers:"*", methods:"*")]
    //[AllowCrossSite]
    public class ServiceApiController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;
        public string sqlconnectString = AppGlobal.sqlConnectionString;
        public SqlConnection sqlconnection = AppGlobal.sqlConnection;
        [HttpGet]
        public string GetUserAvatar(string userName)
        {
            var img = BLLUser.Instance.GetUserAvatar(connectString, userName);
            if (!string.IsNullOrEmpty(img))
                return (ConfigurationManager.AppSettings["imageFolder"].ToString() + img);
            return null;
        }

        [HttpGet]
        public UserModel GetUserInfo(string userName)
        {
            var user = BLLUser.Instance.GetByUserName(connectString, userName);
            if (user != null && !string.IsNullOrEmpty(user.Avatar))
                user.Avatar = (ConfigurationManager.AppSettings["imageFolder"].ToString() + user.Avatar);
            return user;
        }

        [HttpGet]
        public UserModel GetUserInfo(int userId)
        {
            var user = BLLUser.Instance.Get(connectString, userId);
            if (user != null && !string.IsNullOrEmpty(user.Avatar))
                user.Avatar = (ConfigurationManager.AppSettings["imageFolder"].ToString() + user.Avatar);
            return user;
        }

        [HttpGet]
        public AndroidModel GetAndroidInfo(string userName, int getSTT, int getSMS, int getUserInfo)
        {
            var info = BLLUserEvaluate.Instance.GetInfoForAndroid(connectString, userName, getSTT, getSMS, getUserInfo);
            if (getUserInfo == 1 && info.UserInfo != null && !string.IsNullOrEmpty(info.UserInfo.Avatar))
                info.UserInfo.Avatar = (ConfigurationManager.AppSettings["imageFolder"].ToString() + info.UserInfo.Avatar);
            return info;
        }

        [HttpGet]
        public AndroidModel GetAndroidInfo2(int matb, int getSTT, int getSMS, int getUserInfo)
        {
            var info = BLLUserEvaluate.Instance.GetInfoForAndroid(connectString, matb, getSTT, getSMS, getUserInfo);
            if (getUserInfo == 1 && info.UserInfo != null && !string.IsNullOrEmpty(info.UserInfo.Avatar))
                info.UserInfo.Avatar = (ConfigurationManager.AppSettings["imageFolder"].ToString() + info.UserInfo.Avatar);
            return info;
        }

        [HttpGet]
        public AndroidModel GetAndroidInfo3(string userName, int matb, int getSTT, int getSMS, int getUserInfo)
        {
            var counterDayInfo = BLLLoginHistory.Instance.GetForHome(sqlconnectString, sqlconnection, userName, matb, DateTime.Now, 0);
            AndroidModel androidModel = new AndroidModel();
            androidModel.TicketNumber = counterDayInfo.CurrentTicket;
            androidModel.TotalWaiting = counterDayInfo.TotalWating;
            androidModel.CounterWaitings = counterDayInfo.CounterWaitingTickets;
            androidModel.UserId = counterDayInfo.UserId;

            androidModel.HasEvaluate = counterDayInfo.HasEvaluate;
            androidModel.Status = counterDayInfo.Status;
            androidModel.TicketNumber = counterDayInfo.CurrentTicket;
            androidModel.UserInfo = new UserModel() { UserName = counterDayInfo.UserName };
            return androidModel;
        }

        [HttpGet]
        public ModelSelectItem GetNumber(string userName)
        {
            return BLLDailyRequire.Instance.GetCurrentProcess(connectString, userName);
        }

        [HttpGet]
        public ModelSelectItem GetSTT_Username(int maTB)
        {
            return BLLDailyRequire.Instance.GetSTT_UserName(connectString, maTB);
        }

        [HttpGet]
        public ResponseBaseModel Evaluate(string username, string value, int num, string isUseQMS, string comment)
        {
            return BLLUserEvaluate.Instance.Evaluate(connectString, username.Trim().ToUpper(), value, num, isUseQMS, comment);
        }

        [HttpGet]
        public ResponseBaseModel Evaluate2(int matb, string value, int num, string isUseQMS, string comment)
        {
            return BLLUserEvaluate.Instance.Evaluate(connectString, matb, value, num, isUseQMS, comment);
        }

        [HttpGet]
        public List<string> GetRequireSendSMS()
        {
            return BLLUserEvaluate.Instance.GetRequireSendSMSForAndroid(connectString);
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
            connection.Clients.All.sendDayInfoToPage(BLLDailyRequire.Instance.GetDayInfo(connectString, isTienthu, maNVThuNgan, counterIds, distanceWarning));
            return string.Empty;
        }

        [HttpGet]
        public ResponseBase PrintNewTicket(string TenBenhNhan, string MaBenhNhan, string MaPhongKham, string STT_PhongKham)
        {
            return InPhieu(new Q_DailyRequire() { CustomerName = TenBenhNhan, MaBenhNhan = MaBenhNhan, MaPhongKham = MaPhongKham, STT_PhongKham = STT_PhongKham }, string.Empty);
        }

        private ResponseBase InPhieu(Q_DailyRequire model, string maKhoa)
        {
            return BLLDailyRequire.Instance.API_PrintNewTicket(connectString, model, maKhoa);
        }

        [HttpGet]
        public bool CapPhieu(string madichvu, int stt, string ngaycap )
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
           // giocap = giocap.Replace("/", ":");
           // ngaycap = ngaycap + " " + giocap;
            var newDate = DateTime.ParseExact(ngaycap, "dd/MM/yyyy HH:mm:ss", provider);
            return BLLDailyRequire.Instance.API_PrintNewTicket(connectString, madichvu, stt, newDate).IsSuccess;
        }


        [HttpGet]
        public ResponseBase PrintNewTicket(string MaPhongKham, string thoigian)
        {
            var rs = new ResponseBase();
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());

            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 " + thoigian, "dd/MM/yyyy HH:mm:ss", provider);


            var serObj = BLLService.Instance.Get(connectString, int.Parse(MaPhongKham));
            if (serObj != null)
            {
                string checkTime = "0";
                var cf = BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.CheckTimeBeforePrintTicket);
                var now = DateTime.Now;
                if (!string.IsNullOrEmpty(cf))
                    checkTime = cf;
                bool _continue = true;
                if (checkTime == "1" && BLLServiceShift.Instance.CheckTime(connectString, serObj.Id, now))
                    _continue = false;
                if (_continue)
                {
                    int _printType = 1;
                    int.TryParse(ConfigurationManager.AppSettings["PrintType"].ToString(), out _printType);
                    PrinterRequireModel require = null;
                    var result = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, serObj.StartNumber, 0, now, _printType, newDate.TimeOfDay, "", "", 0, "", "", "", "", "", "");
                    if (result.IsSuccess)
                    {
                        string _tenQuay = "";
                        string PrintAllTable = BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.PrintAllTable);

                        if (!string.IsNullOrEmpty(PrintAllTable) && PrintAllTable == "1")
                            _tenQuay = rs.Data_2;
                        else
                        {
                            if (rs.Data_4 != null)
                            {
                                var tqArr = (string[])rs.Data_4;
                                if (tqArr != null && tqArr.Length > 0)
                                    _tenQuay = tqArr[0];
                            }
                        }

                        //_tenQuay = "Quầy " + MaPhongKham;
                        rs.Data_4 = _tenQuay;
                        require = new PrinterRequireModel()
                        {
                            newNumber = ((int)result.Data + 1),
                            oldNumber = (int)result.Data,
                            TenDichVu = serObj.Name,
                            TenQuay = _tenQuay,// result.Data_2,
                            MajorId = (int)result.Data_1,
                            ServiceId = serObj.Id
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu, 1);
                        rs.IsSuccess = true;
                        rs.Data = require.newNumber;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { Message = "Gửi thông tin bị lỗi không xử lý được yêu cầu" });
                    }
                }
                else
                {
                    rs.IsSuccess = false;
                    // rs.Errors.Add(new Error() { Message = "Dịch vụ hiện tạm ngưng phát số thứ tự. Xin quý khách đến vào buổi giao dịch sau." });
                    BLLCounterSoftRequire.Instance.Insert(connectString, "khoa.wav", (int)eCounterSoftRequireType.ReadSound, 0);
                }
            }
            return rs;
        }

        [HttpGet]
        public ResponseBase PrintTicketFromSMS(string smsContent, string sdt)
        {
            //format smsContent
            // HoTen-NamSinh-MaKhoa-MaPhongKham  -> full 
            // HoTen-NamSinh-MaKhoa   -> ko biet ma phong kham(kham moi)
            ResponseBase result = new ResponseBase();
            var _contentArr = smsContent.Trim().Split('-').ToList();
            if (_contentArr.Count >= 3)
            {
                int ns = 0;
                int.TryParse(_contentArr[1], out ns);
                result = InPhieu(new Q_DailyRequire() { CustomerName = _contentArr[0], MaBenhNhan = "", MaPhongKham = (_contentArr.Count == 3 ? string.Empty : _contentArr[3]), STT_PhongKham = "", PhoneNumber = sdt.Trim(), CustomerDOB = ns }, (_contentArr.Count == 3 ? _contentArr[2] : string.Empty));
                result.Data_1 = sdt;
            }
            else
            {
                result.IsSuccess = false;
                result.Errors = new List<Error>();
                result.Errors.Add(new Error() { MemberName = "Lỗi", Message = "Cấu trúc tin nhắn không hợp lệ. Vui lòng gửi lại theo cú pháp: 'HoTen NamSinh MaKhoa MaPhongKham' hoặc 'HoTen NamSinh MaKhoa' " });
            }
            return result;
        }

        [HttpGet]
        public ResponseBase UpdateTicketInfo(string TenBenhNhan, string MaBenhNhan, string STT_PhongKham)
        {
            return BLLDailyRequire.Instance.API_UpdateTicketInfo(connectString, TenBenhNhan, MaBenhNhan, STT_PhongKham);
        }


        [HttpGet]
        public List<ModelSelectItem> GetServices()
        {
            try
            {
                return BLLService.Instance.GetLookUp(connectString, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public ResponseBase _CapPhieu(string madichvu)
        {
            var rs = new ResponseBase();
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());

            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 00:00:00", "dd/MM/yyyy HH:mm:ss", provider);


            var serObj = BLLService.Instance.Get(connectString, int.Parse(madichvu));
            if (serObj != null)
            {
                string checkTime = "0";
                var cf = BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.CheckTimeBeforePrintTicket);
                var now = DateTime.Now;
                if (!string.IsNullOrEmpty(cf))
                    checkTime = cf;
                bool _continue = true;
                if (checkTime == "1" && BLLServiceShift.Instance.CheckTime(connectString, serObj.Id, now))
                    _continue = false;
                if (_continue)
                {
                    int _printType = 1;
                    int.TryParse(ConfigurationManager.AppSettings["PrintType"].ToString(), out _printType);
                    PrinterRequireModel require = null;
                    var result = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, serObj.StartNumber, 0, now, _printType, newDate.TimeOfDay, "", "", 0, "", "", "", "", "", "");
                    if (result.IsSuccess)
                    {
                        string _tenQuay = "";
                        string PrintAllTable = BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.PrintAllTable);

                        if (!string.IsNullOrEmpty(PrintAllTable) && PrintAllTable == "1")
                            _tenQuay = rs.Data_2;
                        else
                        {
                            if (rs.Data_4 != null)
                            {
                                var tqArr = (string[])rs.Data_4;
                                if (tqArr != null && tqArr.Length > 0)
                                    _tenQuay = tqArr[0];
                            }
                        }

                        //_tenQuay = "Quầy " + MaPhongKham;
                        rs.Data_4 = _tenQuay;
                        require = new PrinterRequireModel()
                        {
                            newNumber = ((int)result.Data + 1),
                            oldNumber = (int)result.Data,
                            TenDichVu = serObj.Name,
                            TenQuay = _tenQuay,// result.Data_2,
                            MajorId = (int)result.Data_1,
                            ServiceId = serObj.Id
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu, 1);
                        rs.IsSuccess = true;
                        rs.Data = require.newNumber;
                    }
                    else
                    {
                        rs.IsSuccess = false;
                        rs.Errors.Add(new Error() { Message = "Gửi thông tin bị lỗi không xử lý được yêu cầu" });
                    }
                }
                else
                {
                    rs.IsSuccess = false;
                    // rs.Errors.Add(new Error() { Message = "Dịch vụ hiện tạm ngưng phát số thứ tự. Xin quý khách đến vào buổi giao dịch sau." });
                    BLLCounterSoftRequire.Instance.Insert(connectString, "khoa.wav", (int)eCounterSoftRequireType.ReadSound, 0);
                }
            }
            return rs;
        }



        [HttpGet]
        public List<ModelSelectItem> GetEvaluates()
        {
            return BLLEvaluateDetail.Instance.GetLookUp(connectString);
        }

        [HttpGet]
        public ResponseBase PrintTicket(string soxe, string thoigian, int dichvuId)
        {
            var result = new ResponseBase();
            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 " + thoigian, "dd/MM/yyyy HH:mm:ss", provider);
            // chua co in mới   
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
            var serObj = BLLService.Instance.Get(connectString, dichvuId);
            if (serObj != null)
            {
                PrinterRequireModel require = null;
                var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, 1, 0, DateTime.Now, 2, newDate.TimeOfDay, "", "", 0, "", "", "", "", "", "");
                if (rs.IsSuccess)
                {
                    require = new PrinterRequireModel()
                    {
                        newNumber = ((int)rs.Data + 1),
                        oldNumber = (int)rs.Data,
                        TenDichVu = serObj.Name,
                        TenQuay = rs.Data_2,
                        MajorId = (int)rs.Data_1,
                        ServiceId = serObj.Id
                    };
                    BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu, 0);
                    result.IsSuccess = true;
                    result.Data = require.newNumber;
                    result.Data_1 = serObj.Name;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { Message = "Gửi thông tin bị lỗi không xử lý được yêu cầu" });
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Errors.Add(new Error() { MemberName = "in phiếu", Message = "Không tìm thấy thông tin dịch vụ với mã bạn vừa cung cấp. vui lòng kiểm tra lại." });
            }

            return result;
        }

        /// <summary>
        /// in phieu cho xe may với ds công việc sửa chữa
        /// </summary>
        /// <param name="soxe"></param>
        /// <param name="makh"></param>
        /// <param name="hotenkh"></param>
        /// <param name="maphieudichvu"></param>
        /// <param name="madichvu"></param>
        /// <param name="maphutung">danh sách mã công việc sửa chữa</param>
        /// <returns></returns>
        [HttpGet]
        public ResponseBase PrintTicket1(string maphieudichvu, string madichvu, string macongviec, string loaixe)
        {
            var result = new ResponseBase();
            //ktra xem so cu da co chua neu chua có moi in mới
            var foundTicket = BLLDailyRequire.Instance.Get(connectString, maphieudichvu);
            if (foundTicket == null)
            {
                // chua co in mới   
                var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
                var cf = BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.PrintType);
                int _printype = 1;
                if (!string.IsNullOrEmpty(cf))
                    int.TryParse(cf, out _printype);
                var serObj = BLLService.Instance.Get(connectString, madichvu);
                if (serObj != null)
                {
                    PrinterRequireModel require = null;
                    var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, serObj.StartNumber, 0, DateTime.Now, _printype, serObj.TimeProcess.TimeOfDay, "", "", 0, "", "", maphieudichvu, "", macongviec, loaixe);
                    if (rs.IsSuccess)
                    {
                        require = new PrinterRequireModel()
                        {
                            newNumber = ((int)rs.Data + 1),
                            oldNumber = (int)rs.Data,
                            TenDichVu = serObj.Name,
                            TenQuay = rs.Data_2,
                            MajorId = (int)rs.Data_1,
                            ServiceId = serObj.Id,
                            MaPhongKham = maphieudichvu
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu, 0);
                        result.IsSuccess = true;
                        result.Data = require.newNumber;
                        result.Data_1 = serObj.Name;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { Message = "Gửi thông tin bị lỗi không xử lý được yêu cầu" });
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "in phiếu", Message = "Không tìm thấy thông tin dịch vụ với mã bạn vừa cung cấp. vui lòng kiểm tra lại." });
                }
            }
            else
            {
                //có rồi trả lai so thôi
                result.IsSuccess = true;
                result.Data = foundTicket.TicketNumber;
            }
            return result;
        }

        [HttpGet]
        public ResponseBase PrintTicket_vtq(int stt, string maphieudichvu, string madichvu, string macongviec, string loaixe)
        {
            var result = new ResponseBase();
            //ktra xem so cu da co chua neu chua có moi in mới
            var foundTicket = BLLDailyRequire.Instance.Get(connectString, maphieudichvu);
            if (foundTicket == null)
            {
                // chua co in mới   
                var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
                var serObj = BLLService.Instance.Get(connectString, madichvu);
                if (serObj != null)
                {
                    PrinterRequireModel require = null;
                    var rs = BLLVietThaiQuan.Instance.ThemPhieu(connectString, stt, serObj.Id, maphieudichvu, macongviec, loaixe, serObj.TimeProcess.TimeOfDay, DateTime.Now);
                    if (rs.IsSuccess)
                    {
                        require = new PrinterRequireModel()
                        {
                            newNumber = ((int)rs.Data + 1),
                            oldNumber = (int)rs.Data,
                            TenDichVu = serObj.Name,
                            TenQuay = rs.Data_2,
                            MajorId = (int)rs.Data_1,
                            ServiceId = serObj.Id
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu, 0);
                        result.IsSuccess = true;
                        result.Data = require.newNumber;
                        result.Data_1 = serObj.Name;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Errors.Add(new Error() { Message = "Gửi thông tin bị lỗi không xử lý được yêu cầu" });
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "in phiếu", Message = "Không tìm thấy thông tin dịch vụ với mã bạn vừa cung cấp. vui lòng kiểm tra lại." });
                }
            }
            else
            {
                //có rồi trả lai so thôi
                result.IsSuccess = true;
                result.Data = foundTicket.TicketNumber;
            }
            return result;
        }


        [HttpGet]
        public ResponseBase CounterEvent(string counterId, string action, string param)
        {
            var rs = new ResponseBase();
            if (BLLCounterSoftRequire.Instance.HasProcessing(connectString, (int)eCounterSoftRequireType.PrintTicket) == null)
            {
                string content = "AA," + counterId + "," + action + "," + param;
                rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(connectString, content, (int)eCounterSoftRequireType.CounterEvent, 0);
            }
            return rs;
        }

        public string GetTicketStatus(int ticket)
        {
            return (BLLDailyRequire.Instance.CheckServeInformation(connectString, ticket).Data_1);
        }

        [HttpGet]
        public List<ModelSelectItem> GetEquipments()
        {
            return BLLEquipment.Instance.GetsEquipments(connectString);
        }

        /// <summary>
        /// lay stt cua QMS tu sttphong kham
        /// </summary>
        /// <param name="yourNumber">sttPhongKham</param>
        /// <returns></returns>
        [HttpGet]
        public int FindQMSTicketNumber(string yourNumber)
        {
            var obj = BLLDailyRequire.Instance.Get(connectString, yourNumber);
            return obj != null ? obj.TicketNumber : 0;
        }

        [HttpGet]
        public ResponseBaseModel CallNext(int matb, int userId, int dailyType)
        {
            ResponseBaseModel rs = BLLServiceApi.Instance.Next(connectString, userId, matb, DateTime.Now, 1);
            if (rs.IsSuccess)
            {
                TicketInfo tk = (TicketInfo)rs.Data_3;
                var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, userId, eCodeHex.Next);
                if (readTemplateIds.Count > 0)
                {
                    GPRO_Helper.Instance.GetSound(connectString, readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, (int)eDailyRequireType.KhamBenh, BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.TVReadSound));
                }
            }
            return rs;
        }

        [HttpGet]
        public ResponseBaseModel CallAny(int matb, int userId, int stt)
        {
            var rs = BLLDailyRequire.Instance.CallAny(connectString, userId, matb, stt, DateTime.Now);
            if (rs.IsSuccess)
            {
                TicketInfo tk = (TicketInfo)rs.Data_3;
                var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, userId, eCodeHex.Next);
                if (readTemplateIds.Count > 0)
                    GPRO_Helper.Instance.GetSound(connectString, readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, (int)eDailyRequireType.KhamBenh, BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.TVReadSound));

            }
            return rs;
        }

        [HttpGet]
        public ResponseBaseModel Recall(int matb, int userId)
        {
            var rs = BLLDailyRequire.Instance.GetCurrentTicket(connectString, userId, matb, DateTime.Now, 1);

            if (rs.IsSuccess == true)
            {
                TicketInfo tk = (TicketInfo)rs.Data_3;
                var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, userId, eCodeHex.Recall);
                if (readTemplateIds.Count > 0)
                    GPRO_Helper.Instance.GetSound(connectString, readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, (int)eDailyRequireType.KhamBenh, BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.TVReadSound));
            }
            return rs;
        }

        [HttpGet]
        public ResponseBaseModel DoneTicket(int matb, int userId)
        {
            return BLLDailyRequire.Instance.DoneTicket(connectString, userId, matb, DateTime.Now);
        }

        [HttpGet]
        public ResponseBaseModel DeleteTicket(int number, int userId)
        {
            return BLLDailyRequire.Instance.DeleteTicket(connectString, userId, number, DateTime.Now);
        }

        [HttpGet]
        public ResponseBaseModel TransferTicket(int matb, int manv, int stt)
        {
            var rs = new ResponseBaseModel();
            if (BLLDailyRequire.Instance.TranferTicket(connectString, matb, manv, stt, DateTime.Now, true))
            {
                rs.IsSuccess = true;
            }
            else
                rs.IsSuccess = false;
            return rs;
        }

        [HttpGet]
        public List<ModelSelectItem> GetMajors()
        {
            return BLLMajor.Instance.GetLookUp(connectString);
        }

        public ViewModel GetDayInfo_BV(string counters, string services, int userId, int getLastFiveNumbers)
        {
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var servicesArr = services.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var ss = BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, servicesArr, userId, getLastFiveNumbers == 1);
            // var obj = JsonConvert.SerializeObject(ss);
            // return Json(obj);
            return ss;
        }

        public ViewModel GetDayInfo_PK(string counters, int userId)
        {
            var countersArr = counters.Split(',').Select(x => Convert.ToInt32(x)).ToArray();
            var ss = BLLDailyRequire.Instance.GetDayInfo(AppGlobal.Connectionstring, countersArr, userId);
            return ss;
        }

        [HttpGet]
        public List<ModelSelectItem> GetDevices()
        {
            return BLLServiceApi.Instance.GetDevices(connectString);
        }

        [HttpGet]
        public List<ShowTVModel> GetShowTVs()
        {
            return BLLShowTV.Instance.Gets(connectString);
        }
    }
}
