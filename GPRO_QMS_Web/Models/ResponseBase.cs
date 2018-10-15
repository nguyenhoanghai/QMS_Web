using GPRO.Core.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPRO_QMS_Web.Models
{
    public class ResponseBase
    {
        public bool IsSuccess { get; set; }
        public List<Error> Errors { get; set; }
        public dynamic Data { get; set; }
        public dynamic Records { get; set; }
        public ResponseBase()
        {
            Errors = new List<Error>();
        }
    }
}