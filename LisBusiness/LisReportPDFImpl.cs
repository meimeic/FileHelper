using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBUtility;
using FileHelper;
using log4net;

namespace LisBusiness
{
    public class LisReportPDFImpl : LisReportPDF
    {
        static readonly ILog LOG = LogManager.GetLogger(typeof(LisReportPDFImpl));

        public List<IResult> LisReport(string serialNo)
        {
            List<string> serialNos = new List<string>();
            serialNos.Add(serialNo);
            return LisReports(serialNos);
        }
        public List<IResult> LisReports(List<string> serialNos)
        {
            LisReportResult temp = null;
            string where = getReportSQLWhere(serialNos);
            //申请单表单
            LOG.Info("开始---->查询lis数据库");
            DataTable ReportForm = getReportRecorde(where);
            LOG.Info("结束---->查询lis数据库");
            //结果
            List<IResult> lisResult = new List<IResult>();
            DataRow[] Reportdr;
            List<FileNameAttr> fileNameList;
            LOG.Info("开始---->核对每个申请单是否存在相应路径下的PDF");
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
                    LOG.Error("申请单号为:" + serialNo + "在lis系统中不存在！！！");
                }
                //申请号对应一个报告
                else if (Reportdr.Length == 1)
                {
                    DataRow dr = Reportdr[0];
                    //获取可能存在的文件路径
                    string checkPath = getCheckPath(dr);
                    //路径存在
                    if (checkPath != null)
                    {
                        fileNameList = getPDFFromFS(checkPath, getFileName(dr));
                        if (fileNameList.Count != 0)
                        {
                            FileNameAttr fna = fileNameList[0];
                            temp.FileName = fna.GetFileNameString();
                            temp.FilePath = checkPath;
                            fillListByReportDr(dr, temp, 0);
                            lisResult.Add(temp);
                            LOG.Info("病案号为:" + dr["patno"].ToString() + ",住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",审核时间为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + "存在合法的pdf文件," + "文件路径为:" + temp.FilePath + "\\" + fna.GetFileNameString());
                        }
                        //未生成pdf
                        else
                        {
                            LOG.Warn("病案号为:" + dr["patno"].ToString() + ",住院次数为:" + Convert.ToString(dr["hospitalizedtimes"]) + ",审核时间为:" + dr["checkdate"].ToString() + ",申请单号为:" + dr["serialno"].ToString() + "不存再合法的pdf文件！！");
                            fillListByReportDr(dr, temp, 1);
                            lisResult.Add(temp);
                        }
                    }
                    //路径不存在
                    else
                    {
                        fillListByReportDr(dr, temp, 2);
                        lisResult.Add(temp);
                    }
                }
                //一个申请单对应多个报告
                else
                {
                    LOG.Error("申请单号为：" + serialNo + "  对应多个报告，暂时跳过！！！");
                }
            }
            LOG.Info("结束---->核对每个申请单是否存在相应路径下的PDF");
            return lisResult;
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
        private DataRow[] getRows(DataTable dt, string serialNo)
        {
            string where = "serialno ='" + serialNo + "'";
            return dt.Select(where);

        }
    }
}
