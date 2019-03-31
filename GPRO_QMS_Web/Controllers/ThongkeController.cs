using OfficeOpenXml;
using OfficeOpenXml.Style;
using QMS_System.Data.BLL;
using QMS_System.Data.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QMS_Website.Controllers
{
    public class ThongkeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        #region Chi tiet
        public JsonResult GetObj(string type)
        {
            var list = new List<ModelSelectItem>();
            list.Add(new ModelSelectItem() { Id = 0, Name = "Chọn tất cả" });
            List<ModelSelectItem> listObj = null;
            switch (type)
            {
                case "1":
                    listObj = BLLUser.Instance.GetLookUp(); break;
                case "2":
                    listObj = BLLMajor.Instance.GetLookUp(); break;
                case "3":
                case "4":
                    listObj = BLLService.Instance.GetLookUp(); break;
            }
            if (listObj != null && listObj.Count > 0)
                foreach (var item in listObj)
                    list.Add(new ModelSelectItem() { Id = item.Id, Name = item.Name });
            return Json(list);
        }

        public JsonResult GetReport(int objId, int typeOfSearch, DateTime from, DateTime to, int thungan)
        {
            try
            {
                var objs = (typeOfSearch == 4 ? BLLReport.Instance.DetailReport_DichVuTienThu(objId, thungan, from, to) : BLLReport.Instance.DetailReport(objId, typeOfSearch, from, to));
                return Json(objs);
            }
            catch (Exception)
            {
            }
            return Json("[]");
        }

        public void Excel(int objId, int typeOfSearch, string from, string to, int thungan)
        {
            try
            {
                if (typeOfSearch == 4)
                    //draw mau tien thu
                    DrawDetailsTienThu_CT(objId, from, to, thungan);
                else
                {
                    var fi = new FileInfo(Server.MapPath(@"~\Report Template\TK.xlsx"));
                    var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var dto = DateTime.ParseExact(to, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                    var titles = "STT,số phiếu,nhân viên,nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ";
                    titles += "|STT,số phiếu,nhân viên,quầy,nghiệp vụ,giờ lấy phiếu,giờ giao dịch,giờ kết thúc,TG giao dịch,TG chờ";
                    titles += "|STT,số phiếu,dịch vụ,giờ lấy phiếu,trạng thái";
                    using (var package = new ExcelPackage(fi))
                    {
                        var workbook = package.Workbook;
                        var worksheet = workbook.Worksheets.First();
                        var reportObj = BLLReport.Instance.DetailReport(objId, typeOfSearch, new DateTime(dfrom.Year, dfrom.Month, dfrom.Day), new DateTime(dto.Year, dto.Month, dto.Day, 23, 59, 00));
                        int row = 4;

                        #region Draw header
                        string[] title = null;
                        switch (typeOfSearch)
                        {
                            case 1: title = titles.Split('|')[0].Split(',').ToArray(); break;
                            case 2: title = titles.Split('|')[1].Split(',').ToArray(); break;
                            case 3: title = titles.Split('|')[2].Split(',').ToArray(); break;
                        }
                        worksheet.Cells[2, 2, 2, title.Length + 1].Merge = true;
                        worksheet.Cells[2, 2, 2, title.Length + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        worksheet.Cells[3, 2].Value = "Từ ngày: " + dfrom.ToString("dd/MM/yyyy") + " đến ngày: " + dto.ToString("dd/MM/yyyy") + " tổng giao dịch: " + reportObj.Count;
                        worksheet.Cells[3, 2, 3, title.Length + 1].Merge = true;
                        worksheet.Cells[3, 2, 3, title.Length + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        for (int i = 0; i < title.Length; i++)
                        {
                            worksheet.Cells[row, (2 + i)].Value = title[i];
                            worksheet.Cells[row, (2 + i)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, (2 + i)].Style.Font.Bold = true;
                            worksheet.Cells[row, (2 + i)].Style.Font.Color.SetColor(Color.FromArgb(255, 250, 240));
                            worksheet.Cells[row, (2 + i)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, (2 + i)].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 0, 255));
                        }
                        #endregion

                        if (reportObj.Count > 0)
                        {
                            row = 5;
                            switch (typeOfSearch)
                            {
                                case 1: row = DrawNV(worksheet, reportObj, row); break;
                                case 2: row = DrawNgVu(worksheet, reportObj, row); break;
                                case 3: row = DrawDVu(worksheet, reportObj, row); break;
                            }
                        }

                        Response.ClearContent();
                        Response.BinaryWrite(package.GetAsByteArray());
                        DateTime dateNow = DateTime.Now;
                        string fileName = "ThongKeChiTiet_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                        Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                        Response.ContentType = "application/excel";
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void DrawDetailsTienThu_CT(int objId, string from, string to, int thungan)
        {
            try
            {
                var fi = new FileInfo(Server.MapPath(@"~\Report Template\Mau_DV_CT.xlsx"));
                var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var dto = DateTime.ParseExact(to, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                using (var package = new ExcelPackage(fi))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.First();
                    var reportObj = BLLReport.Instance.DetailReport_DichVuTienThu(objId, thungan, dfrom, dto);

                    worksheet.Cells[2, 1].Value = "Từ " + dfrom.ToString("HH:mm") + " ngày " + dfrom.ToString("dd/MM/yyyy") + " - " + dto.ToString("HH:mm") + " ngày " + dto.ToString("dd/MM/yyyy");
                    int row = 5;
                    if (reportObj.Count > 0)
                    {
                        foreach (var model in reportObj)
                        {
                            worksheet.Cells[row, 1].Value = model.stt;
                            worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 2].Value = model.Number;
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 3].Value = model.ServiceName;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4].Value = model.PrintTime.ToString("dd/MM/yyyy HH:mm");
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.Start != null)
                                worksheet.Cells[row, 5].Value = model.str_Start;
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.End != null)
                                worksheet.Cells[row, 6].Value = model.str_End;
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.StartTN != null)
                                worksheet.Cells[row, 7].Value = model.str_StartTN;
                            worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.ProcessTime != null)
                                worksheet.Cells[row, 8].Value = model.ProcessTime;
                            worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.WaitingTime != null)
                                worksheet.Cells[row, 9].Value = model.WaitingTime;
                            worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            if (model.WaitingTimeTN != null)
                                worksheet.Cells[row, 10].Value = model.WaitingTimeTN;
                            worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            worksheet.Cells[row, 11].Value = model.StatusName;
                            worksheet.Cells[row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            row++;
                        }
                    }

                    row += 2;
                    string[] title1 = ("Chú thích,Tiêu đề báo cáo,Giờ bắt đầu giao dịch,Giờ kết thúc giao dịch,Giờ thu ngân gọi khách hàng,Thời gian giao dịch,Thời gian chờ sau sửa chữa (phút)").Split(',').ToArray();
                    string[] title2 = ("Phần tô màu xanh,Tiêu đề báo cáo + cần thời gian lấy báo cáo,Giờ bắt đầu lên bàn nâng,Giờ kết thúc giao dịch tại bàn nâng"
                    + ",Giờ thu ngân bấm gọi khách hàng thanh toán trên phần mềm,Khoảng thời gian bắt đầu lên bàn nâng đến lúc kết thúc giao dịch tại bàn nâng"
                    + ",Tính từ lúc kết thúc giao dịch tại bàn nâng đến khi Thu ngân gọi KH").Split(',').ToArray();

                    for (int i = 0; i < title1.Length; i++)
                    {
                        worksheet.Cells[row, 3].Value = title1[i];
                        worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                        worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                        worksheet.Cells[row, 4].Value = title2[i];
                        worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);

                        if (i == 0)
                        {
                            worksheet.Cells[row, 3].Style.Font.Bold = true;
                            worksheet.Cells[row, 4].Style.Font.Bold = true;
                        }
                        row++;
                    }

                    Response.ClearContent();
                    Response.BinaryWrite(package.GetAsByteArray());
                    DateTime dateNow = DateTime.Now;
                    string fileName = "ThongKeChiTiet_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
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

        private static int DrawNV(ExcelWorksheet worksheet, List<ReportModel> reportObj, int row)
        {
            for (int i = 0; i < reportObj.Count; i++)
            {
                worksheet.Cells[row, 2].Value = reportObj[i].stt;
                worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 3].Value = reportObj[i].Number;
                worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 4].Value = reportObj[i].UserName;
                worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 5].Value = reportObj[i].MajorName;
                worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 6].Value = reportObj[i].PrintTime.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (reportObj[i].Start != null)
                    worksheet.Cells[row, 7].Value = reportObj[i].Start.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (reportObj[i].End != null)
                    worksheet.Cells[row, 8].Value = reportObj[i].End.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 9].Value = reportObj[i].ProcessTime;
                worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 10].Value = reportObj[i].WaitingTime;
                worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                row++;
            }
            return row;
        }
        private static int DrawNgVu(ExcelWorksheet worksheet, List<ReportModel> reportObj, int row)
        {
            for (int i = 0; i < reportObj.Count; i++)
            {
                worksheet.Cells[row, 2].Value = reportObj[i].stt;
                worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 3].Value = reportObj[i].Number;
                worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 4].Value = reportObj[i].UserName;
                worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 5].Value = reportObj[i].CounterName;
                worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 6].Value = reportObj[i].MajorName;
                worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 7].Value = reportObj[i].PrintTime.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (reportObj[i].Start != null)
                    worksheet.Cells[row, 8].Value = reportObj[i].Start.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                if (reportObj[i].End != null)
                    worksheet.Cells[row, 9].Value = reportObj[i].End.Value.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 9].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 10].Value = reportObj[i].ProcessTime;
                worksheet.Cells[row, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 11].Value = reportObj[i].WaitingTime;
                worksheet.Cells[row, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                row++;
            }
            return row;
        }
        private static int DrawDVu(ExcelWorksheet worksheet, List<ReportModel> reportObj, int row)
        {
            for (int i = 0; i < reportObj.Count; i++)
            {
                worksheet.Cells[row, 2].Value = reportObj[i].stt;
                worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 3].Value = reportObj[i].Number;
                worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 4].Value = reportObj[i].ServiceName;
                worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 5].Value = reportObj[i].PrintTime.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                worksheet.Cells[row, 6].Value = reportObj[i].StatusName;
                worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                row++;
            }
            return row;
        }

        #endregion

        public ActionResult Index_TH()
        {
            return View();
        }

        public JsonResult GetReportTH(int typeOfSearch, DateTime from, DateTime to, int thungan)
        {
            var objs = typeOfSearch == 4 ? BLLReport.Instance.GeneralReport_DichVuTienThu(0, thungan, from, to) : BLLReport.Instance.GeneralReport(0, typeOfSearch, from, to);
            return Json(objs);
        }

        public void ExcelTH(int typeOfSearch, string from, string to, int thungan)
        {
            if (typeOfSearch == 4)
                //draw mau tien thu
                DrawTienThu_TH(0, from, to, thungan);
            else
            {
                var fi = new FileInfo(Server.MapPath(@"~\Report Template\TK.xlsx"));
                var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var dto = DateTime.ParseExact(to, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var titles = "STT,ĐỐI TƯỢNG,SỐ LƯỢT GIAO DỊCH,TỔNG THỜI GIAN GIAO DỊCH(PHÚT),THỜI GIAN GIAO DỊCH TRUNG BÌNH (PHÚT/GD)";
                using (var package = new ExcelPackage(fi))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.First();
                    var reportObj = BLLReport.Instance.GeneralReport(0, typeOfSearch, dfrom, dto);
                    int row = 4;

                    #region Draw header
                    string[] title = titles.Split(',').ToArray();
                    worksheet.Cells[2, 2, 2, title.Length + 1].Merge = true;
                    worksheet.Cells[2, 2, 2, title.Length + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[3, 2].Value = "Từ ngày: " + dfrom.ToString("dd/MM/yyyy") + " đến ngày: " + dto.ToString("dd/MM/yyyy") + " tổng giao dịch: " + reportObj.Count;
                    worksheet.Cells[3, 2, 3, title.Length + 1].Merge = true;
                    worksheet.Cells[3, 2, 3, title.Length + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    for (int i = 0; i < title.Length; i++)
                    {
                        worksheet.Cells[row, (2 + i)].Value = title[i];
                        worksheet.Cells[row, (2 + i)].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, (2 + i)].Style.Font.Bold = true;
                        worksheet.Cells[row, (2 + i)].Style.Font.Color.SetColor(Color.FromArgb(255, 250, 240));
                        worksheet.Cells[row, (2 + i)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, (2 + i)].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 0, 255));
                    }
                    #endregion

                    if (reportObj.Count > 0)
                    {
                        row = 5;
                        for (int i = 0; i < reportObj.Count; i++)
                        {
                            worksheet.Cells[row, 2].Value = (i+1);
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 3].Value = reportObj[i].Name;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4].Value = reportObj[i].TotalTransaction;
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 5].Value = reportObj[i].TotalTransTime;
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 6].Value = reportObj[i].AverageTimePerTrans;
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            row++;
                        }
                    }

                    Response.ClearContent();
                    Response.BinaryWrite(package.GetAsByteArray());
                    DateTime dateNow = DateTime.Now;
                    string fileName = "ThongKeTongHop_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.ContentType = "application/excel";
                    Response.Flush();
                    Response.End();
                }
            }
        }

        private void DrawTienThu_TH(int objId, string from, string to, int thungan)
        {
            try
            {
                var fi = new FileInfo(Server.MapPath(@"~\Report Template\Mau_DV_TH.xlsx"));
                var dfrom = DateTime.ParseExact(from, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var dto = DateTime.ParseExact(to, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                using (var package = new ExcelPackage(fi))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.First();
                    var reportObj = BLLReport.Instance.GeneralReport_DichVuTienThu(objId, thungan, dfrom, dto);

                    worksheet.Cells[2, 1].Value = "Từ " + dfrom.ToString("HH:mm") + " ngày " + dfrom.ToString("dd/MM/yyyy") + " - " + dto.ToString("HH:mm") + " ngày " + dto.ToString("dd/MM/yyyy");
                    int row = 5;
                    if (reportObj.Count > 0)
                    {
                        foreach (var model in reportObj)
                        {
                            worksheet.Cells[row, 1].Value = (row-4);
                            worksheet.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 2].Value = model.Name;
                            worksheet.Cells[row, 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 3].Value = model.TotalTransaction;
                            worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 4].Value = model.TotalTransTime;
                            worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 5].Value = model.AverageTimePerTrans;
                            worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 6].Value = model.AverageTimeWaitingBeforePerTrans;
                            worksheet.Cells[row, 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, 7].Value = model.AverageTimeWaitingAfterPerTrans;
                            worksheet.Cells[row, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                            row++;
                        }
                    }

                    row += 2;
                    string[] title1 = ("Chú thích,Tiêu đề báo cáo,Số lượt giao dịch,Tổng thời gian giao dịch (phút),Thời gian giao dịch trung bình (phút/gd),Thời gian chờ trước sửa chữa (phút/gd),Thời gian chờ sau sửa chữa (phút/gd)").Split(',').ToArray();
                    string[] title2 = ("Phần tô màu xanh,Tiêu đề báo cáo + cần thời gian lấy báo cáo,(Tính từ lúc lên bàn nâng đến khi xuống bàn nâng của 1 phiếu sửa chữa) Dù chuyển bàn nâng thì cũng chỉ tính là 1 lượt giao dịch,(Tính từ lúc bắt đầu lên bàn nâng đến khi xuống bàn nâng) Nếu chuyển qua nhiều bàn nâng thì tính thời gian lúc lên bàn nâng đến xuống bàn nâng của từng bàn nâng"
                    + ",Tổng thời gian giao dịch (phút) / Số lượt giao dịch,(Tính từ lúc lấy phiếu đến khi xe lên bàn nâng)"
                    + ",(Tính từ lúc xe xuống bàn nâng đến khi Thu ngân gọi khách)").Split(',').ToArray();

                    for (int i = 0; i < title1.Length; i++)
                    {
                        worksheet.Cells[row, 3].Value = title1[i];
                        worksheet.Cells[row, 3].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                        worksheet.Cells[row, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, 3].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                        worksheet.Cells[row, 4].Value = title2[i];
                        worksheet.Cells[row, 4].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                        worksheet.Cells[row, 4].Style.WrapText = true;

                        if (i == 0)
                        {
                            worksheet.Cells[row, 3].Style.Font.Bold = true;
                            worksheet.Cells[row, 4].Style.Font.Bold = true;
                        }
                        row++;
                    }

                    Response.ClearContent();
                    Response.BinaryWrite(package.GetAsByteArray());
                    DateTime dateNow = DateTime.Now;
                    string fileName = "ThongKeTongHop_" + dateNow.ToString("yyMMddhhmmss") + ".xlsx";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName);
                    Response.ContentType = "application/excel";
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception)
            {
            }
        }

    }
}