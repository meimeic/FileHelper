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
namespace LisDocumentCheck
{
    class LisBusiness:SuperBusiness
    {
        private DataTable GetFromDB(string checkDate)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            string sql = @"select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname from reportform where CONVERT(varchar(100),receivedate,23)='2013-01-30'";
            return DbHelperSQL.Query(sql).Tables[0];
        }
        public override void Check()
        {
            CheckBaseOnDB();
        }
        private void CheckBaseOnDB()
        {
            //从数据库中获取
            DataTable dt = GetFromDB("");
            string checkPath="";
            List<FileNameAttr> temp;
            string fileNameLike;
            MyFoler mf;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["patno"] == null || dr["patno"].ToString() == "")
                {
                    //抛异常
                    continue;
                }
                if (dr["hospitalizedtimes"] == null || dr["hospitalizedtimes"].ToString() == "")
                {
                    //抛异常
                    continue;
                }
                if (dr["sicktypeno"] != null && dr["sicktypeno"].ToString()!= "")
                {
                    if (Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 1)
                    {
                        string test1 = string.Format("{0:D3}", dr["hospitalizedtimes"]);
                        //住院
                        checkPath = Path.Combine(Record.GetLisHosPathRoot(), dr["patno"].ToString(), string.Format("{0:D3}", dr["hospitalizedtimes"]), "lis");
                    }
                    else
                    {
                        //门诊

                    }
                }
                //检查路径合法性

                //
                mf = new MyFoler(checkPath);
                //文件名
                fileNameLike = "*" + dr["sectionno"].ToString() + "_" + string.Format("{0:yyyyMMdd}", dr["checkdate"]) + "_" + dr["serialno"].ToString() + "_" + dr["zdy1"].ToString();
                //清洗数据
                temp = ListOperate(mf.GetSpecificFileNameAttrs(fileNameLike));

                if (temp.Count == 0)
                {
                    //没有生成pdf,写入数据库
                    FileNameAttr f=new FileNameAttr();
                    f.SetFileNameString(fileNameLike);
                    f.SectionNo = dr["sectionno"].ToString();
                    f.PatientId = dr["patno"].ToString();
                    f.SerialNo = dr["serialno"].ToString();
                    f.SequenceNo = dr["zdy1"].ToString();
                    f.ReportDate = string.Format("{0:yyyyMMdd}", dr["checkdate"]);
                    AddToDB(f, dr, checkPath, "2", "不存在这样的文件名的文件");
                }
                else if (temp.Count == 1)
                {
                    //有pdf
                    FileNameAttr result = temp[0];
                    AddToDB(result, dr, checkPath,"1","正常");

                }
                else
                {
                    //生成多个pdf
                }
            }
 
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
        private bool AddToDB(FileNameAttr result,DataRow dr,string checkPath,string fileStatus,string checkSpec)
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
            parameters[9].Value = DateTime.ParseExact(result.AdmissonDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//入院时间
            parameters[10].Value = result.VisitTimes;//住院次数
            parameters[11].Value = result.DocumentCode;//文件编码
            parameters[12].Value = DateTime.ParseExact(result.DischargeDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//出院时间
            parameters[13].Value = result.DocumentType;//文件大小
            parameters[14].Value = result.IdNo;//身份证号
            parameters[15].Value = result.SectionNo;//小组号
            parameters[16].Value = DateTime.ParseExact(result.ReportDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);//报告时间
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
    }
}
