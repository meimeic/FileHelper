using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelper;
using System.Data;
using DBUtility;

namespace LisDocumentCheck
{
   //文件逻辑处理类
    class Business:SuperBusiness
    {
        private string currentPath;
        private MyFoler folder;
        private List<FileNameAttr> myFileList;
        private string fileType;
        public Business()
        {
            this.currentPath = Record.GetPathRoot();
            this.folder = new MyFoler(this.currentPath);
        }
        public Business(string path)
        {
            this.currentPath = path;
            this.folder = new MyFoler(path);
            this.fileType = ".pdf";
        }
        public Business(string path,string fileType)
        {
            this.currentPath = path;
            this.folder = new MyFoler(path);
            this.fileType = fileType;
        }
        public string CurrentPath
        {
            get { return this.currentPath; }
        }
        public void SetCurrentPath(string path)
        {
            this.currentPath = path;
            this.folder = new MyFoler(path);
            this.myFileList = null;
        }
        public void SetFileType(string fileType)
        {
            this.fileType = fileType;
            this.myFileList = null;
        }
        //
        private void init()
        {
            if (this.folder == null)
            {
                this.folder = new MyFoler(this.currentPath);
            }
            if (fileType != null && !fileType.Equals(""))
            {
                this.myFileList = this.folder.GetFileNameAttrs(this.fileType);
            }
                //如果filetype为null则
            else
            {
                this.myFileList = this.folder.GetFileNameAttrs();
            }
        }
        private List<FileNameAttr> GetFromFileSystem()
        {
            if (this.fileType != null && !this.fileType.Equals(""))
            {
                return folder.GetFileNameAttrs(this.fileType);
            }
            else
            {
                return folder.GetFileNameAttrs(".pdf");
            }
        }
       
        //从数据库中获取已审核数据
        private DataTable GetFromDB(string checkDate)
        {
            string sql = "select PatNo,DeptNo,SectionNo,SerialNo from ReportForm CheckDate=cast('" + checkDate.Trim().ToString() + "') and statusno=1 order by convert(int,serialno) asc";
            return DbHelperSQL.Query(sql).Tables[0];
        }
        public override void Check()
        {
            //1代表以文件系统中文件名为基准
            //2代表以数据库内容为基准（默认）
            //3代表双向均要检测

            //筛选后的list
            List<FileNameAttr> fileNameList = ListOperate();  //筛选
            //判断是否有文件
            if (HaveCorrectFile(fileNameList))
            {
                //如果有进行比较
                DataTable dt = GetFromDB("2013-01-19");
                this.CheckBaseDB(fileNameList, dt);
            }
            else
            {
                //没有则不进行比较
            }
        }
        private void CheckBaseDB(List<FileNameAttr> fileNameList,DataTable dt)
        {
            int listIndex = 0;
            for(int i=0;i<dt.Rows.Count;i++)
            {
                if (listIndex < fileNameList.Count)
                {
                    while (int.Parse(dt.Rows[i]["serialno"].ToString()) > fileNameList[listIndex].SerialNo)
                    {
                        //该pdf记录在数据库里没有
                        listIndex++;
                    }
                    if (int.Parse(dt.Rows[i]["serialno"].ToString()) == fileNameList[listIndex].SerialNo)
                    {
                        //数据库-pdf均有此条记录
                        listIndex++;
                    }
                    else
                    {
                        //数据库里记录不存在pdf;
                    }
                }
                else
                {
                    //数据库里的记录不存在对应的pdf;
                }
            }
        }
        //清洗，排序
        private List<FileNameAttr> ListOperate(List<FileNameAttr> fileNameList)
        {
            if (fileNameList != null && fileNameList.Count != 0)
            {
                var sortResult = from items in fileNameList where items.GetLegal() == true orderby items.SerialNo ascending select items;
                return sortResult.ToList();
            }
            else
            {
                return null;
            }
        }
        private List<FileNameAttr> ListOperate()
        {
            if (this.myFileList == null)
            {
                if (this.fileType != null && !this.fileType.Equals(""))
                {
                    this.myFileList = folder.GetFileNameAttrs(this.fileType);
                }
                else
                {
                    this.myFileList = folder.GetFileNameAttrs(".pdf");
                }
            }
            if(this.myFileList.Count==0)
            {
                return null;
            }
            else
            {
                var sortResult = from items in this.myFileList where items.GetLegal() == true orderby items.SerialNo ascending select items;
                return sortResult.ToList();
            }
        }
        
        //当前文件夹是否存在文件
        public bool HaveFile()
        {
            if (folder.GetFileCount() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool HaveFile(string fileType)
        {
            if (folder.GetFileCount(fileType) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //当前文件夹是否存在合法文件
        public bool HaveCorrectFile()
        {
            if (ListOperate()!=null&&ListOperate().Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool HaveCorrectFile(List<FileNameAttr> fileNameList)
        {
            if (fileNameList != null && fileNameList.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void test()
        {
            List<FileNameAttr> fileNameList = ListOperate(GetFromFileSystem());
        }
    }
}
