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
        //文件全名。
         private string _name;
        //类别，文件还是目录。
         private string _typeName;
        //文件扩展类型。
         private string _extension;
         private SuperFile()
         {
             this._typeName = "文件";
         }
         public SuperFile(FileInfo f)
             : this()
         {
             this._name = f.Name;
             this._extension = f.Extension;
         }
         public SuperFile(string path)
             : this()
         {
             FileInfo f = new FileInfo(path);
             this._name = f.Name;
             this._extension = f.Extension;
         }
         public static bool IsFile(string path)
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
         public static bool fileExist(string path)
         {
             return File.Exists(path);
         }
        //获取指定路径下的文件、文件名称
         public static string GetFileName(string path)
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
         public string TypeName
         {
             get { return this._typeName; }
         }
         public string Name
         {
             get { return this._name; }
         }
         public string Extension 
         {
             get { return this._extension; }
         }
    }
}
