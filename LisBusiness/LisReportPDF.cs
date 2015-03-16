using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Configuration;
using DBUtility;
using FileHelper;
using log4net;

namespace LisBusiness
{
    public abstract class LisReportPDF
    {
        static readonly ILog LOG = LogManager.GetLogger(typeof(LisReportPDF));

        protected List<FileNameAttr> getPDFFromFS(string filePath, string fileName)
        {
            MyFoler mf = new MyFoler(filePath);
            return ListOperate(mf.GetSpecificFileNameAttrs(fileName));
        }
        protected virtual string getCheckPath(DataRow dr)
        {
            string checkPath = "";
            if (dr["patno"] == null || dr["patno"].ToString() == "")
            {
                LOG.Error("申请单号为:" + dr["serialno"].ToString()+",小组号为:"+dr["sectionno"].ToString()+ "的记录对应病人id为空！！！");
                return null;
            }
            else if (dr["sicktypeno"] == null && dr["sicktypeno"].ToString() == "")
            {
                LOG.Error("申请单号为:" + dr["serialno"].ToString() + ",小组号为:" + dr["sectionno"].ToString() + "的记录对应门诊类型为空！！！");
                return null;
            }
            //住院
            else if (Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 1 || Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 4)
            {
                if (dr["hospitalizedtimes"] != null && dr["hospitalizedtimes"].ToString() != "")
                {
                    checkPath = Path.Combine(Record.GetLisHosPathRoot(), dr["patno"].ToString(), string.Format("{0:D3}", dr["hospitalizedtimes"]), "lis");
                    if (MyFoler.CheckPath(checkPath))
                    {
                        return checkPath;
                    }
                    else
                    {
                        LOG.Warn("病案号为:" + dr["patno"].ToString() + ",门诊类型为:住院," + "住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",申请单号为:" + dr["serialno"].ToString() + ",对应的路径不存在！！");
                        //pdf路径不存在 
                        return null;
                    }
                }
                else
                {
                    LOG.Error("申请单号为:" + dr["serialno"].ToString() + "的住院报告记录对应的住院次数为空！！！");
                    return null;
                }
            }
            //门诊
            else if (Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 2 || Int32.Parse(dr["sicktypeno"].ToString().Trim()) == 3)
            {
                checkPath = Path.Combine(Record.GetLisClinicPathRoot(), string.Format("{0:yyyy-MM-dd}", dr["checkdate"]));
                if (MyFoler.CheckPath(checkPath))
                {
                    return checkPath;
                }
                else
                {
                    LOG.Warn("病案号为:" + dr["patno"].ToString() + ",门诊类型为:门诊," + "审核日期为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + ",对应的路径不存在！!");
                    return null;
                }
            }
            else
            {
                LOG.Error("申请单号为:" + dr["serialno"].ToString() + "门诊类型为:" + dr["sicktypeno"].ToString() + "的记录对应门诊类型未知！！！");
                //未知门诊类型
                return null;
            }
        }
        protected virtual string getFileName(DataRow dr)
        {
            return "*" + "_"+string.Format("{0:yyyyMMdd}", dr["checkdate"]) +"_"+ dr["sectionno"].ToString() + "_" + dr["serialno"].ToString() + "_" + dr["zdy1"].ToString();
        }

        protected virtual List<FileNameAttr> ListOperate(List<FileNameAttr> fileNameList)
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
        
        private static DataTable getReportFormTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        protected virtual string getReportSQLWhere(List<string> serialNos)
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
        private string getReportSQLString(string where)
        {
            return "select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname from reportform" + where;
        }
        protected DataTable getReportRecorde(string where)
        {
            string sql = getReportSQLString(where);
            return getReportFormTable(sql);
        }
    }
}
