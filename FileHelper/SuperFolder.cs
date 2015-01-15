using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FileHelper
{
    public class SuperFolder:IFile
    {
        private string _name;
        private string _typeName;
        private DirectoryInfo _di;
        private string _path;
        private SuperFolder()
        {
            this._typeName = "文件夹";
        }
        public SuperFolder(DirectoryInfo di)
            : this()
        {
            this._di = di;
            this._path = di.FullName;
        }
        public SuperFolder(string path)
            : this()
        {
            this._path = path;
            this._di = new DirectoryInfo(path);
        }
        //检查路径是否存在
        public static bool CheckPath(string path)
        {
            return Directory.Exists(path);
        }
        public string Name
        {
            set { this._name = value; }
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
            get { return this._path; }
        }
    }
}
