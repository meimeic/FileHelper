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
namespace LisBusiness
{
   public class LisReportPDFService : LisReportPDF
    {
        static readonly ILog LOG = LogManager.GetLogger(typeof(LisReportPDFService));
        //从lis数据库获取已审核报告记录
        //拼接查询条件
        protected  void CheckOnDB(string startDate,string endDate)
        {
            //从数据库中获取
            string where = getReportSQLWhere(startDate, endDate);
            DataTable dt = getReportRecorde(where);
            List<FileNameAttr> temp;
            string checkPath;
            string fileNameLike;
            foreach (DataRow dr in dt.Rows)
            {
                //路径
                checkPath = getCheckPath(dr);
                //文件名
                fileNameLike = getFileName(dr);
                //文件路径存在
                if (checkPath != null)
                {
                    //清洗数据
                    temp = getPDFFromFS(checkPath, fileNameLike);
                    if (temp.Count == 0)
                    {
                        //没有生成pdf
                        LOG.Warn("病案号为:" + dr["patno"].ToString() + ",住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",审核时间为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + "不存在合法的pdf文件！！");
                    }
                    else if (temp.Count == 1)
                    {
                        //有pdf
                        FileNameAttr result = temp[0];
                        LOG.Info("病案号为:" + dr["patno"].ToString() + ",住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",审核时间为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + "存在合法的pdf文件," + "文件路径为:" + checkPath+"\\"+ result.GetFileNameString());
                        //LOG.Info("开始---->生成的pdf记录写入PDF表");
                        //AddToPDFTable(result, dr, checkPath, "1", "正常");
                        //LOG.Info("结束---->生成的pdf记录写入PDF表");
                    }
                    else
                    {
                        //生成多个pdf
                        FileNameAttr result = temp[0];
                        AddToPDFTable(result, dr, checkPath, "1", "正常");
                        LOG.Error("病案号为:" + dr["patno"].ToString() + ",住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",审核时间为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + "存在多个合法的pdf文件！！！");
                    }
                }
            }
        }

        private void CheckBaseOnFS()
        {
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
        private DataTable getPDFRecord(List<string> serialNos)
        {
            string where = getPDFSQLWhere(serialNos);
            return getPDFTable(getPDFSQLString(where));
        }
        private DataTable getPDFTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["MyMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        protected virtual string getPDFSQLString(string where)
        {
            return "SELECT FileName,FileType,FilePath,SickType,SectionNo,ReportDate,SerialNo,ReportName FROM PDFRecord" + where;
        }
        private string getPDFSQLWhere(List<string> serialNos)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append(" where serialno in(");
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
        private List<FileNameAttr> getPDFFromFS(string filePath, string fileName)
        {
            MyFoler mf = new MyFoler(filePath);
            return ListOperate(mf.GetSpecificFileNameAttrs(fileName));
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

        protected override string getReportSQLWhere(string startDate,string endDate)
        {
            return " where checkdate>'" + startDate + "' and checkdate<'" + endDate + "'";
        }
       //private DataTable getReportFormTable(string sql)
        //{
        //    DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
        //    return DbHelperSQL.Query(sql).Tables[0];
        //}
        //private string getReportSQLWhere(List<string> serialNos)
        //{
        //    StringBuilder sqlstr = new StringBuilder();
        //    sqlstr.Append(" where serialno in(");
        //    foreach (string serialNo in serialNos)
        //    {
        //        sqlstr.Append("'" + serialNo + "',");
        //    }
        //    sqlstr.Remove(sqlstr.Length - 1, 1);
        //    sqlstr.Append(")");
        //    return sqlstr.ToString();
        //}
        //protected virtual string getReportSQLString(string where)
        //{
        //    return "select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname from reportform" + where;
        //}
        //private DataTable getReportRecorde(List<string> serialNos)
        //{
        //    string where = getReportSQLWhere(serialNos);
        //    return getReportFormTable(getReportSQLString(where));
        //}
    }
}
