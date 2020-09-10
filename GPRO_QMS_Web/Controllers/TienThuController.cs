using Newtonsoft.Json;
using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class TienThuController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;
        [HttpGet]
        public ResponseBase PrintTicket(string maphieudichvu, string madichvu, string diachi, string ten, string maKH,string soxe)
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
                    var rs = BLLDailyRequire.Instance.PrintNewTicket(connectString, serObj.Id, 1, 0, DateTime.Now, 2, serObj.TimeProcess.TimeOfDay, ten,diachi,0, maKH, "", maphieudichvu, soxe, "", "");
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
    }
}
