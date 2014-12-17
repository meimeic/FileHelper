using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FileHelper
{
    public class MyFoler:SuperFile
    {
        private MyFile myfile;
        private string path;
        private DirectoryInfo di;
        //private List<MyFoler> subfolders;
        //private MyFoler parentfolder;
        public MyFoler()
        {}
        public MyFoler(string path)
        {
            di = new DirectoryInfo(path);
            this.path = path;
            this.Name = di.Name;
            this.lastWriteTime = di.LastWriteTime;
        }
        public MyFoler(DirectoryInfo di)
        {
            this.di = di;
            this.Name = di.Name;
            this.path = di.FullName;
            this.lastWriteTime = di.LastWriteTime;
        }
        //获取文件夹下的文件名信息集合
        public List<FileNameAttr> GetFileNameAttrs(string fileType,int searchType)
        {
            List<FileNameAttr> fileNameList = new List<FileNameAttr>();
            if (searchType == 1)
            {
                foreach (FileInfo fi in di.GetFiles("*" + fileType, SearchOption.TopDirectoryOnly))
                {
                    myfile = new MyFile(fi);
                    fileNameList.Add(myfile.GetNameAttr());
                }
                return fileNameList;
            }
            //else if (searchType == 2)
            //{
 
            //}
            else
            {
                foreach (FileInfo fi in di.GetFiles("*"+fileType, SearchOption.AllDirectories))
                {
                    myfile = new MyFile(fi);
                    fileNameList.Add(myfile.GetNameAttr());
                }
                return fileNameList;
            }
        }
        public List<FileNameAttr> GetFileNameAttrs(string fileType)
        {
            return this.GetFileNameAttrs(fileType, 1);
        }
        public List<FileNameAttr> GetFileNameAttrs()
        {
            return this.GetFileNameAttrs(".pdf", 1);
        }
        //获取指定文件名的文件名信息集合
        public List<FileNameAttr> GetSpecificFileNameAttrs(string fileNameStr,string fileType)
        {
            List<FileNameAttr> fileNameList = new List<FileNameAttr>();

            foreach (FileInfo fi in di.GetFiles(fileNameStr + fileType, SearchOption.TopDirectoryOnly))
            {
                this.myfile = new MyFile(fi);
                fileNameList.Add(this.myfile.GetNameAttr());
            }
            return fileNameList;
        }
        public List<FileNameAttr> GetSpecificFileNameAttrs(string fileNameStr)
        {
            return GetSpecificFileNameAttrs(fileNameStr, ".pdf");
        }
        //获取 文件夹下的MyFile文件信息。
        public List<MyFile> GetFiles()
        {
            List<MyFile> myFileList = new List<MyFile>();
             DirectoryInfo di = new DirectoryInfo(path);
             foreach (FileInfo fi in di.GetFiles())
             {
                 myfile = new MyFile(fi);
                 myFileList.Add(myfile);
             }
             return myFileList;
        }
        public List<MyFile> GetMyFiles(string fileType)
        {
            List<MyFile> myFileList = new List<MyFile>();
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo fi in di.GetFiles("*"+fileType))
            {
                myfile = new MyFile(fi);
                myFileList.Add(myfile);
            }
            return myFileList;
        }
        public MyFile GetFile(string fileName)
        {
            foreach (FileInfo fi in di.GetFiles(fileName))
            {
                myfile = new MyFile(fi);
            }
            return myfile;
        }
        public int GetFileCount()
        {
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo fi in di.GetFiles())
            {
                i++;
            }
            return i;
        }
        public int GetFileCount(string fileType)
        {
            int i = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo fi in di.GetFiles("*"+fileType))
            {
                i++;
            }
            return i;
        }
    }
}
