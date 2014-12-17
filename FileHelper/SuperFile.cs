using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace FileHelper
{
    public  class SuperFile : IFile
    {
         protected DateTime lastWriteTime;
         protected string Name;
        public bool IsFile(string path)
        {
            if (CheckPath(path))
            {
                FileInfo F = new FileInfo(path);
                if ((F.Attributes & FileAttributes.Directory) != 0)
                {
                    //文件夹
                    return false;
                }
                else
                {
                    //文件
                    return true;
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public string GetFileName(string path)
        {
            if (IsFile(path))
            {
                FileInfo F = new FileInfo(path);
                return F.Name;
            }
            else 
            {
                DirectoryInfo di = new DirectoryInfo(path);
                return di.Name;
            }
        }
        //检查路径是否合法
        public bool CheckPath(string path)
        {
            return true;
        }
    }
}
