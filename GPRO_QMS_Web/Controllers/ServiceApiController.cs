using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.BLL.VietThaiQuan;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
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
        public string connectString = AppGlobal.Connectionstring;
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
            return BLLDailyRequire.Instance.API_PrintNewTicket(connectString, TenBenhNhan, null, null, MaBenhNhan, MaPhongKham, STT_PhongKham);
        }

        [HttpGet]
        public ResponseBase PrintNewTicket(string MaPhongKham, string thoigian)
        {
            var rs = new ResponseBase();
            //if (BLLCounterSoftRequire.Instance.HasProcessing(connectString, (int)eCounterSoftRequireType.PrintTicket) == null)
            //{
            var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());

            CultureInfo provider = CultureInfo.InvariantCulture;
            var newDate = DateTime.ParseExact("01/01/2018 " + thoigian, "dd/MM/yyyy HH:mm:ss", provider);

            //var require = new PrinterRequireModel()
            //{
            //    PrinterId = printerId,
            //    SoXe = "",
            //    ServeTime = newDate,
            //    ServiceId = int.Parse(MaPhongKham)
            //};
            //rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.PrintTicket);


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
                        require = new PrinterRequireModel()
                        {
                            newNumber = ((int)result.Data + 1),
                            oldNumber = (int)result.Data,
                            TenDichVu = serObj.Name,
                            TenQuay = result.Data_2,
                            MajorId = (int)result.Data_1,
                            ServiceId = serObj.Id
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu);
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
                    BLLCounterSoftRequire.Instance.Insert(connectString, "khoa.wav", (int)eCounterSoftRequireType.ReadSound);
                }
            }
            return rs;
        }

        [HttpGet]
        public ResponseBase UpdateTicketInfo(string TenBenhNhan, string MaBenhNhan, string STT_PhongKham)
        {
            return BLLDailyRequire.Instance.API_UpdateTicketInfo(connectString, TenBenhNhan, MaBenhNhan, STT_PhongKham);
        }

        [HttpGet]
        public List<ModelSelectItem> GetServices()
        {
            return BLLService.Instance.GetLookUp(connectString, false);
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
                    BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu);
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
                var serObj = BLLService.Instance.Get(connectString, madichvu);
                if (serObj != null)
                {
                    PrinterRequireModel require = null;
                    var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, 1, 0, DateTime.Now, 2, serObj.TimeProcess.TimeOfDay, "", "", 0, "", "", maphieudichvu, "", macongviec, loaixe);
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
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu);
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
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu);
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


        //[HttpGet]
        //public ResponseBase PrintTicket1(string maphieudichvu, string madichvu, string macongviec, string loaixe)
        //{
        //    var rs = new ResponseBase();
        //    if (BLLCounterSoftRequire.Instance.HasProcessing(connectString, (int)eCounterSoftRequireType.PrintTicket) == null)
        //    {
        //        var printerId = Convert.ToInt32(ConfigurationManager.AppSettings["PrinterId"].ToString());
        //        var serObj = BLLService.Instance.Get(connectString, madichvu);
        //        if (serObj != null)
        //        {
        //            //if (CheckTimeBeforePrintTicket == "1" && serObj.Shifts.FirstOrDefault(x => now.TimeOfDay >= x.Start.TimeOfDay && now.TimeOfDay <= x.End.TimeOfDay) == null)
        //            //    temp.Add(SoundLockPrintTicket);
        //            //else
        //            //{
        //            var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serviceId, startNumber, businessId, now, printType, (timeServeAllow != null ? timeServeAllow.Value : serObj.TimeProcess.TimeOfDay), Name, Address, DOB, MaBenhNhan, MaPhongKham, SttPhongKham, SoXe, MaCongViec, MaLoaiCongViec);
        //            if (rs.IsSuccess)
        //            {
        //                if (!isProgrammer)
        //                {
        //                    var soArr = BaseCore.Instance.ChangeNumber(((int)rs.Data + 1));
        //                    printStr = (soArr[0] + " " + soArr[1] + " ");
        //                    if (printTicketReturnCurrentNumberOrServiceCode == 1)
        //                    {
        //                        soArr = BaseCore.Instance.ChangeNumber((int)rs.Records);
        //                    }
        //                    else
        //                    {
        //                        soArr = BaseCore.Instance.ChangeNumber(serviceId);
        //                    }
        //                    printStr += (soArr[0] + " " + soArr[1] + " " + now.ToString("dd") + " " + now.ToString("MM") + " " + now.ToString("yy") + " " + now.ToString("HH") + " " + now.ToString("mm"));
        //                }
        //                else if (isProgrammer)
        //                    lbRecieve.Caption = printerId + "," + serviceId + "," + ((int)rs.Data + 1);
        //                nghiepVu = rs.Data_1;
        //                newNumber = ((int)rs.Data + 1);
        //                tenQuay = rs.Data_2;
        //            }
        //            else
        //                errorsms = rs.Errors[0].Message;
        //            //  MessageBox.Show(rs.Errors[0].Message, rs.Errors[0].MemberName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            // }







        //            var require = new PrinterRequireModel()
        //            {
        //                PrinterId = printerId,
        //                SoXe = "",
        //                Name = "",
        //                MaBenhNhan = "",
        //                ServiceId = serObj.Id,
        //                ServeTime = serObj.TimeProcess,
        //                SttPhongKham = maphieudichvu,
        //                MaCongViec = macongviec,
        //                MaLoaiCongViec = loaixe
        //            };
        //            rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.PrintTicket);
        //        }
        //        else
        //        {
        //            rs.IsSuccess = false;
        //            rs.Errors.Add(new Error() { MemberName = "in phiếu", Message = "Không tìm thấy thông tin dịch vụ với mã bạn vừa cung cấp. vui lòng kiểm tra lại." });
        //        }
        //    }
        //    return rs;
        //}

        [HttpGet]
        public ResponseBase CounterEvent(string counterId, string action, string param)
        {
            var rs = new ResponseBase();
            if (BLLCounterSoftRequire.Instance.HasProcessing(connectString, (int)eCounterSoftRequireType.PrintTicket) == null)
            {
                string content = "AA," + counterId + "," + action + "," + param;
                rs.IsSuccess = BLLCounterSoftRequire.Instance.Insert(connectString, content, (int)eCounterSoftRequireType.CounterEvent);
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
    }
}
