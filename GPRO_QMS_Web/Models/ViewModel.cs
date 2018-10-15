using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.Models
{
    public class ViewModel
    {
        public string Date  { get; set; }
        public string  Time { get; set; }
        public int TotalCar { get; set; }
        public int TotalCarServed { get; set; }
        public int TotalCarWaiting { get; set; }
        public int TotalCarProcessing { get; set; }
        public List<ViewDetailModel> Details { get; set; }
        public ViewModel()
        {
            Details = new List<ViewDetailModel>();
        }
    }
}