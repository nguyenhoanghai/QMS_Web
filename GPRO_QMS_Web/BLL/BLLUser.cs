using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GPRO_QMS_Web.Models; //ResponseBase  //ModelSelectItem
using System.Web.Mvc;
using GPRO.Core.Generic; //JsonDataResult
using GPRO.Core.Mvc;
using PagedList; //PageList  

namespace GPRO_QMS_Web.BLL
{
    public class BLLUser
    {
        #region constructor
        QMSEntities db;
        static object key = new object();
        private static volatile BLLUser _Instance;  //volatile =>  tranh dung thread
        public static BLLUser Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (key)
                    {
                        _Instance = new BLLUser();
                    }
                }
                return _Instance;
            }
        }

        private BLLUser() { }
        #endregion
        public PagedList<UserModel> GetList(string keyWord, int searchBy, int startIndexRecord, int pageSize, string sorting)
        {
            try
            {
                db = new QMSEntities();
                db.Configuration.ProxyCreationEnabled = false;

                if (string.IsNullOrEmpty(sorting))
                {
                    sorting = "MANV DESC";
                }
                IQueryable<NHANVIEN> objs = null;  //dung kieu IQueryable<> moi OrderBy duoc; List<> ko OrderBy dc 
                var pageNumber = (startIndexRecord / pageSize) + 1;
                if (!string.IsNullOrEmpty(keyWord))
                    objs = db.NHANVIENs.Where(x => x.TENNV.Trim().ToUpper().Contains(keyWord.Trim().ToUpper())).OrderByDescending(x => x.MANV);
                else
                    objs = db.NHANVIENs.OrderByDescending(x => x.MANV);

                if (objs != null && objs.Count() > 0)
                {
                    var NhanVien = objs.Select(x => new UserModel()
                   {
                       MANV = x.MANV,
                       TENNV = x.TENNV,
                       DIACHI = x.DIACHI,
                       GIOITINH = x.GIOITINH,
                       TROGIUP = x.TROGIUP,
                       USERNAME = x.USERNAME,
                       PASSWORD = x.PASSWORD,
                       Hinh = x.Hinh,
                       ChuyenMon = x.ChuyenMon,
                       ChucVu = x.ChucVu,
                       QTCongTac = x.QTCongTac,
                   });
                    return new PagedList<UserModel>(NhanVien.ToList(), pageNumber, pageSize);
                }
                else
                    return new PagedList<UserModel>(new List<UserModel>(), pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase CreateOrUpdate(NHANVIEN nv)
        {
            try
            {
                db = new QMSEntities();
                var result = new GPRO_QMS_Web.Models.ResponseBase();
                bool flag = false;
                if (CheckExists(nv.TENNV, nv.MANV))
                {
                    result.IsSuccess = false;
                    result.Errors.Add(new Error() { MemberName = "Insert", Message = "Tên Nhân Viên này đã được sử dụng. Vui lòng nhập Tên khác !." });
                    flag = true;
                }
                if (!flag)
                {
                    if (nv.MANV == 0)
                    {
                        int manv = db.NHANVIENs.Count() + 1;
                        var obj = db.NHANVIENs.FirstOrDefault(x => x.MANV == (Int16)manv); //Int16 == short int
                        while ((obj != null))
                        {
                            manv++;
                            obj = db.NHANVIENs.FirstOrDefault(x => x.MANV == (Int16)manv);
                        }
                        nv.MANV = (Int16)manv;
                        db.NHANVIENs.Add(nv);
                        db.SaveChanges();
                    }
                    else
                    {
                        NHANVIEN nhanvien = db.NHANVIENs.FirstOrDefault(m => m.MANV == nv.MANV);
                        if (nhanvien == null)
                        {
                            result.IsSuccess = false;
                            result.Errors.Add(new Error() { MemberName = "Update", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                            return result;
                        }
                        else
                        {
                            nhanvien.MANV = nv.MANV;
                            nhanvien.TENNV = nv.TENNV;
                            nhanvien.DIACHI = nv.DIACHI;
                            nhanvien.GIOITINH = nv.GIOITINH;
                            nhanvien.TROGIUP = nv.TROGIUP;
                            nhanvien.USERNAME = nv.USERNAME;
                            nhanvien.PASSWORD = nv.PASSWORD;
                            nhanvien.Hinh = nv.Hinh;
                            nhanvien.ChucVu = nv.ChucVu;
                            nhanvien.QTCongTac = nv.QTCongTac;
                            nhanvien.ChuyenMon = nv.ChuyenMon;
                            db.SaveChanges();
                        }
                    }
                    result.IsSuccess = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResponseBase Delete(int manv)
        {
            try
            {
                db = new QMSEntities();
                var result = new ResponseBase();
                var nv = db.NHANVIENs.FirstOrDefault(x => x.MANV == manv);
                if (nv == null)
                {
                    result.IsSuccess = false; ;
                    result.Errors.Add(new Error() { MemberName = "Delete", Message = "Dữ liệu bạn đang thao tác đã bị xóa hoặc không tồn tại. Vui lòng kiểm tra lại !." });
                }
                else
                {
                    db.NHANVIENs.Remove(nv);
                    db.SaveChanges();
                    result.IsSuccess = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckExists(string keyword, int manv)
        {
            try
            {
                db = new QMSEntities();
                NHANVIEN nv = null;
                keyword = keyword.Trim().ToUpper();
                nv = db.NHANVIENs.FirstOrDefault(x => x.MANV != manv && x.TENNV.Trim().ToUpper().Equals(keyword));
                if (nv == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       
        public NHANVIEN Get (int MaNV)
        {
            db = new QMSEntities();
            return db.NHANVIENs.FirstOrDefault(x => x.MANV == MaNV);
        }

        public NHANVIEN Get (string username)
        {
            db = new QMSEntities();
            return db.NHANVIENs.FirstOrDefault(x => x.USERNAME.Trim().Equals(username));
        }

        public List<ModelSelectItem> GetUserSelect()
        {
            db = new QMSEntities();
            //db.Configuration.ProxyCreationEnabled = false;
            return db.NHANVIENs.OrderBy(x => x.MANV).Select(x => new ModelSelectItem() { Value = x.MANV, Name = x.TENNV }).ToList(); //(Int32)x.MANV có thể gây lỗi
        }

        public NHANVIEN Get(string userName, string password)
        {
            db = new QMSEntities();
            return db.NHANVIENs.Where(x => x.USERNAME.Trim().ToUpper().Equals(userName) && x.PASSWORD.Trim().ToUpper().Equals(password) ).FirstOrDefault();
        }

    }

}