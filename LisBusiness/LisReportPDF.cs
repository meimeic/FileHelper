using System;
using System.Collections.Generic;
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

        protected abstract string getReportSQLWhere();
   
        protected List<string> getPDFFromFS(string filePath, string fileName)
        {
            LisPDFFoler lpf = new LisPDFFoler(filePath);
            return lpf.GetSpecFileNames(".pdf", fileName);
        }
        protected virtual string getCheckPath(DataRow dr)
        {
            string checkPath = "";
            //if (dr["patno"] == null || dr["patno"].ToString() == "")
            //{
            //    LOG.Error("申请单号为:" + dr["serialno"].ToString()+",小组号为:"+dr["sectionno"].ToString()+ "的记录对应病人id为空！！！");
            //    return null;
            //}
            if (dr["sicktypeno"] == null && dr["sicktypeno"].ToString() == "")
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
                    if (SuperFolder.CheckPath(checkPath))
                    {
                        return checkPath;
                    }
                    else
                    {
                        LOG.Error("病案号为:" + dr["patno"].ToString() + ",门诊类型为:住院," + "住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",申请单号为:" + dr["serialno"].ToString() + ",对应的路径不存在！！");
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
                if (SuperFolder.CheckPath(checkPath))
                {
                    return checkPath;
                }
                else
                {
                    LOG.Error("病案号为:" + dr["patno"].ToString() + ",门诊类型为:门诊," + "审核日期为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + ",对应的路径不存在！!");
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
        protected virtual string getFileNameLike(DataRow dr)
        {
            return "*" + "_"+string.Format("{0:yyyyMMdd}", dr["checkdate"]) +"_"+ dr["sectionno"].ToString() + "_" + dr["serialno"].ToString() + "_" + dr["zdy1"].ToString();
        }
        protected virtual string getFileName(DataRow dr)
        {
            if (dr["zdy11"] != null)
            {
                return dr["zdy11"].ToString();
            }
            else
            {
                return null;
            }
        }
        //对获取的文件系统的报告名对象进行预处理操作(包括 清洗、排序等)
        //protected virtual List<FileNameAttr> ListOperate(List<FileNameAttr> fileNameList)
        //{
        //    if (fileNameList.Count != 0)
        //    {
        //        var sortResult = from items in fileNameList where items.GetLegal() == true orderby items.ReportDate descending select items;
        //        return sortResult.ToList();
        //    }
        //    else
        //    {
        //        return fileNameList;
        //    }
        //}
        //从数据库中读取reportform
        private static DataTable getReportFormTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        private string getReportSQLString(string where)
        {
            return "select sicktypeno,patno,hospitalizedtimes,cname,sectionno,checkdate,serialno,zdy1,paritemname,zdy11 from reportform " + where;
        }
        //以数据库为基准,进行检查
        public void CheckBaseOnDB() {
            string sqlWhere = getReportSQLWhere();
            string sql = getReportSQLString(sqlWhere);
            LOG.Info("查询数据库的SQL语句为-->"+sql);
            LOG.Info("开始查询LIS数据库");
            DataTable resultTable = getReportFormTable(sql);
            LOG.Info("查询LIS数据库成功");
            CheckPDF(resultTable);
        }
        public List<IResult> getLisPDFResult() {
            string sqlWhere = getReportSQLWhere();
            string sql = getReportSQLString(sqlWhere);
            LOG.Info("查询数据库的SQL语句为-->" + sql);
            LOG.Info("开始查询LIS数据库");
            DataTable resultTable = getReportFormTable(sql);
            List<IResult> results = CheckPDFResult(resultTable);
            return results;
        }
        protected virtual void CheckPDF(DataTable dt)
        {
            LOG.Info("开始：以数据库记录为基准，查询记录对应PDF文件是否存在");
            string checkPath;
            string fileName;
            foreach (DataRow dr in dt.Rows)
            {
                checkPath = getCheckPath(dr);
                if (checkPath != null)
                {
                    LOG.Info("数据库记录对应路径存在-->" + checkPath);
                    fileName = getFileName(dr);
                    if (fileName != null && fileName != "")
                    {
                        string fullPath = Path.Combine(checkPath, fileName);
                        if (!SuperFile.fileExist(fullPath))
                        {
                            LOG.Error("文件名为-->" + fileName + "，文件全路径为-->" + fullPath + "，的文件不存在！！！");
                        }
                    }
                    else
                    {
                        LOG.Warn("数据库中对应的文件名字段为空！！！");
                        LOG.Info("进入文件名模糊匹配流程");
                        fileName = getFileNameLike(dr);
                        LOG.Info("获得的模糊文件名为-->" + fileName);
                        List<string> fileNames = getPDFFromFS(checkPath, fileName);
                        if (fileNames.Count == 0)
                        {
                            LOG.Error("未找到匹配的PDF文件！！！");
                        }
                        else
                        {
                            LOG.Warn("查找到匹配文件,请核实正确性！请查实数据库中对应的文件名字段为空原因！!");
                        }
                        LOG.Info("退出文件名模糊匹配流程");
                    }
                }
            }
            LOG.Info("结束：以数据库记录为基准，查询记录对应PDF文件是否存在");
        }
        protected virtual List<IResult> CheckPDFResult(DataTable dt)
        {
            LOG.Info("开始：以数据库记录为基准，查询记录对应PDF文件是否存在");
            LisReportResult temp;
            string checkPath, fileName;
            List<IResult> lisResult = new List<IResult>();
            foreach(DataRow dr in dt.Rows){
                temp = new LisReportResult();

                //根据数据路记录填充一些基本信息
                fillListByReportDr(dr, temp);
                checkPath = getCheckPath(dr);
                if (checkPath != null)
                {
                    //填充路径
                    temp.FilePath = checkPath;

                    LOG.Info("数据库记录对应路径存在-->" + checkPath);
                    fileName = getFileName(dr);
                    if (fileName != null && fileName != "")
                    {
                        //填充文件名
                        temp.FileName = fileName;

                        string fullPath = Path.Combine(checkPath, fileName);
                        if (!SuperFile.fileExist(fullPath))
                        {
                            LOG.Error("文件名为-->" + fileName + "，文件全路径为-->" + fullPath + "，的文件不存在！！！");
                            //文件路径存在，文件不存在
                            temp.ReportStatus = 1;
                            temp.FileQuality = 2;
                        }
                        else
                        {
                            //路径存在，文件存在
                            temp.ReportStatus = 0;
                            temp.FileQuality = 0;
                        }
                    }
                    else
                    {
                        LOG.Warn("数据库中对应的文件名字段为空！！！");
                        LOG.Info("进入文件名模糊匹配流程");
                        fileName = getFileNameLike(dr);
                        LOG.Info("获得的模糊文件名为-->" + fileName);
                        List<string> fileNames = getPDFFromFS(checkPath, fileName);
                        if (fileNames.Count == 0)
                        {
                            temp.ReportStatus = 1;
                            temp.FileQuality = 2;
                            LOG.Error("未找到匹配的PDF文件！！！");
                        }
                        else
                        {
                            //路径存在，文件存在（文件质量差）
                            temp.ReportStatus = 0;
                            temp.FileQuality = 1;
                            temp.FileName = fileNames[0];
                            LOG.Warn("查找到匹配文件,请核实正确性！请查实数据库中对应的文件名字段为空原因！!");
                        }
                        LOG.Info("退出文件名模糊匹配流程");
                    }
                }
                    //文件路径不存在
                else
                {
                    temp.ReportStatus = 2;
                    temp.FileQuality = 2;
                }
                lisResult.Add(temp);
            }
            LOG.Info("结束：以数据库记录为基准，查询记录对应PDF文件是否存在");
            return lisResult;
        }
        private void fillListByReportDr(DataRow dr, LisReportResult temp)
        {
            temp.ReportDate = dr["checkdate"].ToString();
            temp.SerialNo = dr["serialno"].ToString();
            temp.SectionNo = dr["sectionno"].ToString();
            temp.PrintName = dr["paritemname"].ToString();
            temp.SickType = dr["sicktypeno"].ToString();
        }
    }
}
