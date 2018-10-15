using GPRO_QMS_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.BLL
{
    public class BLLUser_Eval
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLUser_Eval _Instance;
        public static BLLUser_Eval Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLUser_Eval();

                return _Instance;
            }
        }

        private BLLUser_Eval() { }

        #endregion

        public ResponseBase Evaluate(string username, string value, int num)
        {
            var rs = new ResponseBase();
            try
            {
                db = new QMSEntities();
                var ticketinfo = db.XULYYCs.FirstOrDefault(x => x.MAPHIEU == num);
                if (ticketinfo != null)
                {
                    var user = db.NHANVIENs.FirstOrDefault(x => x.USERNAME.Trim().ToUpper().Equals(username.Trim().ToUpper()));
                    var obj = new Q_UserEvaluate();
                    obj.UserId = user.MANV;
                    obj.TicketNumber = num;
                    obj.GLAYPHIEU = ticketinfo.GLAYPHIEU;
                    obj.GDENQUAY = ticketinfo.GDENQUAY;
                    obj.GGIAODICH = ticketinfo.GGIAODICH;
                    obj.GKETTHUC = ticketinfo.GKETTHUC;
                   
                }

                //if (obj.Id == 0)
                //{
                //    db.Q_Evaluate.Add(obj);
                //    rs.IsSuccess = false;
                //}
                //else
                //{
                //    var oldObj = Get(obj.Id);
                //    if (oldObj != null)
                //    {
                //        oldObj.Index = obj.Index;
                //        oldObj.Name = obj.Name;
                //        oldObj.Note = obj.Note;
                //    }
                //}
                db.SaveChanges();
                rs.IsSuccess = true;
            }
            catch { }
            return rs;
        }
    }
}