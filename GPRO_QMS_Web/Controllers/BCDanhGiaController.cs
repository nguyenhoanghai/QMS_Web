using OfficeOpenXml;
using OfficeOpenXml.Style;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class BCDanhGiaController : Controller
    {
        // GET: BCDanhGia
        public ActionResult ChatLuongPhucVuNV()
        {
            return View();
        }

        #region Xe_NangSuatDV_TGTrungBinhNV
        public ActionResult Xe_NangSuatDV_TGTrungBinhNV()
        {
            return View();
        }

        public JsonResult Xe_GetNSPhucVu_TGTrungBinh(string fromDate, string toDate, int type)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            if (type == 1) // theo nv
                return Json(BLLReport.Instance.BaoCaoNangSuatDichVuVaThoiGianTrungBinh_TheoNV(App_Global.AppGlobal.sqlConnection, f, t, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString())));
            //theo dich vu
            return Json(BLLReport.Instance.BaoCaoNangSuatDichVuVaThoiGianTrungBinh_TheoDV(App_Global.AppGlobal.sqlConnection, f, t, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString())));
        }

        public void Xe_GetNSPhucVu_TGTrungBinh_Excel(string from, string to, int type)
        {
            try
            {
                var fi = new FileInfo(Server.MapPath(@"~\Report Template\Mau_NangSuatDV_TGTrungBinh.xlsx"));
                var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dto = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                using (var package = new ExcelPackage(fi))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.First();
                    List<ReportModel> reportObj = null;
                    switch (type)
                    {
                        case 1:
                            reportObj = BLLReport.Instance.BaoCaoNangSuatDichVuVaThoiGianTrungBinh_TheoNV(AppGlobal.sqlConnection, dfrom, dto, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString()));
                            worksheet.Cells[4, 2].Value = "Nhân viên";
                            break;
                        case 2:
                            reportObj = BLLReport.Instance.BaoCaoNangSuatDichVuVaThoiGianTrungBinh_TheoDV(AppGlobal.sqlConnection, dfrom, dto, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString()));
                            worksheet.Cells[4, 2].Value = "Dịch vụ";
                            break;
                    }

                    worksheet.Cells[2, 1].Value = "Từ " + dfrom.ToString("HH:mm") + " ngày " + dfrom.ToString("dd/MM/yyyy") + " - " + dto.ToString("HH:mm") + " ngày " + dto.ToString("dd/MM/yyyy");
                    int row = 5;
                    if (reportObj != null && reportObj.Count > 0)
                    {
                        for (int i = 0; i < reportObj.Count; i++)
                        {
                            worksheet.Cells[row, 1].Value = (i + 1);
                            worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 2].Value = (type == 1 ? reportObj[i].UserName : reportObj[i].ServiceName);
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 3].Value = reportObj[i].Number;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 4].Value = reportObj[i].TongTGChoTB.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 5].Value = reportObj[i].TGChoTruocSC.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 6].Value = reportObj[i].TGChoSauSC.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            row++;
                        }
                    }

                    Response.ClearContent();
                    Response.BinaryWrite(package.GetAsByteArray());
                    DateTime dateNow = DateTime.Now;
                    string fileName = "BaoCaoNangSuatDV_TGTrungBinh_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.ContentType = "application/excel";
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Xe_ChiTietTungPhieu 
        public ActionResult Xe_ChiTietTungPhieu()
        {
            return View();
        }

        public JsonResult Xe_GetNSPhucVu_ChiTietTungPhieu(string fromDate, string toDate )
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
             return Json(BLLReport.Instance.BaoCaoNangSuatDichVuChiTietTheoPhieu(App_Global.AppGlobal.sqlConnection, f, t, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString())));
        }
        public void Xe_GetNSPhucVu_ChiTietTungPhieu_Excel(string from, string to )
        {
            try
            {
                var fi = new FileInfo(Server.MapPath(@"~\Report Template\Mau_NangSuatDV_TheoTungPhieu.xlsx"));
                var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var dto = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                using (var package = new ExcelPackage(fi))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.First();
                    List<ReportModel> reportObj = null; 
                            reportObj = BLLReport.Instance.BaoCaoNangSuatDichVuChiTietTheoPhieu(AppGlobal.sqlConnection, dfrom, dto, Convert.ToInt32(ConfigurationManager.AppSettings["ThuNgan"].ToString()));
                         
                    worksheet.Cells[2, 1].Value = "Từ " + dfrom.ToString("HH:mm") + " ngày " + dfrom.ToString("dd/MM/yyyy") + " - " + dto.ToString("HH:mm") + " ngày " + dto.ToString("dd/MM/yyyy");
                    int row = 5;
                    if (reportObj != null && reportObj.Count > 0)
                    {
                        for (int i = 0; i < reportObj.Count; i++)
                        {
                            worksheet.Cells[row, 1].Value = (i + 1);
                            worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 2].Value =   reportObj[i].STT_PhongKham ;
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 3].Value = reportObj[i].ServiceName;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 4].Value = reportObj[i].Start.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 5].Value = reportObj[i].TongTGChoTB.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 6].Value = reportObj[i].ServeTime.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 7].Value = reportObj[i].TGXuLyTT.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 8].Value = reportObj[i].TGChoTruocSC.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 9].Value = reportObj[i].TGChoSauSC.Value.ToString(@"HH\:mm\:ss");
                            worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 10].Value = reportObj[i].UserName;
                            worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 11].Value = reportObj[i].PhatSinh ? "Có":"Không";
                            worksheet.Cells[row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            row++;
                        }
                    }

                    Response.ClearContent();
                    Response.BinaryWrite(package.GetAsByteArray());
                    DateTime dateNow = DateTime.Now;
                    string fileName = "BaoCaoNangSuatDV_ChiTietTungPhieu_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.ContentType = "application/excel";
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}