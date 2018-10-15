using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.Models
{
    public class ModelSelectItem
    { 
        public string Code { get; set; }
        public int Data { get; set; }
        public double Double { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}