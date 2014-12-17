using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FileHelper
{
    public class MyFile : SuperFile
    {
        protected string extension;
        private string fileName;
        public MyFile()
        { }
        public MyFile(FileInfo f)
        {
            this.Name = f.Name;
            this.extension = f.Extension;
            this.lastWriteTime = f.LastWriteTime;
            this.fileName = f.Name.Replace(f.Extension, "");
        }
        public MyFile(string path)
        {
            FileInfo f = new FileInfo(path);
            this.Name = f.Name;
            this.extension = f.Extension;
            this.lastWriteTime = f.LastWriteTime;
            this.fileName = f.Name.Replace(f.Extension, "");
        }
        public FileNameAttr GetNameAttr()
        {
            FileNameAttr fna = new FileNameAttr();
            fna.SetFileNameString(this.fileName);
            string[] nameArry = this.fileName.Split(new char[] {'_'});
            if (nameArry.Length != FileNameAttr.AttrCount)
            {
                //存在非法文件名--写入日志
                fna.SetLegal(false);
                return fna;
            }
            else
            {
                fna.SetLegal(true);

                //设置属性
                fna.PatientId = nameArry[0];
                fna.ClinicType = nameArry[1];
                fna.SystemType = nameArry[2];
                fna.AdmissonDate = nameArry[3];
                fna.LisDept = nameArry[4];
                fna.VisitTimes = nameArry[5];
                fna.DocumentCode = nameArry[6];
                fna.DischargeDate = nameArry[7];
                fna.DocumentType = nameArry[8];
                fna.IdNo = nameArry[9];
                fna.SectionNo = nameArry[10];
                fna.ReportDate = nameArry[11];
                fna.SerialNo =int.Parse(nameArry[12].ToString());
                fna.SequenceNo = nameArry[13];
                return fna;
            }
        }
        //获取文件名
        //private static string GetName(string fullFileName)
        //{
        //    string[] temp = fullFileName.Split(new char[] { '.' });
        //    if (temp.Length == 2)
        //    {
        //        return temp[0];
        //    }
        //    else
        //    {
        //        //这里应该抛出异常
        //        return "";
        //    }
        //}
        ////获取文件扩展
        //private static string GetExtension(string fullFileName)
        //{
        //    string[] temp = fullFileName.Split(new char[] { '.' });
        //    if (temp.Length == 2)
        //    {
        //        return "."+temp[1];
        //    }
        //    else
        //    {
        //        //这里应该抛出异常
        //        return "";
        //    }
        //}
        public DateTime  GetLastWriteTime()
        {
            return this.lastWriteTime;
        }
        public string GetFileName(bool hasExtension)
        {
            if (hasExtension)
            {
                return this.fileName;
               
            }
            else
            {
                return this.Name;
            }
        }
        public string GetFileName()
        {
            return this.GetFileName(true);
        }
        //包含点(.)
        public string GetExtension()
        {
            return this.extension;
        }
    }
}
