
namespace LisBusiness
{
    public class LisReportResult:IResult
    {
        private static readonly string _resultType = "lis";
        private int _fileQuality;
        private string _filePath;
        private string _fileName;
        private string _reportDate;
        private int _reportStatus;
        private string _printName;
        private string _serialNo;
        private string _sectionNo;
        private string _sickType;
        public string ResultType
        {
            get { return LisReportResult._resultType; }
        }
        public int FileQuality
        {
            get { return this._fileQuality; }
            set { this._fileQuality = value; }
        }
        public string FilePath
        {
            get { return this._filePath; }
            set { this._filePath = value; }
        }
        public string FileName
        {
            get { return this._fileName; }
            set { this._fileName = value; }
        }
        public string ReportDate
        {
            get { return this._reportDate; }
            set{this._reportDate=value;}
        }
        public int ReportStatus
        {
            get { return this._reportStatus; }
            set { this._reportStatus = value; }
        }
        public string SerialNo
        {
            get { return this._serialNo; }
            set { this._serialNo = value; }
        }
        public string SectionNo
        {
            get { return this._sectionNo; }
            set { this._sectionNo = value; }
        }
        public string SickType
        {
            get { return this._sickType; }
            set { this._sickType = value; }
        }
        public string PrintName
        {
            get { return this._printName; }
            set { this._printName = value; }
        }
    }
}
