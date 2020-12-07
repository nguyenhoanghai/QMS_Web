using QMS_System.Data.BLL;
using QMS_System.Data.BLL.HuuNghi;
using QMS_System.Data.Model;
using QMS_System.ThirdApp.Enum;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                        GetSound(readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId,dailyType);
                    kq = true;
                }
            }

            return kq;
        }

        public void GetSound(List<int> templateIds, string ticket, int counterId, int dailyType)
        {
            var readDetails = BLLReadTempDetail.Instance.Gets(connectString, templateIds);
            if (readDetails.Count > 0)
            {
                var sounds = BLLSound.Instance.Gets(connectString);
                SoundModel soundFound;
                var soundStr = string.Empty;
                if (sounds.Count > 0)
                {
                    for (int y = 0; y < readDetails.Count; y++)
                    {
                        if (readDetails[y].Details.Count > 0)
                        {
                            for (int i = 0; i < readDetails[y].Details.Count; i++)
                            {
                                if (readDetails[y].Details[i].SoundId == 9999)
                                {
                                    for (int j = 0; j < ticket.Length; j++)
                                    {
                                        soundFound = sounds.FirstOrDefault(x => x.Code != null && x.Code.Equals(ticket[j] + "") && x.LanguageId == readDetails[y].LanguageId);
                                        if (soundFound != null)
                                        {
                                            soundStr += soundFound.Name + "|";
                                        }
                                    }
                                }
                                else if (readDetails[y].Details[i].SoundId == 10000)
                                {
                                    var name = BLLCounterSound.Instance.GetSoundName(connectString, counterId, readDetails[y].LanguageId);
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        soundStr += name + "|";
                                    }
                                }
                                else
                                {
                                    soundFound = sounds.FirstOrDefault(x => x.Id == readDetails[y].Details[i].SoundId);
                                    if (soundFound != null)
                                    {
                                        soundStr += soundFound.Name + "|";
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(soundStr))
                    {
                        soundStr = soundStr.Substring(0, soundStr.Length - 1);

                        if(dailyType == (int)eDailyRequireType.KetLuan)
                        {
                            soundStr += "|NhanKetQuaKetLuan.wav";
                        }

                        BLLCounterSoftRequire.Instance.Insert(connectString, soundStr, 1, counterId);
                    }
                }
            }
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
                        GetSound(readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId, (int)eDailyRequireType.KhamBenh);
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
            return BLLHuuNghi.Instance.DSCho(sqlConnection, maPK,(int)eDailyRequireType.KetLuan);
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
        public ResponseBase PrintNewTicket(string maBN, string tenBN, string maPK,  string bnAdd,int bnDOB,bool isKetLuan)
        {
            return BLLHuuNghi.Instance.API_PrintNewTicket(connectString, tenBN, bnAdd, bnDOB, maBN, maPK,isKetLuan);
        }

        [HttpGet]
        public int XoaPhieu(int ticket, string maPK, string maBN )
        {
            return BLLHuuNghi.Instance.DeleteTicket(connectString, ticket, maPK, maBN );
        }

        [HttpGet]
        public int KetThucPhieu(int ticket, string maPK, string maBN)
        {
            return BLLHuuNghi.Instance.DoneTicket(connectString, ticket, maPK, maBN);
        }
    }
}
