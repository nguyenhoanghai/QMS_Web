using QMS_System.Data.BLL;
using QMS_System.Data.BLL.HuuNghi;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_System.ThirdApp.Enum;
using QMS_Website.App_Global;
using QMS_Website.Helper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class HuuNghiServiceController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;
        public SqlConnection sqlConnection = AppGlobal.sqlConnection;

        [HttpGet]
        public bool GoiKB(string pMaPhongKham, string pSoThuTu, int dailyType)
        {
            bool kq = false;
            var info = BLLLoginHistory.Instance.findLogin(connectString, pMaPhongKham);
            if (info.IsSuccess)
            {
                ResponseBaseModel rs = BLLHuuNghi.Instance.CallAny(connectString, info.Data, info.Records, Convert.ToInt32(pSoThuTu), DateTime.Now, dailyType);
                if (rs.IsSuccess)
                {
                    TicketInfo tk = (TicketInfo)rs.Data_3;
                    var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, info.Data, "8B");
                    if (readTemplateIds.Count > 0)
                        GPRO_Helper.Instance.GetSound(connectString, readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, dailyType, BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.TVReadSound));
                    kq = true;
                }
            }
            return kq;
        }


        /// <summary>
        /// goi so phong vien phi va phát thuốc
        /// </summary>
        /// <param name="pMaPhongKham"></param> 
        /// <returns></returns>
        [HttpGet]
        public bool Goi_VienPhi_PhatThuoc(string pMaPhongKham)
        {

            bool kq = false;
            var info = BLLLoginHistory.Instance.findLogin(connectString, pMaPhongKham);
            if (info.IsSuccess)
            {
                ResponseBaseModel rs = BLLDailyRequire.Instance.Next(connectString, info.Data, info.Records, DateTime.Now, 1);
                if (rs.IsSuccess)
                {
                    TicketInfo tk = (TicketInfo)rs.Data_3;
                    var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, info.Data, "8B");
                    if (readTemplateIds.Count > 0)
                        GPRO_Helper.Instance.GetSound(connectString, readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, (int)eDailyRequireType.KhamBenh, BLLConfig.Instance.GetConfigByCode(connectString, eConfigCode.TVReadSound));
                    kq = true;
                }
            }

            return kq;
        }

        /// <summary>
        /// Lấy DS BN chờ Kết luận bệnh
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DataSet DSChoKetLuan(string maPK)
        {
            return BLLHuuNghi.Instance.DSCho(sqlConnection, maPK, (int)eDailyRequireType.KetLuan);
        }

        /// <summary>
        /// Lấy DS BN chờ khu vực khám bệnh - Viện phí - phát thuốc
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public DataSet DSBenhNhanCho_KBenh_VPhi_PThuoc(string maPK)
        {
            return BLLHuuNghi.Instance.DSCho(sqlConnection, maPK, (int)eDailyRequireType.KhamBenh);
        }

        [HttpGet]
        public int QuaLuot(int ticket, string maPK, string maBN, int serviceType)
        {
            return BLLHuuNghi.Instance.QuaLuot(connectString, ticket, maPK, maBN, serviceType);
        }

        [HttpGet]
        public ResponseBase PrintNewTicket(string maBN, string tenBN, string maPK, string bnAdd, int bnDOB, bool isKetLuan)
        {
            return BLLHuuNghi.Instance.API_PrintNewTicket(connectString, tenBN, bnAdd, bnDOB, maBN, maPK, isKetLuan);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="maPK"></param>
        /// <param name="maBN"></param>
        /// <param name="type">KhamBenh = 1, KetLuan = 2</param>
        /// <returns></returns>
        [HttpGet]
        public int XoaPhieu(int ticket, string maPK, string maBN, int type)
        {
            return BLLHuuNghi.Instance.DeleteTicket(connectString, ticket, maBN, maPK, type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="maPK"></param>
        /// <param name="maBN"></param>
        /// <param name="type">KhamBenh = 1, KetLuan = 2</param>
        /// <returns></returns>
        [HttpGet]
        public int KetThucPhieu(int ticket, string maPK, string maBN, int type)
        {
            return BLLHuuNghi.Instance.DoneTicket(connectString, maBN, maPK, ticket, type);
        }
    }
}
