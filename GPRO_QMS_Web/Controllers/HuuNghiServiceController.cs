using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace QMS_Website.Controllers
{
    public class HuuNghiServiceController : ApiController
    {
        public string connectString = AppGlobal.Connectionstring;

        [HttpGet]
        public bool GoiKB(string pMaPhongKham, string pSoThuTu)
        {

            bool kq = false;
            var info = BLLLoginHistory.Instance.findLogin(connectString, pMaPhongKham);
            if (info.IsSuccess)
            {
                ResponseBaseModel rs = BLLDailyRequire.Instance.CallAny(connectString, info.Data, info.Records, Convert.ToInt32(pSoThuTu), DateTime.Now);
                if (rs.IsSuccess)
                {
                    TicketInfo tk = (TicketInfo)rs.Data_3;
                    var readTemplateIds = BLLUserCmdReadSound.Instance.GetReadTemplateIds(connectString, info.Data, "8B");
                    if (readTemplateIds.Count > 0)
                        GetSound(readTemplateIds, tk.TicketNumber.ToString(), tk.CounterId);
                    kq = true;
                }
            }

            return kq;
        }

        public void GetSound(List<int> templateIds, string ticket, int counterId)
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
                        BLLCounterSoftRequire.Instance.Insert(connectString, soundStr, 1, counterId);
                    }
                }
            }
        }

    }
}
