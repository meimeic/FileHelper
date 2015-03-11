using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelper
{
    public class FileNameAttr
    {
        public static readonly int AttrCount=14;  //文件名包含字段数。
        private string _patientId; //病人ID
        private string _clinicType;  //门诊类别(住院、门诊)
        private string _systemType; //系统类别(lis、emr、pacs等)
        private string _admissonDate; //入院时间(精确到天)
        private string _lisDept;  //(科室代码 lis的科室时自有的)
        private string _visitTimes; //住院次数()
        private string _documentCode;  //文档名称编码
        private string _dischargeDate;  //出院时间
        private string _documentType;  // 文档类型（纸张大小）
        private string _idNo;    //病人身份证号
        private string _sectionNo;  // 小组号
        private string _reportDate;  //报告的审核时间
        private string _serialNo;   //报告的申请单号
        private string _sequenceNo;  //报告包含的项目数
        private string _fileNameString;  //文件全名
        private bool _isLegal;  //文件名是否合法

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
            get { return this._sequenceNo; }
            set { this._sequenceNo = value; }
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
