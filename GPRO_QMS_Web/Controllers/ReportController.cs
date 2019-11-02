using OfficeOpenXml;
using OfficeOpenXml.Style;
using QMS_System.Data.BLL;
using QMS_Website.App_Global;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GPRO_QMS_Web.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(int userId, DateTime from, DateTime to)
        {
            return Json(BLLUserEvaluate.Instance.GetReport(AppGlobal.Connectionstring, userId, from, to));
        }

        public JsonResult GetReport_NotUseQMS(int userId, DateTime from, DateTime to)
        {
            return Json(BLLUserEvaluate.Instance.GetReport_NotUseQMS(AppGlobal.Connectionstring, userId, from, to));
        }

        public ActionResult DailyReport()
        {
            return View();
        }

        public ActionResult DailyDetailsReport()
        {
            return View();
        }

        public JsonResult GetDailyReport_details(string fromDate, string toDate)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            return Json(BLLUserEvaluate.Instance.GetDailyReport_Detail(AppGlobal.Connectionstring, f, t));
        }

        public void Excel_Dgia_Ctiet(bool useQMS, string fromDate, string toDate)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var fi = new FileInfo(Server.MapPath(@"~\Report Template\BCao_DGia_Ngay_CTiet.xlsx"));
            using (var package = new ExcelPackage(fi))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                worksheet.Cells[3, 11].Value = (fromDate == toDate ? fromDate : (fromDate + " - " + toDate));
                List<ReportEvaluateDetailModel> reportObj = null;
                if (useQMS)
                    reportObj = BLLUserEvaluate.Instance.GetDailyReport_Detail(AppGlobal.Connectionstring, f, t);
                //else
                //    reportObj = BLLUserEvaluate.Instance.GetDailyReport_NotUseQMS(reportForUser);
                if (reportObj.Count > 0)
                {
                    int col = 7;
                    for (int i = 0; i < reportObj.Count; i++)
                    {
                        worksheet.Cells[col, 2].Value = (i + 1);
                        worksheet.Cells[col, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 3].Value = reportObj[i].Number;
                        worksheet.Cells[col, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 4].Value = reportObj[i].UserName;
                        worksheet.Cells[col, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 5].Value = reportObj[i].ServiceName;
                        worksheet.Cells[col, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 6].Value = reportObj[i].PrintTime.ToString("dd/MM/yyyy HH:mm:ss");
                        worksheet.Cells[col, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 7].Value = reportObj[i].EvaluateTime.ToString("dd/MM/yyyy HH:mm:ss");
                        worksheet.Cells[col, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        worksheet.Cells[col, 8].Value = reportObj[i].Score == "1000" && reportObj[i].Comment != null ? reportObj[i].Comment : "";
                        worksheet.Cells[col, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                        switch (reportObj[i].Score)
                        {
                            case "1_1":
                                worksheet.Cells[col, 9].Value = "v";
                                worksheet.Cells[col, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_2":
                                worksheet.Cells[col, 10].Value = "v";
                                worksheet.Cells[col, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_3":
                                worksheet.Cells[col, 11].Value = "v";
                                worksheet.Cells[col, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_4":
                                worksheet.Cells[col, 12].Value = "v";
                                worksheet.Cells[col, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_5":
                                worksheet.Cells[col, 13].Value = "v";
                                worksheet.Cells[col, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_6":
                                worksheet.Cells[col, 14].Value = "v";
                                worksheet.Cells[col, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_7":
                                worksheet.Cells[col, 15].Value = "v";
                                worksheet.Cells[col, 15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_8":
                                worksheet.Cells[col, 16].Value = "v";
                                worksheet.Cells[col, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_9":
                                worksheet.Cells[col, 17].Value = "v";
                                worksheet.Cells[col, 17].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                            case "1_10":
                                worksheet.Cells[col, 18].Value = "v";
                                worksheet.Cells[col, 18].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                                break;
                        }
                        col++;
                    }
                }

                Response.ClearContent();
                Response.BinaryWrite(package.GetAsByteArray());
                DateTime dateNow = DateTime.Now;
                string fileName = "ThongKeDanhGiaChiTiet_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.ContentType = "application/excel";
                Response.Flush();
                Response.End();
            }
        }


        public JsonResult GetDailyReport_NotUseQMS(bool reportForUser, string fromDate, string toDate)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            return Json(BLLUserEvaluate.Instance.GetDailyReport_NotUseQMS(AppGlobal.Connectionstring, reportForUser,f,t));
        }

        public JsonResult GetDailyReport(bool reportForUser, string fromDate, string toDate)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            return Json(BLLUserEvaluate.Instance.GetDailyReport(AppGlobal.Connectionstring, reportForUser,f,t));
        }

        public void Excel(int userId, string from, string to, bool useQMS)
        {
            var fi = new FileInfo(Server.MapPath(@"~\Report Template\Report QMS Template.xlsx"));
            var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dto = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            using (var package = new ExcelPackage(fi))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                List<ReportEvaluateModel> reportObj;
                if (useQMS)
                    reportObj = BLLUserEvaluate.Instance.GetReport(AppGlobal.Connectionstring, userId, dfrom, dto);
                else
                    reportObj = BLLUserEvaluate.Instance.GetReport_NotUseQMS(AppGlobal.Connectionstring, userId, dfrom, dto);
                if (reportObj.Count > 0)
                {
                    int col = 5;
                    for (int i = 0; i < reportObj.Count; i++)
                    {
                        worksheet.Cells[col, 3].Value = (i + 1);
                        worksheet.Cells[col, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[col, 4].Value = reportObj[i].Name;
                        worksheet.Cells[col, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin); 
                        for (int iii = 0; iii < reportObj[i].Details.Count; iii++)
                        {
                            worksheet.Cells[col, (5 + iii)].Value = reportObj[i].Details[iii].Id;
                            worksheet.Cells[col, (5 + iii)].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            if (i == 0)
                            {
                                worksheet.Cells[4, (5 + iii)].Value = reportObj[i].Details[iii].Name.ToUpper();
                                worksheet.Cells[col, (5 + iii)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            }
                        }
                        //worksheet.Cells[col, 6].Value = reportObj[i].tc2;
                        //worksheet.Cells[col, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        //worksheet.Cells[col, 7].Value = reportObj[i].tc3;
                        //worksheet.Cells[col, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        col++;
                    }
                }

                Response.ClearContent();
                Response.BinaryWrite(package.GetAsByteArray());
                DateTime dateNow = DateTime.Now;
                string fileName = "ThongKeDanhGia_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.ContentType = "application/excel";
                Response.Flush();
                Response.End();
            }
        }

        public void Excel_Dgia(bool useQMS, bool reportForUser, string fromDate, string toDate)
        {
            var f = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var t = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            var fi = new FileInfo(Server.MapPath(@"~\Report Template\BCao_DGia_Ngay.xlsx"));
            using (var package = new ExcelPackage(fi))
            {
                var workbook = package.Workbook;
                var worksheet = workbook.Worksheets.First();
                List<ReportEvaluateModel> reportObj;
                if (useQMS)
                    reportObj = BLLUserEvaluate.Instance.GetDailyReport(AppGlobal.Connectionstring, reportForUser,f,t);
                else
                    reportObj = BLLUserEvaluate.Instance.GetDailyReport_NotUseQMS(AppGlobal.Connectionstring, reportForUser,f, t);
                worksheet.Cells[3, 8].Value = (fromDate == toDate ? fromDate : (fromDate + " - " + toDate));
                if (reportObj.Count > 0)
                {
                    int col = 7;
                    for (int i = 0; i < reportObj.Count; i++)
                    {
                        worksheet.Cells[col, 2].Value = (i + 1);
                        worksheet.Cells[col, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[col, 3].Value = reportObj[i].ServiceName;
                        worksheet.Cells[col, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        for (int iii = 0; iii < reportObj[i].Details.Count; iii++)
                        {
                            worksheet.Cells[col, (4 + iii)].Value = reportObj[i].Details[iii].Id;
                            worksheet.Cells[col, (4 + iii)].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            //if (i == 0)
                            //{
                            //    worksheet.Cells[4, (5 + iii)].Value = reportObj[i].Details[iii].Name.ToUpper();
                            //    worksheet.Cells[col, (5 + iii)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            //}
                        }
                        col++;
                    }
                }

                Response.ClearContent();
                Response.BinaryWrite(package.GetAsByteArray());
                DateTime dateNow = DateTime.Now;
                string fileName = "ThongKeDanhGiaNgay_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                Response.ContentType = "application/excel";
                Response.Flush();
                Response.End();
            }
        }

    }
}