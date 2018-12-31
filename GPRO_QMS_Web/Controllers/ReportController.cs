using OfficeOpenXml;
using OfficeOpenXml.Style;
using QMS_System.Data.BLL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
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
            return Json(BLLUserEvaluate.Instance.GetReport(userId, from, to));
        }
        public JsonResult GetReport_NotUseQMS(int userId, DateTime from, DateTime to)
        {
            return Json(BLLUserEvaluate.Instance.GetReport_NotUseQMS(userId, from, to));
        }

        public ActionResult DailyReport()
        {
            return View();
        }

       

        public JsonResult GetDailyReport_NotUseQMS()
        {
            return Json(BLLUserEvaluate.Instance.GetDailyReport_NotUseQMS( ));
        }

        public JsonResult GetDailyReport ()
        {
            return Json(BLLUserEvaluate.Instance.GetDailyReport ());
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
                if(useQMS)
                  reportObj = BLLUserEvaluate.Instance.GetReport(userId, dfrom, dto);
                else
                    reportObj = BLLUserEvaluate.Instance.GetReport_NotUseQMS(userId, dfrom, dto);
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
                            worksheet.Cells[col, (5+iii)].Value = reportObj[i].Details[iii].Id;
                            worksheet.Cells[col, (5 + iii)].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            if (i==0)
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

    }
}