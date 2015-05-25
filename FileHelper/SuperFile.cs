using System;
using System.IO;

namespace FileHelper
{
    public class SuperFile : IFile
    {
        //文件全名。
         private string _name;
        //类别，文件还是目录。
         private string _typeName;
         private string _fullname; 
         private SuperFile()
         {
             this._typeName = "文件";
         }
         public SuperFile(FileInfo f)
             : this()
         {
             this._name = f.Name;
             this._fullname = f.FullName;
         }
         public SuperFile(string path)
             : this()
         {
             FileInfo f = new FileInfo(path);
             this._name = f.Name;
             this._fullname = f.FullName;
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
         public static string Rename(string path,string oriName,string desName)
         {
             string fullName = Path.Combine(path, oriName);
             string fulldesName=desName.Trim()+".pdf";
             long startTime = DateTime.Now.Ticks;
             long currentTime;
             long runTime;
             while (!fileExist(fullName))
             {
                 //do nothing
                 currentTime = DateTime.Now.Ticks;
                 runTime=currentTime-startTime;
                 if (runTime>100000)
                 {
                     return "error--超时！";
                 }
             }
             FileInfo f = new FileInfo(fullName);
             f.MoveTo(Path.Combine(path, fulldesName));
             return "ok--成功！";
         }
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
         public string FullName{
             get { return this._fullname; }
         }
    }
}
