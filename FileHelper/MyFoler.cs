using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FileHelper
{
    public class MyFoler:SuperFolder
    {
        private MyFile myfile;
        public MyFoler(string path)
            : base(path)
        {
        }
        public MyFoler(DirectoryInfo di)
            : base(di)
        {
        }
        //获取指定文件夹下的文件名信息集合
        public List<FileNameAttr> GetFileNameAttrs(string fileType,int searchType)
        {
            List<FileNameAttr> fileNameList = new List<FileNameAttr>();
            if (searchType == 1)
            {
                foreach (FileInfo fi in DI.GetFiles("*" + fileType, SearchOption.TopDirectoryOnly))
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
                foreach (FileInfo fi in DI.GetFiles("*"+fileType, SearchOption.AllDirectories))
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
        //获取指定文件夹下某些的文件名信息集合
        public List<FileNameAttr> GetSpecificFileNameAttrs(string fileNameStr,string fileType)
        {
            List<FileNameAttr> fileNameList = new List<FileNameAttr>();

            foreach (FileInfo fi in DI.GetFiles(fileNameStr + fileType, SearchOption.TopDirectoryOnly))
            {
                this.myfile = new MyFile(fi);
                fileNameList.Add(this.myfile.GetNameAttr());
            }
            return fileNameList;
        }
        //搜寻pdf扩展的文件
        public List<FileNameAttr> GetSpecificFileNameAttrs(string fileNameStr)
        {
            return GetSpecificFileNameAttrs(fileNameStr, ".pdf");
        }
        //获取指定文件夹下的MyFile文件信息。
        public List<MyFile> GetFiles()
        {
            List<MyFile> myFileList = new List<MyFile>();
             foreach (FileInfo fi in DI.GetFiles())
             {
                 myfile = new MyFile(fi);
                 myFileList.Add(myfile);
             }
             return myFileList;
        }
        public List<MyFile> GetMyFiles(string fileType)
        {
            List<MyFile> myFileList = new List<MyFile>();
            foreach (FileInfo fi in DI.GetFiles("*"+fileType))
            {
                myfile = new MyFile(fi);
                myFileList.Add(myfile);
            }
            return myFileList;
        }
        public MyFile GetFile(string fileName)
        {
            foreach (FileInfo fi in DI.GetFiles(fileName))
            {
                myfile = new MyFile(fi);
            }
            return myfile;
        }
        //获取指定文件夹下的文件数
        public int GetFileCount()
        {
            int i = 0;
            foreach (FileInfo fi in DI.GetFiles())
            {
                i++;
            }
            return i;
        }
        //获取指定文件夹的指定类型的文件数
        public int GetFileCount(string fileType)
        {
            int i = 0;
            foreach (FileInfo fi in DI.GetFiles("*"+fileType))
            {
                i++;
            }
            return i;
        }
        public int GetSpecificFileNameCount(string fileName,string fileType)
        {
            int i = 0;
            foreach (FileInfo fi in DI.GetFiles(fileName + fileType))
            {
                i++;
            }
            return i;
        }
    }
}
