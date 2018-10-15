using GPRO_QMS_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using Hugate.Framework;
using GPRO_QMS_Web.Enum;

namespace GPRO_QMS_Web.BLL
{
    public class BLLUserEvaluate
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLUserEvaluate _Instance;
        public static BLLUserEvaluate Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLUserEvaluate();

                return _Instance;
            }
        }

        private BLLUserEvaluate() { }

        #endregion

        public ResponseBase Evaluate(string username, string value, int num, string isUseQMS)
        {
            var rs = new ResponseBase();
            try
            {
                db = new QMSEntities();
                var user = db.NHANVIENs.FirstOrDefault(x => x.USERNAME.Trim().ToUpper().Equals(username.Trim().ToUpper()));
                Q_UserEvaluate newObj;
                var now = DateTime.Now;
                if (isUseQMS == "1")
                {
                    #region
                    var ticketInfos = db.XULYYCs.Where(x => x.MAPHIEU == num && x.NHANVIEN.USERNAME.Trim().ToUpper().Equals(username.Trim().ToUpper()) && (x.MATT == eStatusName.Rating || x.MATT == eStatusName.Processing)).ToList();
                    if (ticketInfos.Count > 0 && user != null)
                    {
                        var firstObj = ticketInfos.FirstOrDefault();
                        var obj = db.Q_UserEvaluate.FirstOrDefault(x => x.UserId == user.MANV && x.TicketNumber == num && x.GLAYPHIEU == firstObj.GLAYPHIEU);
                        if (obj == null)
                        {
                            newObj = new Q_UserEvaluate();
                            newObj.UserId = user.MANV;
                            newObj.TicketNumber = num;
                            newObj.Score = value;
                            newObj.GLAYPHIEU = firstObj.GLAYPHIEU;
                            newObj.GDENQUAY = firstObj.GDENQUAY;
                            newObj.GGIAODICH = firstObj.GGIAODICH;
                            newObj.GKETTHUC = firstObj.GKETTHUC;
                            db.Q_UserEvaluate.Add(newObj);

                            //var findButton = db.Q_CallButtonNext.FirstOrDefault(x => x.CounterId == ticketInfo.MATB.Value && x.UserId == ticketInfo.MANV);
                            //if (findButton != null)
                            //    findButton.IsClick = true;
                            //else
                            //{
                            //    findButton = new Q_CallButtonNext();
                            //    findButton.UserId = ticketInfo.MANV.Value;
                            //    findButton.CounterId = ticketInfo.MATB.Value;
                            //    findButton.IsClick = true;
                            //    db.Q_CallButtonNext.Add(findButton);
                            //} 
                        }
                        foreach (var item in ticketInfos)
                        {
                            item.MATT = eStatusName.Done;
                            item.GKETTHUC = now;
                        }

                        if (ticketInfos.Count >0)
                        {
                            var yc = db.YEUCAUs.FirstOrDefault(x => x.MAPHIEU == num);
                            if (yc != null)
                                yc.MATT = eStatusName.Done;
                        }
                        db.SaveChanges();
                        rs.IsSuccess = true;
                    }
                    #endregion
                }
                else
                {
                    newObj = new Q_UserEvaluate();
                    newObj.GKETTHUC = now;
                    newObj.UserId = user.MANV;
                    newObj.TicketNumber = 0;
                    newObj.Score = value;
                    db.Q_UserEvaluate.Add(newObj);
                    db.SaveChanges();
                    rs.IsSuccess = true;
                }
            }
            catch { }
            return rs;
        }

        public Q_Evaluate Get(int Id)
        {
            db = new Models.QMSEntities();
            return db.Q_Evaluate.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public PagedList<EvaluateModel> GetList(string keyWord, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                db = new QMSEntities();
                IEnumerable<Q_Evaluate> objs = null;
                var pageNumber = (startIndexRecord / pageSize) + 1;
                if (string.IsNullOrEmpty(sorting))
                    sorting = "Index DESC";
                if (!string.IsNullOrEmpty(keyWord))
                    objs = db.Q_Evaluate.Where(x => !x.IsDeleted && x.Name.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderBy(sorting);
                else
                    objs = db.Q_Evaluate.Where(x => !x.IsDeleted).OrderBy(sorting);

                if (objs != null && objs.Count() > 0)
                    return new PagedList<EvaluateModel>(objs.Select(x => new EvaluateModel() { Id = x.Id, Name = x.Name, Index = x.Index, Note = x.Note }).ToList(), pageNumber, pageSize);
                else
                    return new PagedList<EvaluateModel>(new List<EvaluateModel>(), pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ModelSelectItem> Gets()
        {
            db = new Models.QMSEntities();
            return db.Q_Evaluate.Where(x => !x.IsDeleted).OrderBy(x => x.Index).Select(x => new ModelSelectItem() { Value = x.Id, Name = x.Name }).ToList();
        }

        public ResponseBase Delete(int Id)
        {
            var rs = new ResponseBase();
            try
            {
                db = new Models.QMSEntities();
                var obj = Get(Id);
                if (obj != null)
                {
                    obj.IsDeleted = true;
                    db.SaveChanges();
                    rs.IsSuccess = true;
                }
                else
                    rs.IsSuccess = false;
            }
            catch (Exception)
            {
            }
            return rs;
        }

        public List<EvaluateModel> GetList()
        {
            db = new Models.QMSEntities();
            var parents = db.Q_Evaluate.Where(x => !x.IsDeleted).OrderBy(x => x.Index).Select(x => new EvaluateModel()
            {
                Id = x.Id,
                Name = x.Name,
                Index = x.Index
            }).ToList();
            if (parents.Count > 0)
            {
                var childs = db.Q_EvaluateDetail.Where(x => !x.IsDeleted).Select(x => new EvaluateDetailModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Index = x.Index,
                    EvaluateId = x.EvaluateId,
                    IsDefault = x.IsDefault,
                    Icon = x.Icon
                }).ToList();
                if (childs.Count > 0)
                {
                    foreach (var item in parents)
                    {
                        item.Childs.AddRange(childs.Where(x => x.EvaluateId == item.Id).OrderBy(x => x.Index).ToList());
                    }
                    return parents;
                }
            }
            return null;
        }

        public List<ReportModel> GetReport(int userId, DateTime from, DateTime to)
        {
            db = new Models.QMSEntities();
            List<ReportModel> report = new List<ReportModel>();
            ReportModel obj;
            if (userId != 0)
            {
                obj = new ReportModel();
                obj.Name = db.NHANVIENs.FirstOrDefault(x => x.MANV == userId).TENNV;
                obj.tc1 = 0;
                obj.tc2 = 0;
                obj.tc3 = 0;
                var danhgia = db.Q_UserEvaluate.Where(x => (x.GKETTHUC.Value.Day >= from.Day && x.GKETTHUC.Value.Month == from.Month && x.GKETTHUC.Value.Year == from.Year) && (x.GKETTHUC.Value.Day <= to.Day && x.GKETTHUC.Value.Month == to.Month && x.GKETTHUC.Value.Year == to.Year) && x.UserId == userId).ToList();
                if (danhgia.Count > 0)
                {
                    obj.tc1 = danhgia.Where(x => x.Score == "1_3").Count();
                    obj.tc2 = danhgia.Where(x => x.Score == "1_1").Count();
                    obj.tc3 = danhgia.Where(x => x.Score == "1_2").Count();
                }
                report.Add(obj);
            }
            else
            {
                var danhgia = db.Q_UserEvaluate.Where(x => (x.GKETTHUC.Value.Day >= from.Day && x.GKETTHUC.Value.Month == from.Month && x.GKETTHUC.Value.Year == from.Year) && (x.GKETTHUC.Value.Day <= to.Day && x.GKETTHUC.Value.Month == to.Month && x.GKETTHUC.Value.Year == to.Year)).ToList();
                report.AddRange(db.NHANVIENs.Select(x => new ReportModel() { UserId = x.MANV, Name = x.TENNV, tc1 = 0, tc2 = 0, tc3 = 0 }));
                   
                if (danhgia.Count > 0)
                {
                  //  var ids = danhgia.Select(x => x.UserId).Distinct();
                  //  report.AddRange(db.NHANVIENs.Select(x => new ReportModel() { UserId = x.MANV, Name = x.TENNV, tc1 = 0, tc2 = 0, tc3 = 0 }));
                    foreach (var item in report)
                    {
                        var objs = danhgia.Where(x=> x.UserId == item.UserId).ToList();
                        if (objs.Count > 0)
                        {
                            item.tc1 = objs.Where(x => x.Score == "1_3"  ).Count();
                            item.tc2 = objs.Where(x => x.Score == "1_1"  ).Count();
                            item.tc3 = objs.Where(x => x.Score == "1_2"  ).Count();
                        }                        
                    }
                }
            }
            return report;
        }

        public List<ReportModel> GetDailyReport()
        {
            db = new Models.QMSEntities();
            var now = DateTime.Now;
            List<ReportModel> report = new List<ReportModel>();
            report.AddRange(db.NGHIEPVUs.Select(x => new ReportModel() { ServiceId = x.MANVU, ServiceName = x.TENNVU, tc1 = 0, tc2 = 0, tc3 = 0 }));
            if (report.Count > 0)
            {
                var danhgia = db.Q_UserEvaluate.Where(x => (x.GKETTHUC.Value.Day >= now.Day && x.GKETTHUC.Value.Month == now.Month && x.GKETTHUC.Value.Year == now.Year) && (x.GKETTHUC.Value.Day <= now.Day && x.GKETTHUC.Value.Month == now.Month && x.GKETTHUC.Value.Year == now.Year)).ToList();
                var nvDG = db.NV_NGHIEPVU.ToList();
                List<int> userIds;
                foreach (var r in report)
                {
                    userIds = nvDG.Where(x => x.MANVU == r.ServiceId && x.UUTIEN == 1).Select(x => x.MANV).Distinct().ToList();
                    if ( userIds.Count > 0)
                    {
                        r.tc1 = danhgia.Where(x => x.Score == "1_3" && userIds.Contains( x.UserId)).Count();
                        r.tc2 = danhgia.Where(x => x.Score == "1_1" && userIds.Contains(x.UserId)).Count();
                        r.tc3 = danhgia.Where(x => x.Score == "1_2" && userIds.Contains(x.UserId)).Count();
                    }                    
                } 
            }
            return report;
        }
    }

    public class ReportModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public int tc1 { get; set; }
        public int tc2 { get; set; }
        public int tc3 { get; set; }
    }
}