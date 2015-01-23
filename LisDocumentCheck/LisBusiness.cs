using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using DBUtility;
using FileHelper;
using System.Configuration;
using log4net;
namespace LisDocumentCheck
{
    class LisBusiness : SuperBusiness
    {
        static readonly ILog LOG = LogManager.GetLogger(typeof(LisBusiness));
        //从lis数据库获取已审核报告记录
        //拼接查询条件
        private string CombineQueryCondition(string[] QueryCondition)
        {
            StringBuilder strCondition = new StringBuilder();
            strCondition.Append(" and sectionno in (");
            strCondition.Append("select distinct sectionno from printform where");
            foreach (string str in QueryCondition)
            {
                strCondition.Append(" printprogram like " + str + " or");
            }
            strCondition.Remove(strCondition.Length - 2, 2);
            strCondition.Append(")");
            return strCondition.ToString();
        }
        protected override void CheckOnDB()
        {
            //从数据库中获取
            string where;
            string[] QueryCondition = Record.GetLisQueryCondition();
            if (QueryCondition != null)
            {
                where = "where CONVERT(varchar(100),receivedate,23)='" + this.CheckCondition + "'" + CombineQueryCondition(QueryCondition);
            }
            else
            {
                where = "where CONVERT(varchar(100),receivedate,23)='" + this.CheckCondition + "'";
            }
            DataTable dt = getReportFormTable(getReportSQLString(where));
            List<FileNameAttr> temp;
            MyFoler mf;
            string checkPath;
            string fileNameLike;
            foreach (DataRow dr in dt.Rows)
            {
                //路径
                checkPath = getCheckPath(dr);
                //文件名
                fileNameLike = getFileName(dr);
                if (checkPath!=null&&checkPath!="")
                {
                    mf = new MyFoler(checkPath);
                    //清洗数据
                    temp = ListOperate(mf.GetSpecificFileNameAttrs(fileNameLike));
                    if (temp.Count == 0)
                    {
                        //没有生成pdf
                        LOG.Info("病案号为：" + dr["patno"].ToString() + "的患者:" + dr["cname"].ToString() + ",申请单号为" + dr["serialno"].ToString() + "小组号为：" + dr["sectionno"].ToString() + ",对应报告未生成PDF文件");
                    }
                    else if (temp.Count == 1)
                    {
                        //有pdf
                        FileNameAttr result = temp[0];
                        AddToPDFTable(result, dr, checkPath, "1", "正常");
                    }
                    else
                    {
                        //生成多个pdf
                        FileNameAttr result = temp[0];
                        AddToPDFTable(result, dr, checkPath, "1", "正常");
                    }
                }
                //路径不存在
                else if (checkPath == "")
                {
                    LOG.Info("病案号为：" + dr["patno"].ToString() + "的患者:" + dr["cname"].ToString() + ",申请单号为" + dr["serialno"].ToString() + "小组号为：" + dr["sectionno"].ToString() + "的记录,未有对应文件夹");
                }
                else
                {
                    LOG.Error("病案号为：" + dr["patno"].ToString() + "的患者:" + dr["cname"].ToString() + ",申请单号为" + dr["serialno"].ToString() + "小组号为：" + dr["sectionno"].ToString() + "的记录存在错误！！");
                }
            }
        }
        public override bool IsChecked()
        {
            return false;
        }
        private void CheckBaseOnFS()
        {
        }
        private List<FileNameAttr> ListOperate(List<FileNameAttr> fileNameList)
        {
            if (fileNameList.Count != 0)
            {
                var sortResult = from items in fileNameList where items.GetLegal() == true orderby items.ReportDate descending select items;
                return sortResult.ToList();
            }
            else
            {
                return fileNameList;
            }
        }
        private bool AddToPDFTable(FileNameAttr result, DataRow dr, string checkPath, string fileStatus, string checkSpec)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["MyMSSQLConnectionString"].ConnectionString.ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO dbo.CheckRecord(");
            strSql.Append("FileName,FileType,FilePath,FileStatus,CheckDate,CheckSpec,PID,SickType,FileClass,AdmissionDate");
            strSql.Append(",VisitID,ReportCode,DischargeDate,PageSize,CID,SectionNo,ReportDate,SerialNo,ItemSum,CName,ReportName)");
            strSql.Append(" values (");
            strSql.Append("@FileName,@FileType,@FilePath,@FileStatus,@CheckDate,@CheckSpec,@PID,@SickType,@FileClass,@AdmissionDate");
            strSql.Append(",@VisitID,@ReportCode,@DischargeDate,@PageSize,@CID,@SectionNo,@ReportDate,@SerialNo,@ItemSum,@CName,@ReportName)");
            SqlParameter[] parameters = {
					new SqlParameter("@FileName", SqlDbType.VarChar,300),
					new SqlParameter("@FileType", SqlDbType.VarChar,10),
					new SqlParameter("@FilePath", SqlDbType.VarChar),
					new SqlParameter("@FileStatus", SqlDbType.VarChar,10),
					new SqlParameter("@CheckDate", SqlDbType.DateTime),
					new SqlParameter("@CheckSpec", SqlDbType.NVarChar),
                    new SqlParameter("@PID", SqlDbType.VarChar,15),
                    new SqlParameter("@SickType", SqlDbType.VarChar,5),
                    new SqlParameter("@FileClass", SqlDbType.VarChar,10),
                    new SqlParameter("@AdmissionDate", SqlDbType.DateTime),
                    new SqlParameter("@VisitID", SqlDbType.VarChar,5),
                    new SqlParameter("@ReportCode", SqlDbType.VarChar,100),
                    new SqlParameter("@DischargeDate", SqlDbType.DateTime),
                    new SqlParameter("@PageSize", SqlDbType.VarChar,10),
                    new SqlParameter("@CID", SqlDbType.VarChar,18),
                    new SqlParameter("@SectionNo", SqlDbType.VarChar,10),
                    new SqlParameter("@ReportDate", SqlDbType.DateTime),
                    new SqlParameter("@SerialNo", SqlDbType.VarChar,20),
                    new SqlParameter("@ItemSum", SqlDbType.Int),
                    new SqlParameter("@CName", SqlDbType.NVarChar,20),
                    new SqlParameter("@ReportName", SqlDbType.NVarChar,100)
                                        };
            parameters[0].Value = result.GetFileNameString(); //filename
            parameters[1].Value = "pdf";//filetype
            parameters[2].Value = checkPath;//filepath
            parameters[3].Value = fileStatus;//filestatus
            parameters[4].Value = DateTime.Now.Date;//checkdate
            parameters[5].Value = checkSpec;//CheckSpec
            parameters[6].Value = result.PatientId;//PID
            parameters[7].Value = result.ClinicType;//门诊类型
            parameters[8].Value = result.SystemType;//文件类型(lis/his/pacs)
            if (result.AdmissonDate != null && !result.AdmissonDate.Equals("%"))
            {
                parameters[9].Value = DateTime.ParseExact(result.AdmissonDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//入院时间
            }
            parameters[10].Value = result.VisitTimes;//住院次数
            parameters[11].Value = result.DocumentCode;//文件编码
            if (result.DischargeDate != null && !result.DischargeDate.Equals("%"))
            {
                parameters[12].Value = DateTime.ParseExact(result.DischargeDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//出院时间
            }
            parameters[13].Value = result.DocumentType;//文件大小
            parameters[14].Value = result.IdNo;//身份证号
            parameters[15].Value = result.SectionNo;//小组号
            if (result.ReportDate != null && !result.ReportDate.Equals("%"))
            {
                parameters[16].Value = DateTime.ParseExact(result.ReportDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//报告时间
            }
            parameters[17].Value = result.SerialNo;//申请单号
            parameters[18].Value = Convert.ToInt32(result.SequenceNo);//项目数
            parameters[19].Value = dr["cname"].ToString();//病人姓名
            parameters[20].Value = dr["paritemname"].ToString();//打印的名称
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool AddToPDFTable(FileNameAttr result, DataRow dr, string checkPath)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["MyMSSQLConnectionString"].ConnectionString.ToString();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO dbo.PDFRecord(");
            strSql.Append("FileName,FileType,FilePath,CheckDate,PID,SickType,FileClass,AdmissionDate");
            strSql.Append(",VisitID,ReportCode,DischargeDate,CID,SectionNo,ReportDate,SerialNo,ItemSum,CName,ReportName)");
            strSql.Append(" values (");
            strSql.Append("@FileName,@FileType,@FilePath,@CheckDate,@PID,@SickType,@FileClass,@AdmissionDate");
            strSql.Append(",@VisitID,@ReportCode,@DischargeDate,@CID,@SectionNo,@ReportDate,@SerialNo,@ItemSum,@CName,@ReportName)");
            SqlParameter[] parameters = {
					new SqlParameter("@FileName", SqlDbType.VarChar,300),
					new SqlParameter("@FileType", SqlDbType.VarChar,10),
					new SqlParameter("@FilePath", SqlDbType.VarChar),
					new SqlParameter("@CheckDate", SqlDbType.DateTime),
                    new SqlParameter("@PID", SqlDbType.VarChar,15),
                    new SqlParameter("@SickType", SqlDbType.VarChar,5),
                    new SqlParameter("@FileClass", SqlDbType.VarChar,10),
                    new SqlParameter("@AdmissionDate", SqlDbType.DateTime),
                    new SqlParameter("@VisitID", SqlDbType.VarChar,5),
                    new SqlParameter("@ReportCode", SqlDbType.VarChar,100),
                    new SqlParameter("@DischargeDate", SqlDbType.DateTime),
                    new SqlParameter("@CID", SqlDbType.VarChar,18),
                    new SqlParameter("@SectionNo", SqlDbType.VarChar,10),
                    new SqlParameter("@ReportDate", SqlDbType.DateTime),
                    new SqlParameter("@SerialNo", SqlDbType.VarChar,20),
                    new SqlParameter("@ItemSum", SqlDbType.Int),
                    new SqlParameter("@CName", SqlDbType.NVarChar,20),
                    new SqlParameter("@ReportName", SqlDbType.NVarChar,100)
                                        };
            parameters[0].Value = result.GetFileNameString(); //filename
            parameters[1].Value = ".pdf";//filetype
            parameters[2].Value = checkPath;//filepath
            parameters[3].Value = DateTime.Now.Date;//checkdate
            parameters[4].Value = result.PatientId;//PID
            parameters[5].Value = result.ClinicType;//门诊类型
            parameters[6].Value = result.SystemType;//文件类型(lis/his/pacs)
            if (result.AdmissonDate != null && !result.AdmissonDate.Equals("%"))
            {
                parameters[7].Value = DateTime.ParseExact(result.AdmissonDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//入院时间
            }
            parameters[8].Value = result.VisitTimes;//住院次数
            parameters[9].Value = result.DocumentCode;//文件编码
            if (result.DischargeDate != null && !result.DischargeDate.Equals("%"))
            {
                parameters[10].Value = DateTime.ParseExact(result.DischargeDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//出院时间
            }
            parameters[11].Value = result.IdNo;//身份证号
            parameters[12].Value = result.SectionNo;//小组号
            if (result.ReportDate != null && !result.ReportDate.Equals("%"))
            {
                parameters[13].Value = DateTime.ParseExact(result.ReportDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//报告时间
            }
            parameters[14].Value = result.SerialNo;//申请单号
            parameters[15].Value = Convert.ToInt32(result.SequenceNo);//项目数
            parameters[16].Value = dr["cname"].ToString();//病人姓名
            parameters[17].Value = dr["paritemname"].ToString();//打印的名称
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override List<IResult> LisReport(string serialNo)
        {
            List<string> serialNos = new List<string>();
            serialNos.Add(serialNo);
            return LisReports(serialNos);
        }
        public override List<IResult> LisReports(List<string> serialNos)
        {
            LisReportResult temp = null;
            //申请单表单
            DataTable ReportForm = getReportRecorde(serialNos);
            DataTable PDFTable =getPDFRecord(serialNos);
            //结果
            List<IResult> lisResult = new List<IResult>();
            DataRow[] Reportdr;
            DataRow[] PDFdr;
            foreach (string serialNo in serialNos)
            {
                temp = new LisReportResult();
                Reportdr = getRows(ReportForm, serialNo);

                //查看该申请单号在lis中是否存在
                if (Reportdr.Length == 0)
                {
                    temp.SerialNo = serialNo;
                    //未有此申请单号
                    temp.ReportStatus = 2;
                    lisResult.Add(temp);
                }

                //申请号对应一个报告
                else if (Reportdr.Length == 1)
                {
                    PDFdr = getRows(PDFTable, serialNo);
                    //判断该申请单号是否有pdf文件记录
                    if (PDFdr.Length > 0)
                    {
                        //有pdf文件记录
                        fillListByPDFDr(PDFdr[0], temp, 0);
                        lisResult.Add(temp);
                    }
                    //没有pdf文件记录
                    else
                    {
                        DataRow dr = Reportdr[0];
                        //获取可能存在的文件路径
                        string checkPath = getCheckPath(dr);
                        //是否有pdf文件
                        if (checkPath != null && checkPath != "")
                        {
                            List<FileNameAttr> fileNameList = getPDFFromFS(checkPath, getFileName(dr));
                            if (fileNameList.Count != 0)
                            {
                                FileNameAttr fna = fileNameList[0];
                                //将已生成pdf文件的记录写入数据库
                                AddToPDFTable(fna, dr, checkPath);
                                temp.FileName = fna.GetFileNameString() + ".pdf";
                                temp.FilePath = checkPath;
                                fillListByReportDr(dr, temp, 0);
                                lisResult.Add(temp);
                            }
                            //未生成pdf
                            else
                            {
                                fillListByReportDr(dr, temp, 1);
                                lisResult.Add(temp);
                            }
                        }
                        //不存在pdf文件路径
                        else
                        {
                            fillListByReportDr(dr, temp, 1);
                            lisResult.Add(temp);
                        }
                    }
                }
                //一个申请单对应多个报告
                else
                {
                    LOG.Info("申请单号为：" + serialNo + "  对应多个报告，暂时跳过！");
                }
            }
            return lisResult;
        }
        //获取已生成pdf的记录
        private DataTable getPDFRecord(List<string> serialNos)
        {
            string sql = getPDFSQLString(serialNos);
            return getPDFTable(sql);
        }
        private DataTable getPDFTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["MyMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        private string getPDFSQLString(List<string> serialNos)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append("SELECT FileName,FileType,FilePath,SickType,SectionNo,ReportDate,SerialNo,ReportName");
            sqlstr.Append(" FROM PDFRecord where serialno in(");
            foreach (string serialNo in serialNos)
            {
                sqlstr.Append("'" + serialNo + "',");
            }
            sqlstr.Remove(sqlstr.Length - 1, 1);
            sqlstr.Append(")");
            return sqlstr.ToString();
        }
        private DataRow[] getRows(DataTable dt, string serialNo)
        {
            string where = "serialno ='" + serialNo + "'";
            return dt.Select(where);

        }
        private List<FileNameAttr> getPDFFromFS(string checkPath, string fileName)
        {
            MyFoler mf = new MyFoler(checkPath);
            return ListOperate(mf.GetSpecificFileNameAttrs(fileName));
        }
        private string getCheckPath(DataRow dr)
        {
            string checkPath = "";
            if (dr["patno"] != null && dr["patno"].ToString() != "" && dr["sicktypeno"] != null && dr["sicktypeno"].ToString() != "")
            {
                if (Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 1 || Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 4)
                {
                    //住院
                    if (dr["hospitalizedtimes"] != null && dr["hospitalizedtimes"].ToString() != "")
                    {
                        checkPath = Path.Combine(Record.GetLisHosPathRoot(), dr["patno"].ToString(), string.Format("{0:D3}", dr["hospitalizedtimes"]), "lis");
                        if (MyFoler.CheckPath(checkPath))
                        {
                            return checkPath;
                        }
                        else
                        {
                            //pdf路径不存在 
                            return "";
                        }
                    }
                    else
                    {
                        //住院次数不存在
                        return null;
                    }
                }
                //门诊
                else if (Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 2 || Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 3)
                {
                    checkPath = Path.Combine(Record.GetLisClinicPathRoot(), string.Format("{0:yyyyMMdd}", dr["checkdate"]));
                    if (MyFoler.CheckPath(checkPath))
                    {
                        return checkPath;
                    }
                    else
                    {
                        //pdf路径不存在
                        return "";
                    }
                }
                else
                {
                    //未知门诊类型
                    return null;
                }
            }
            else
            {
                //病历号、
                return null;
            }
        }
        private string getFileName(DataRow dr)
        {
            return "*" + dr["sectionno"].ToString() + "_" + string.Format("{0:yyyyMMdd}", dr["checkdate"]) + "_" + dr["serialno"].ToString() + "_" + dr["zdy1"].ToString();
        }
        private void fillListByReportDr(DataRow dr, LisReportResult temp, int status)
        {
            temp.ReportDate = dr["checkdate"].ToString();
            temp.SerialNo = dr["serialno"].ToString();
            temp.SectionNo = dr["sectionno"].ToString();
            temp.PrintName = dr["paritemname"].ToString();
            temp.SickType = dr["sicktypeno"].ToString();
            //已审核未发布
            temp.ReportStatus = status;
        }
        private void fillListByPDFDr(DataRow dr, LisReportResult temp, int status)
        {
            temp.FileName = dr["FileName"].ToString() + dr["FileType"].ToString();
            temp.FilePath = dr["FilePath"].ToString();
            temp.ReportDate = dr["ReportDate"].ToString();
            temp.SerialNo = dr["SerialNo"].ToString();
            temp.SectionNo = dr["SectionNo"].ToString();
            temp.PrintName = dr["ReportName"].ToString();
            temp.SickType = dr["SickType"].ToString();
            temp.ReportStatus = status;
        }
        private DataTable getReportFormTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        private string getReportSQLString(List<string> serialNos)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append("select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname");
            sqlstr.Append(" from reportform where serialno in(");
            foreach (string serialNo in serialNos)
            {
                sqlstr.Append("'" + serialNo + "',");
            }
            sqlstr.Remove(sqlstr.Length - 1, 1);
            sqlstr.Append(")");
            return sqlstr.ToString();
        }
        private string getReportSQLString(string where)
        {
            return "select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname from reportform " + where;
        }
        private DataTable getReportRecorde(List<string> serialNos)
        {
            string sql = getReportSQLString(serialNos);
            return getReportFormTable(sql);
        }
    }
}
