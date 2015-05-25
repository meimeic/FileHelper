using System;
using System.IO;
namespace FileHelper
{
    public class SuperFolder:IFile
    {
        private string _name;
        private string _typeName;
        private DirectoryInfo _di;
        private string _fullName;
        private SuperFolder()
        {
            this._typeName = "folder";
        }
        public SuperFolder(DirectoryInfo di)
            : this()
        {
            this._di = di;
            this._fullName = di.FullName;
            this._name = di.Name;
        }
        public SuperFolder(string path)
            : this()
        {
            this._fullName = path;
            this._di= new DirectoryInfo(path);
            this._name = this._di.Name;
        }
        //检查路径是否存在
        public static bool CheckPath(string path)
        {
            return Directory.Exists(path);
        }
        public string Name
        {
            get { return this._name; }
        }
        public string TypeName
        {
            get { return this._typeName; }
        }
        public DirectoryInfo DI
        {
            get { return this._di; }
        }
        public string Path
        {
            get { return this._fullName; }
        }
    }
}
