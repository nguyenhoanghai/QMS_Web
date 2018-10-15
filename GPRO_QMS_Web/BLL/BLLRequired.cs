using GPRO_QMS_Web.Enum;
using GPRO_QMS_Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.BLL
{
    public class BLLRequired
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLRequired _Instance;  //volatile =>  tranh dung thread
        public static BLLRequired Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (key)
                    {
                        _Instance = new BLLRequired();
                    }
                }
                return _Instance;
            }
        }

        private BLLRequired() { }

        #endregion

        public ViewModel GetDayInfo()
        {
            db = new QMSEntities();
            DateTime Now = DateTime.Now;
            var totalrequireds = db.YEUCAUs.Where(x => x.GIOCAP.Value.Day == DateTime.Now.Day && x.GIOCAP.Value.Month == DateTime.Now.Month && x.GIOCAP.Value.Year == DateTime.Now.Year).ToList();
            var dayInfo = new ViewModel();
            dayInfo.Date = DateTime.Now.ToString("dd'/'MM'/'yyyy");
            dayInfo.Time = DateTime.Now.ToString("HH' : 'mm' : 'ss");
            dayInfo.TotalCar = totalrequireds.Select(x => x.MAPHIEU).Distinct().Count();
            dayInfo.TotalCarWaiting = totalrequireds.Where(x => x.MATT == eStatusName.Wating).Select(x => x.MAPHIEU).Distinct().Count();
            if (ConfigurationManager.AppSettings["IsTIENTHU"] == "1")
            {
                var numbers = db.XULYYCs.Where(x => x.GLAYPHIEU.Value.Day == Now.Day && x.GLAYPHIEU.Value.Month == Now.Month && x.GLAYPHIEU.Value.Year == Now.Year && x.MANV.Value == 16 && x.MATT == eStatusName.Processing).Select(x => x.MAPHIEU).Distinct().ToList();
                dayInfo.TotalCarProcessing = totalrequireds.Where(x => !numbers.Contains(x.MAPHIEU) && x.MATT == eStatusName.Processing).Select(x => x.MAPHIEU).Distinct().Count();
                dayInfo.TotalCarServed = totalrequireds.Where(x => x.MATT == eStatusName.Done || numbers.Contains(x.MAPHIEU)).Select(x => x.MAPHIEU).Distinct().Count();           
            }
            else
            {
                dayInfo.TotalCarProcessing = totalrequireds.Where(x => x.MATT == eStatusName.Processing).Select(x => x.MAPHIEU).Distinct().Count();
                dayInfo.TotalCarServed = totalrequireds.Where(x => x.MATT == eStatusName.Done).Select(x => x.MAPHIEU).Distinct().Count();
           
            }
            try
            {
                dayInfo.Details.AddRange(db.QUAYs.Where(x => x.IsShow).Select(x => new ViewDetailModel()
                {
                    CarNumber = "0",
                    TableId = x.MAQUAY,
                    TableName = x.TENQUAY,
                    Start = null,
                    TicketNumber = 0,
                    TimeProcess = "0",
                    StartStr = "0"
                }).ToList());

                var objs = db.XULYYCs.Where(x => x.MATT == eStatusName.Processing && x.GDENQUAY.Value.Day == DateTime.Now.Day && x.GDENQUAY.Value.Month == DateTime.Now.Month && x.GDENQUAY.Value.Year == DateTime.Now.Year).Select(x => new ViewDetailModel()
                    {
                        STT = (int)x.STT,
                        TableId = x.THIETBI.MAQUAY.Value,
                        CarNumber = x.YEUCAU.SOXE,
                        TableName = x.THIETBI.QUAY.TENQUAY,
                        Start = x.GGIAODICH.Value,
                        TicketNumber = (int)x.MAPHIEU,
                        Temp = x.Temp.Value
                    }).ToList();

                if (dayInfo.Details.Count > 0 && objs.Count > 0)
                {
                    foreach (var item in dayInfo.Details)
                    {
                        var find = objs.FirstOrDefault(x => x.TableId == item.TableId);
                        if (find != null)
                        {
                            if (find.Temp == null)
                            {
                                var fObj = db.XULYYCs.FirstOrDefault(x => x.STT == find.STT);
                                fObj.Temp = find.Start;
                                find.Temp = find.Start;
                            }

                            item.CarNumber = !string.IsNullOrEmpty(find.CarNumber) ? find.CarNumber : "0";
                            item.TicketNumber = find.TicketNumber;
                            item.Start = find.Temp;
                            if (find.Start != null)
                            {
                                TimeSpan time = DateTime.Now.Subtract(item.Start.Value);
                                item.TimeProcess = time.Hours + "<span class=\"blue\">'</span> " + time.Minutes + "<span class=\"blue\">\"</span>";
                                item.StartStr = (item.Start.Value.Hour > 9 ? item.Start.Value.Hour.ToString() : ("0" + item.Start.Value.Hour)) + "<span class=\"blue\"> : </span> " + (item.Start.Value.Minute > 9 ? item.Start.Value.Minute.ToString() : ("0" + item.Start.Value.Minute));
                            }
                            else
                            {
                                item.TimeProcess = "0";
                                item.StartStr = "0";
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
            return dayInfo;
        }

        public XULYYC GetCurrentProcess(string userName)
        {
            db = new QMSEntities();
            return db.XULYYCs.Where(x => (x.MATT == eStatusName.Processing || x.MATT == eStatusName.Rating && x.GDENQUAY.Value.Day == DateTime.Now.Day && x.GDENQUAY.Value.Month == DateTime.Now.Month && x.GDENQUAY.Value.Year == DateTime.Now.Year) && x.NHANVIEN.USERNAME.Trim().ToUpper().Equals(userName)).FirstOrDefault();
        }
    }
}