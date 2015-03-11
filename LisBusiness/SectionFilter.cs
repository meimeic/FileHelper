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
        public static List<string> FiltrateBySectionNos(List<string> serialNos,int sectionNo)
        {
            DataTable dt = getSerialNosTable(serialNos, sectionNo);
            return SerialTableToList(dt); 
        }
        public static List<string> FiltrateBySectionNos(string startDate, string endDate, int sectionNo)
        {
            DataTable dt = getSerialNosTable(startDate, endDate, sectionNo);
            return SerialTableToList(dt); 
        }
        private static List<string> SerialTableToList(DataTable dt)
        {
            List<string> temp = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                temp.Add(dr["serialno"].ToString());
            }
            return temp;
        }
        private static DataTable getSerialNosTable(List<string> serialNos,int sectionNo)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append("select serialno from reportform");
            sqlstr.Append(" where serialno in(");
            foreach (string serialNo in serialNos)
            {
                sqlstr.Append("'" + serialNo + "',");
            }
            sqlstr.Remove(sqlstr.Length - 1, 1);
            sqlstr.Append(")");
            sqlstr.Append(" and section=");
            sqlstr.Append(sectionNo);
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sqlstr.ToString()).Tables[0];
        }
        private static DataTable getSerialNosTable(string startDate, string endDate, int sectionNo)
        {
            StringBuilder sqlstr = new StringBuilder();
            sqlstr.Append("select serialno from reportform");
            if (sectionNo == 10)
            {
                sqlstr.Append(" where sendertime2>'");
                sqlstr.Append(startDate);
                sqlstr.Append("' and sendertime2<'");
                sqlstr.Append(endDate);
                sqlstr.Append("' and sectionno=");
                sqlstr.Append(sectionNo);
            }
            else 
            {
                sqlstr.Append(" where checkdate>'");
                sqlstr.Append(startDate);
                sqlstr.Append("' and checkdate<'");
                sqlstr.Append(endDate);
                sqlstr.Append("' and sectionno=");
                sqlstr.Append(sectionNo);
            }
            DbHelperSQL.connectionstring = ConfigurationManager.ConnectionStrings["LisMSSQLConnectionString"].ConnectionString.ToString();
            return DbHelperSQL.Query(sqlstr.ToString()).Tables[0];
        }
    }
}
