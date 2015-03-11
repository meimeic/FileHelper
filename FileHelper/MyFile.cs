using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FileHelper
{
    public class MyFile : SuperFile
    {
        //文件名
        private string _fileName;
        public MyFile(FileInfo f)
            : base(f)
        {
            this._fileName = f.Name.Replace(f.Extension, "");
        }
        public MyFile(string path)
            : base(path)
        {
            this._fileName = this.Name.Replace(this.Extension, "");
        }
        public string FileName
        {
            get { return this._fileName; }
        }
        public FileNameAttr GetNameAttr()
        {
            FileNameAttr fna = new FileNameAttr();
            fna.SetFileNameString(this._fileName);
            string[] nameArry = this._fileName.Split(new char[] {'_'});
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
                fna.PatientId = nameArry[0] == "" ? "%" : nameArry[0];
                fna.ClinicType = nameArry[1] == "" ? "%" : nameArry[1];
                fna.SystemType = nameArry[2] == "" ? "%" : nameArry[2];
                fna.AdmissonDate = nameArry[3] == "" ? "%" : nameArry[3];
                fna.LisDept = nameArry[4] == "" ? "%" : nameArry[4];
                fna.VisitTimes = nameArry[5] == "" ? "%" : nameArry[5];
                fna.DocumentCode = nameArry[6] == "" ? "%" : nameArry[6];
                fna.DischargeDate = nameArry[7] == "" ? "%" : nameArry[7];
                fna.DocumentType = nameArry[8] == "" ? "%" : nameArry[8];
                fna.IdNo = nameArry[9] == "" ? "%" : nameArry[9];
                fna.ReportDate = nameArry[10] == "" ? "%" : nameArry[10];
                fna.SectionNo = nameArry[11] == "" ? "%" : nameArry[11];
                fna.SerialNo = nameArry[12] == "" ? "%" : nameArry[12];
                fna.SequenceNo = nameArry[13] == "" ? "%" : nameArry[13];
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
    }
}
