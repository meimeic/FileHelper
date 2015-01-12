using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelper
{
    public class FileNameAttr
    {
        public static readonly int AttrCount=14;
        private string _patientId;
        private string _clinicType;
        private string _systemType;
        private string _admissonDate;
        private string _lisDept;
        private string _visitTimes;
        private string _documentCode;
        private string _dischargeDate;
        private string _documentType;
        private string _idNo;
        private string _sectionNo;
        private string _reportDate;
        private string _serialNo;
        private string _sequenceNo;
        private string _fileNameString;
        private bool _isLegal;

        public FileNameAttr()
        {
            this._isLegal = true;
        }
        public string PatientId
        {
            get { return _patientId; }
            set { _patientId = value; }
        }
        public string ClinicType
        {
            get { return _clinicType; }
            set { _clinicType = value; }
        }
        public string SystemType
        {
            get { return _systemType; }
            set { _systemType = value; }
        }
        public string AdmissonDate
        {
            get { return _admissonDate; }
            set { _admissonDate = value; }
        }
        public string LisDept
        {
            get { return _lisDept; }
            set { _lisDept = value; }
        }
        public string VisitTimes
        {
            get { return _visitTimes; }
            set { _visitTimes = value; }
        }
        public string DocumentCode
        {
            get { return _documentCode; }
            set { _documentCode = value; }
        }
        public string DischargeDate
        {
            get { return _dischargeDate; }
            set { _dischargeDate = value; }
        }
        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }
        public string IdNo
        {
            get { return _idNo; }
            set { _idNo = value; }
        }
        public string SectionNo
        {
            get { return _sectionNo; }
            set { _sectionNo = value; }
        }
        public string ReportDate
        {
            get { return _reportDate; }
            set { _reportDate = value; }
        }
        public string SerialNo
        {
            get { return _serialNo; }
            set { _serialNo = value; }
        }
        public string SequenceNo
        {
            get { return _sequenceNo; }
            set { _sequenceNo = value; }
        }
        public void SetFileNameString(string s)
        {
            this._fileNameString = s;
        }
        public string GetFileNameString()
        {
            return this._fileNameString;
        }
        public void SetLegal(bool b)
        {
           this._isLegal = b;
        }
        public bool GetLegal()
        {
            return this._isLegal;
        }
    }
}
