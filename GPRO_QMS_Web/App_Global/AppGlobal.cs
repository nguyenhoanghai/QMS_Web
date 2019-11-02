using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}