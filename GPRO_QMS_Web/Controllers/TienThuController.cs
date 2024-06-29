using GPRO.Core.Mvc;
using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Configuration;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class TienThuController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;
        [HttpGet]
        public ResponseBase PrintTicket(string maphieudichvu, string madichvu, string diachi, string ten, string maKH, string soxe, string phone )
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
                    var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, 1, 0, DateTime.Now, 2, serObj.TimeProcess.TimeOfDay, ten, diachi, 0, maKH, "", maphieudichvu, soxe, "", "");
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
                            MaPhongKham = maphieudichvu,
                            MaBenhNhan = maKH,
                            Name = ten,
                            Address = diachi,
                            SoXe = soxe ,
                            Phone = phone
                        };
                        BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(require), (int)eCounterSoftRequireType.inPhieu,0);
                        //BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(new
                        //{
                        //    MaKH = maKH,
                        //    TenKH = ten,
                        //    DChi = diachi,
                        //    BSX = soxe,
                        //    SoLan = solan
                        //}), (int)eCounterSoftRequireType.ShowCustDetail_TT);
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
        public ResponseBase LuuThongTinKH( string dChi, string tenKH, string maKH, string soxe, string soLan,string ngaysua, string congviecs, string cuahang)
        {
            BLLCounterSoftRequire.Instance.Insert(connectString, JsonConvert.SerializeObject(new
            {
                MaKH = maKH,
                TenKH = tenKH,
                DChi = dChi,
                bSX = soxe,
                SoLan = soLan,
                NgaySua = ngaysua,
                CongViecs = congviecs,
                CuaHang = cuahang
            }), (int)eCounterSoftRequireType.ShowCustDetail_TT,0);
            return new ResponseBase();
        }
    }
}
