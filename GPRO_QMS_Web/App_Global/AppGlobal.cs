using System.Data.SqlClient;
using System.Web;

namespace QMS_Website.App_Global
{
    public static class AppGlobal
    {
        private static string _connectionstring;
        public static string Connectionstring
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionstring))
                {
                    _connectionstring = Helper.GPRO_Helper.Instance.GetEntityConnectString();
                }
                return _connectionstring;
            }
        }

        private static SqlConnection _sqlConnection;
        public static SqlConnection sqlConnection
        {
            get
            {
                if (_sqlConnection == null)
                {
                    _sqlConnection = GPRO.Core.Hai.DatabaseConnection.Instance.Connect(HttpContext.Current.Server.MapPath("~/Config_XML") + "\\DATA.XML"); //Helper.GPRO_Helper.Instance.GetEntityConnectString();
                }
                return _sqlConnection;
            }
        }

        private static string _videoPath;
        public static string  videoPath
        {
            get
            {
                if (_videoPath == null)
                {
                    _videoPath =  HttpContext.Current.Server.MapPath("~/Videos/") ;  
                }
                return _videoPath;
            }
        }

        private static string _userAvatarPath;
        public static string  userAvatarPath
        {
            get
            {
                if (_userAvatarPath == null)
                {
                    _userAvatarPath = HttpContext.Current.Server.MapPath("~/Areas/Admin/Content/User/Images/");
                }
                return _userAvatarPath;
            }
        }
    }
}