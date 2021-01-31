using System.Web.Mvc;
using System.Web.Routing;

namespace GPRO_QMS_Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
            name: "lg",
            url: "DangNhap",
            defaults: new { controller = "DangNhap", action = "Login" });


            routes.MapRoute(
               name: "tc",
               url: "Hien-Thi-Quay",
               defaults: new { controller = "HienThiQuay", action = "Index" });
            routes.MapRoute(
               name: "bv",
               url: "MH1",
               defaults: new { controller = "HienThiQuay", action = "Benhvien", id = UrlParameter.Optional });
            routes.MapRoute(
               name: "bv_2",
               url: "MH2",
               defaults: new { controller = "HienThiQuay", action = "Benhvien_2", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "MH_3",
                url: "MH3",
                defaults: new { controller = "HienThiQuay", action = "ManHinhCoVideo", id = UrlParameter.Optional });
            routes.MapRoute(
              name: "MH_4",
              url: "MH4",
              defaults: new { controller = "HienThiQuay", action = "vinhcat", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "huunghi1",
                url: "huunghi1",
                defaults: new { controller = "huunghi", action = "lcd1", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "huunghi2",
                url: "huunghi2",
                defaults: new { controller = "huunghi", action = "lcd2", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "huunghi3",
                url: "huunghi3",
                defaults: new { controller = "huunghi", action = "lcd3", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "thongtinKH",
                url: "thongtinKH",
                defaults: new { controller = "Xe_TT", action = "ThongTinKH", id = UrlParameter.Optional });
            routes.MapRoute(
               name: "RHM_DT",
               url: "RHM1",
               defaults: new { controller = "BVRangHamMat", action = "LCD1", id = UrlParameter.Optional });
            routes.MapRoute(
              name: "RHM_KB",
              url: "RHM2",
              defaults: new { controller = "BVRangHamMat", action = "LCD2", id = UrlParameter.Optional });


            routes.MapRoute(
                name: "tkct",
                url: "ThongKeChiTiet",
                defaults: new { controller = "ThongKe", action = "index" });
            routes.MapRoute(
               name: "tkth",
               url: "ThongKeTongHop",
               defaults: new { controller = "ThongKe", action = "index_TH" });
            routes.MapRoute(
                name: "bc",
                url: "Bao-cao-1",
                defaults: new { controller = "Report", action = "dailyreport" });
            routes.MapRoute(
                name: "bc_ct",
                url: "Bao-cao-danh-gia-chi-tiet",
                defaults: new { controller = "Report", action = "DailyDetailsReport" });
            routes.MapRoute(
                name: "tk",
                url: "bao-cao-2",
                defaults: new { controller = "Report", action = "index" });
            routes.MapRoute(
                name: "ex",
                url: "Excel",
                defaults: new { controller = "Report", action = "Excel", userId = UrlParameter.Optional, from = UrlParameter.Optional, to = UrlParameter.Optional });
            routes.MapRoute(
                name: "xe-baocaochatluongphucvu",
                url: "xe-baocaochatluongphucvu",
                defaults: new { controller = "bcdanhgia", action = "ChatLuongPhucVuNV", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "xe-baocaonangsuatdichvuvathoigiantrungbinh",
                url: "xe-baocaonangsuatdichvuvathoigiantrungbinh",
                defaults: new { controller = "bcdanhgia", action = "xe_NangSuatDV_TGTrungBinhNV", id = UrlParameter.Optional });
            routes.MapRoute(
                name: "xe-baocaochitiettungphieu",
                url: "xe-baocaochitiettungphieu",
                defaults: new { controller = "bcdanhgia", action = "Xe_ChiTietTungPhieu", id = UrlParameter.Optional });


            routes.MapRoute(
              name: "honghanhdanhgia",
              url: "MH-Danh-Gia",
              defaults: new { controller = "Home", action = "DanhGia_TienThu" });
            routes.MapRoute(
                name: "danhgia",
                url: "MH-Danh-Gia-2",
                defaults: new { controller = "Home", action = "dg_teso_thanglong" });
            routes.MapRoute(
              name: "danhgia3",
              url: "Danh-Gia-3",
              defaults: new { controller = "danhgia", action = "tenbutton" });


            routes.MapRoute(
               name: "tracuu",
               url: "Tracuu",
               defaults: new { controller = "DangKyOnline", action = "FindInformation", id = UrlParameter.Optional }
           );


            routes.MapRoute(
                name: "dsdg",
                url: "DanhMucDanhGia",
                defaults: new { controller = "Evaluate", action = "index" }).DataTokens["area"] = "Admin";
            routes.MapRoute(
                name: "dsnv",
                url: "DanhMucNhanVien",
                defaults: new { controller = "User", action = "index" }).DataTokens["area"] = "Admin";
            routes.MapRoute(
                name: "dstn",
                url: "DanhMucTinNhan",
                defaults: new { controller = "ReceiverSMS", action = "index" }).DataTokens["area"] = "Admin";

            routes.MapRoute(
               name: "dg",
               url: "Nhan-Vien",
               defaults: new { controller = "Home", action = "Index_N" }
           );


            routes.MapRoute(
                name: "ketnoicsdl",
                url: "ketnoicsdl",
                defaults: new { controller = "SQLConnect", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
             // defaults: new { controller = "HienThiQuay", action = "ManHinhCoVideo", id = UrlParameter.Optional }
             // defaults: new { controller = "HienThiQuay", action = "Index", id = UrlParameter.Optional }
             //defaults: new { controller = "huunghi", action = "lcd2", id = UrlParameter.Optional } 
             //huu nghi
             // defaults: new { controller = "tv", action = "tv1", id = UrlParameter.Optional } //viet thai quan

             //rang ham mat 
             defaults: new { controller = "BVRangHamMat", action = "LCD1", id = UrlParameter.Optional } 
            );
        }
    }
}
