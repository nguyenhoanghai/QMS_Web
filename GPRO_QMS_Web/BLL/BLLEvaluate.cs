using GPRO_QMS_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GPRO_QMS_Web.Models;
using PagedList;
using Hugate.Framework;

namespace GPRO_QMS_Web.BLL
{
    public class BLLEvaluate
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLEvaluate _Instance;
        public static BLLEvaluate Instance
        {
            get
            {
                if (_Instance == null)
                    lock (key)
                        _Instance = new BLLEvaluate();

                return _Instance;
            }
        }

        private BLLEvaluate() { }

        #endregion

        public ResponseBase InsertOrUpdate(Q_Evaluate obj)
        {
            var rs = new ResponseBase();
            try
            {
                if (obj.Id == 0)
                {
                    db.Q_Evaluate.Add(obj);
                    rs.IsSuccess = false;
                }
                else
                {
                    var oldObj = Get(obj.Id);
                    if (oldObj != null)
                    {
                        oldObj.Index = obj.Index;
                        oldObj.Name = obj.Name;
                        oldObj.Note = obj.Note;
                    }
                }
                db.SaveChanges();
                rs.IsSuccess = true;
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
                    IsDefault = x.IsDefault,Icon = x.Icon
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
    }
}