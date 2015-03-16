using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LisBusiness
{
    public class LisSerialResult:IResult
    {
        private static readonly string _resultType = "lis";
        private string _serialNo;
        private bool _hasPDF;
        public string SerialNo
        {
            get { return this._serialNo; }
            set { this._serialNo = value; }
        }
        public bool PDFFlag
        {
            get { return this._hasPDF; }
            set { this._hasPDF = value; }
        }

    }
}
