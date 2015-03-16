using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LisBusiness
{
    public class LisSerialResult:IResult
    {
        private static readonly string _resultType = "lis";
        private string _patNo;
        private string _cName;
        private string _sickTypeNo;
        private string _visitNo;
        private string _serialNo;
        private string _collectDate;
        private string _inceptDate;
        private string _receiveDate;
        private string _checkDate;
        private bool _hasPDF;
        private string _parItemName;
        private string _fileName;
        public string ResultType
        {
            get { return LisSerialResult._resultType; }
        }
        public string PatNo
        {
            get { return this._patNo; }
            set { this._patNo = value; }
        }
        public string CName
        {
            get { return this._cName; }
            set { this._cName = value; }
        }
        public string SickTypeNo
        {
            get { return this._sickTypeNo; }
            set { this._sickTypeNo = value; }
        }
        public string VisitNo
        {
            get { return this._visitNo; }
            set { this._visitNo = value; }
        }
        public string SerialNo
        {
            get { return this._serialNo; }
            set { this._serialNo = value; }
        }
        public string CollectDate
        {
            get { return this._collectDate; }
            set { this._collectDate = value; }
        }
        public string InceptDate
        {
            get { return this._inceptDate; }
            set { this._inceptDate = value; }
        }
        public string ReceiveDate
        {
            get { return this._receiveDate; }
            set { this._receiveDate = value; }
        }
        public string CheckDate
        {
            get { return this._checkDate; }
            set { this._checkDate = value; }
        }
        public bool PDFFlag
        {
            get { return this._hasPDF; }
            set { this._hasPDF = value; }
        }
        public string ParItemName
        {
            get { return this._parItemName; }
            set { this._parItemName = value; }
        }
        public string FileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }
    }
}
