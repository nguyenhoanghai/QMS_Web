using QMS_System.Data.BLL;
using QMS_System.Data.Enum;
using QMS_System.Data.Model;
using QMS_System.ThirdApp.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace QMS_Website.Helper
{
    public class GPRO_Helper
    {
        #region constructor
        static object key = new object();
        private static volatile GPRO_Helper _Instance;  //volatile =>  tranh dung thread
        public static GPRO_Helper Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new GPRO_Helper();

                return _Instance;
            }
        }
        private GPRO_Helper() { }
        #endregion

        public string GetEntityConnectString()
        {
            try
            { 
                string filename =HttpContext.Current.Server.MapPath("~/Config_XML") + "\\DATA.XML";
                if (File.Exists(filename))
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(filename);
                    XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("SQLServer");
                    string serverName = EncryptionHelper.Instance.Decrypt(elementsByTagName.Item(0).ChildNodes[0].InnerText);
                    string databaseName = EncryptionHelper.Instance.Decrypt(elementsByTagName.Item(0).ChildNodes[1].InnerText);
                    string login = EncryptionHelper.Instance.Decrypt(elementsByTagName.Item(0).ChildNodes[2].InnerText);
                    string password = EncryptionHelper.Instance.Decrypt(elementsByTagName.Item(0).ChildNodes[3].InnerText);
                    string windowAuthen = elementsByTagName.Item(0).ChildNodes[4].InnerText;
                    //Build an Entity Framework connection string
                    var entityString = new EntityConnectionStringBuilder()
                    {
                        Provider = "System.Data.SqlClient",
                        Metadata = "res://*/QMSModel.csdl|res://*/QMSModel.ssdl|res://*/QMSModel.msl"
                    };
                    if (!Boolean.Parse(windowAuthen))
                    {
                        entityString.ProviderConnectionString = @"data source=" + serverName + ";initial catalog=" + databaseName + ";user id=" + login + ";password=" + password + ";MultipleActiveResultSets=true;";
                    }
                    else
                    {
                        entityString.ProviderConnectionString = @"data source=" + serverName + ";initial catalog=" + databaseName + ";integrated security=True;MultipleActiveResultSets=true;";
                    }
                    return entityString.ConnectionString;
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public void GetSound(string connectString, List<int> templateIds, string ticket, int counterId, int dailyType, string tvReadSound)
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

                        if (dailyType == (int)eDailyRequireType.KetLuan)
                        {
                            soundStr += "|NhanKetQuaKetLuan.wav";
                        }

                        BLLCounterSoftRequire.Instance.Insert(connectString, soundStr, 1, counterId);
                        if (!string.IsNullOrEmpty(soundStr) && !string.IsNullOrEmpty(tvReadSound) && tvReadSound == "1")
                        { 
                            BLLCounterSoftRequire.Instance.Insert(connectString, soundStr, (int)eCounterSoftRequireType.TVReadSound, counterId);
                        }
                    }
                }
            }
        }

    }
}