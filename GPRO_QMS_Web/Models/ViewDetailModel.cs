using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPRO_QMS_Web.Models
{
   public class ViewDetailModel
    {
        public int STT { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int TicketNumber { get; set; }
        public string CarNumber { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Temp { get; set; }
        public string StartStr { get; set; }
        public string TimeProcess { get; set; }
    }
}
