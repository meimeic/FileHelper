using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DBUtility;
using FileHelper;
namespace LisDocumentCheck
{
    class LisBusiness:SuperBusiness
    {
        private DataTable GetFromDB(string checkDate)
        {
            string sql = "select PatNo DeptNo SectionNo SerialNo from ReportForm CheckDate=cast('" + checkDate.Trim().ToString() + "') and statusno=1 order by convert(int,serialno) asc";
            return DbHelperSQL.Query(sql).Tables[0];
        }
        public override void Check()
        {
            
        }
        private void CheckBaseOnDB()
        {
            //从数据库中获取
            DataTable dt = GetFromDB("");
            string checkPath;
            List<FileNameAttr> temp;
            string fileNameLike;
            MyFoler mf;
            foreach (DataRow dr in dt.Rows)
            {
                checkPath = "";//检查路径
                //检查路径合法性

                //
                mf = new MyFoler(checkPath);
                fileNameLike = "";
                temp = ListOperate(mf.GetSpecificFileNameAttrs(fileNameLike));
                if (temp.Count == 0)
                {
                    //没有生成pdf,写入数据库
                }
                else if (temp.Count == 1)
                {
                    //有pdf

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
                var sortResult = from items in fileNameList where items.GetLegal() == true orderby items.SerialNo ascending select items;
                return sortResult.ToList();
            }
            else
            {
                return fileNameList;
            }
        }
    }
}
