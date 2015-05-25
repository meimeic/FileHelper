using System;
using System.IO;
using System.Collections.Generic;
namespace FileHelper
{
    public class LisPDFFoler:SuperFolder
    {
        public LisPDFFoler(DirectoryInfo di)
            : base(di)
        {
        }
        public LisPDFFoler(string path)
            : base(path)
        {
        }
        public List<string> GetAllFileNames() {
            List<string> results=new List<string>();
            string temp;
            foreach (FileInfo fi in DI.GetFiles())
            {
                temp = fi.Name;
                results.Add(temp);
            }
            return results;
        }
        public List<string> GetSpecFileNames(string fileType, string specName)
        {
            List<string> results = new List<string>();
            string temp;
            foreach (FileInfo fi in DI.GetFiles(specName + fileType))
            {
                temp = fi.Name;
                results.Add(temp);
            }
            return results;
        }
        public List<string> GetSpecFileNames(string fileType) {
            return GetSpecFileNames(fileType, "*");
        }
    }
}
