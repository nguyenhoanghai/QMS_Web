using PagedList;
using GPRO_QMS_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hugate.Framework;

namespace GPRO_QMS_Web.BLL
{
    public class BLLEvaluateDetail
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLEvaluateDetail _Instance;
        public static BLLEvaluateDetail Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEvaluateDetail();

                return _Instance;
            }
        }

        private BLLEvaluateDetail() { }

        #endregion

        public ResponseBase InsertOrUpdate(Q_EvaluateDetail obj)
        {
            var rs = new ResponseBase();
            try
            {
                db = new Models.QMSEntities();
                if (obj.Id == 0)
                {
                    db.Q_EvaluateDetail.Add(obj);
                    rs.IsSuccess = false;
                }
                else
                {
                    var oldObj = Get(obj.Id);
                    if (oldObj != null)
                    {
                        oldObj.EvaluateId = obj.EvaluateId;
                        oldObj.Index = obj.Index;
                        oldObj.Name = obj.Name;
                        oldObj.IsDefault = obj.IsDefault;
                        oldObj.Note = obj.Note;
                        oldObj.Icon = obj.Icon;
                    }
                }
                db.SaveChanges();
                rs.IsSuccess = true;
            }
            catch { }
            return rs;
        }

        public Q_EvaluateDetail Get(int Id)
        {
            db = new Models.QMSEntities();
            return db.Q_EvaluateDetail.FirstOrDefault(x => !x.IsDeleted && x.Id == Id);
        }

        public PagedList<EvaluateDetailModel> GetList(int type, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                db = new Models.QMSEntities();
                var pageNumber = (startIndexRecord / pageSize) + 1;
                if (string.IsNullOrEmpty(sorting))
                    sorting = "Index DESC";
                var objs = db.Q_EvaluateDetail.Where(x => !x.IsDeleted && x.EvaluateId == type).OrderBy(sorting).Select(x => new EvaluateDetailModel()
                {
                    Id = x.Id,
                    EvaluateId = x.EvaluateId,
                    Index = x.Index,
                    Name = x.Name,
                    Note = x.Note,
                    IsDefault = x.IsDefault,
                    Icon = x.Icon
                }).ToList();
                return new PagedList<EvaluateDetailModel>(objs, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
    }
}