using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GPRO_QMS_Web.Models;
namespace GPRO_QMS_Web.Models
{
    public class ServiceInfoModel: DICHVU
    {
        public string ServiceName { get; set; }
        public int TicketNumberProcessing { get; set; }
        public int TotalCarsWaiting { get; set; }

    }
}