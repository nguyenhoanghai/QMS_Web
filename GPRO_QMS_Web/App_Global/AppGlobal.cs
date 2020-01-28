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

    }
}