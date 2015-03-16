using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBUtility;
using System.Configuration;
namespace LisBusiness
{
    public class SectionFilter:IFilter
    {
        private static readonly string _filterName = "小组号过滤器";
        public string FilterName
        {
            get { return SectionFilter._filterName; }
        }
        public List<LisSerialResult> FiltrateBySectionNos(List<string> serialNos, int sectionNo)
        {
            string where = getSerialNosWhere(serialNos, sectionNo);
            return this.getLisSerialResult(where);
        }
        public List<LisSerialResult> FiltrateBySectionNos(string startDate, string endDate, int sectionNo)
        {
            string where = getSerialNosWhere(startDate, endDate, sectionNo);
            return this.getLisSerialResult(where);
        }
        public List<LisSerialResult> FiltrateBySectionNos(string patNo, int visitNo, int sectionNo)
        {
            string where = getSerialNosWhere(patNo, visitNo, sectionNo);
            return this.getLisSerialResult(where);
        }
        private List<LisSerialResult> getLisSerialResult(string where)
        {
            DataTable dt = getSerialNosRecord(where);
            return SerialTableToList(dt);
        }
        private static List<LisSerialResult> SerialTableToList(DataTable dt)
        {
            List<LisSerialResult> temp = new List<LisSerialResult>();
            LisSerialResult lsr;
            foreach (DataRow dr in dt.Rows)
            {
                lsr = new LisSerialResult();
                lsr.PatNo = dr["patno"].ToString();
                lsr.CName = Convert.ToString(dr["cname"]);
                lsr.SickTypeNo = Convert.ToString(dr["sicktypeno"]);
                lsr.VisitNo = Convert.ToString(dr["hospitalizedtimes"]);
                lsr.SerialNo = dr["serialno"].ToString();
                if (dr["collectdate"] != null && dr["collectdate"].ToString() != "")
                {
                    lsr.CollectDate = string.Format("{0:yyyy-MM-dd}", dr["collectdate"]);
                }
                else
                {
                    lsr.CollectDate = "";
                }
               
                if (dr["inceptdate"] != null && dr["inceptdate"].ToString() != "")
                {
                    lsr.InceptDate = string.Format("{0:yyyy-MM-dd}", dr["inceptdate"]);
                }
                else
                {
                    lsr.InceptDate = "";
                }
                lsr.ReceiveDate = string.Format("{0:yyyy-MM-dd}", dr["receivedate"]);
                lsr.CheckDate = string.Format("{0:yyyy-MM-dd}", dr["checkdate"]);
                lsr.SerialNo = dr["serialno"].ToString();
                if (Convert.ToString(dr["hissendflag"]).Trim() == "1")
                {
                    lsr.PDFFlag=true;
                }
                else
                {
                    lsr.PDFFlag = false;
                }
                lsr.ParItemName = Convert.ToString(dr["paritemname"]);
                lsr.FileName = Convert.ToString(dr["zdy11"]);
                temp.Add(lsr);
            }
            return temp;
        }
        protected virtual string getSerialNosWhere(List<string> serialNos, int sectionNo)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append(" where serialno in(");
            foreach (string serialNo in serialNos)
            {
                sqlstr.Append("'" + serialNo + "',");
            }
            sqlstr.Remove(sqlstr.Length - 1, 1);
            sqlstr.Append(")");
            if (sectionNo >= 0)
            {
                sqlstr.Append(" and section=");
                sqlstr.Append(sectionNo);
            }
            return sqlstr.ToString();
        }
        protected virtual string getSerialNosWhere(string startDate, string endDate, int sectionNo)
        {
            StringBuilder sqlstr = new StringBuilder();
            if (sectionNo == 10)
            {
                sqlstr.Append(" where sendertime2>'");
                sqlstr.Append(startDate);
                sqlstr.Append("' and sendertime2<'");
                sqlstr.Append(endDate);
                sqlstr.Append("'");
            }
            else
            {
                sqlstr.Append(" where checkdate>'");
                sqlstr.Append(startDate);
                sqlstr.Append("' and checkdate<'");
                sqlstr.Append(endDate);
                sqlstr.Append("'");
            }
            if (sectionNo >= 0)
            {
                sqlstr.Append(" and section=");
                sqlstr.Append(sectionNo);
            }
            sqlstr.Append(" and patno is not null and patno<>''");
            return sqlstr.ToString();
        }
        protected virtual string getSerialNosWhere(string patNo, int visitNo, int sectionNo)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append(" where patNo='");
            sqlstr.Append(patNo);
            sqlstr.Append("' and hospitalizedtimes='");
            sqlstr.Append(visitNo);
            sqlstr.Append("'");
            if (sectionNo >= 0)
            {
                sqlstr.Append(" and section=");
                sqlstr.Append(sectionNo);
            }
            if(sectionNo==10)
            {
                sqlstr.Append(" and sendertime2 is not null and sendertime2 <>''");
            }
            return sqlstr.ToString();
        }
        protected virtual string getSQLStr(string where)
        {
            string sql = "select patno,cname,sicktypeno,hospitalizedtimes,serialno,collectdate,inceptdate,receivedate,checkdate,hissendflag,paritemname,zdy11 from reportform" + where;
            return sql;
        }
        private static DataTable getSerialNosTable(string sql)
        {
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sql).Tables[0];
        }
        protected DataTable getSerialNosRecord(string where)
        {
            string sql = getSQLStr(where);
            return getSerialNosTable(sql);
        }
    }
}
