using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.Models
{
    public class EvaluateModel : Q_Evaluate
    {
        public List<EvaluateDetailModel> Childs { get; set; }
        public EvaluateModel()
        {
            Childs = new List<EvaluateDetailModel>();
        }
    }
}