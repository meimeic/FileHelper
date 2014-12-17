using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LisDocumentCheck
{
    class PreDo
    {
        private string path;
        //查看检查路径是否合法
        public static bool IsSubFolder(string path)
        {
          
            if (path.Contains(Record.GetPathRoot()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //相对路径
        public static bool CheckPath(string path)
        {
            //检查该路径是否审核过
            bool isCheck = Record.IsChecked(path);
            if (isCheck)
            {
                //该目录已审核
                return false;
            }
            else
            {
                //该目录未审核
                return true;
            }
        }
        public static string GetCheckPath(DataRow  dr)
        {

        }

    }
}
