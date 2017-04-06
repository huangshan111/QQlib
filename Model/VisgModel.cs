using System;
using System.Collections.Generic;
using System.Text;

namespace QQLib.Model
{
    public class VisgModel
    {
        public string vsig { get; set; }
        public string ques { get; set; }
    }

    public class VCodeModel
    {
        public string errorCode { get; set; }
        public string randstr { get; set; }
        public string ticket { get; set; }
        public string errMessage { get; set; }

    }
}
