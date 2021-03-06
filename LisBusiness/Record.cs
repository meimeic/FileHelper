﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace LisBusiness
{
    class Record
    {
        private static SortedList mySortedList = new SortedList();
        private static string LisHospitalRootPath = ConfigurationManager.AppSettings["HospitalRootPath"];
        private static string LisClinicRootPath = ConfigurationManager.AppSettings["ClinicRootPath"]; //
        private static string LisFilter = ConfigurationManager.AppSettings["LisFilter"];
        //public static void init()
        //{
        //    XDocument myDoc = XDocument.Load("XMLRecord.xml");
        //    XElement root = myDoc.Root;
        //    XElement xRecord = root.Element("record");
        //    XElement xPathRoot = xRecord.Element("pathroot");
        //    pathRoot = xPathRoot.Value;
        //    XElement xChecked = xRecord.Element("checked");
        //    IEnumerable<XElement> xResultSet = xChecked.Elements("item");

        //    XAttribute xCheckedDate;
        //    XAttribute xCheckedPath;
        //    //XAttribute xIsChecked;
        //    //XAttribute xLastCheckName;
        //    foreach (XElement el in xResultSet)
        //    {
        //         xCheckedDate=el.Attribute("checkedDate");
        //         xCheckedPath = el.Attribute("checkedPath");
        //         if (el.Attribute("IsChecked").Value.Equals("0"))
        //         {
        //             Record.checkingPath = el.Attribute("checkedPath").Value;
        //             Record.lastCheckName = el.Attribute("lastCheckName").Value;
        //         }
        //         else
        //         {
        //             mySortedList.Add(Int64.Parse(xCheckedDate.Value), xCheckedPath.Value);
        //         }
        //    }
        //}
        //public static void AddRecord(string path)
        //{
        //    if (!mySet.Contains(path))
        //    {
        //        mySet.Add(path);
        //    }
        //}
        //public static string Intercepts(string path)
        //{
        //    //char[] chars = pathRoot.ToCharArray();
        //    //string s=path.TrimStart(pathRoot.ToCharArray());
        //    //return s;
        //    return path.Replace(Record.pathRoot, "");
        //}
        public static void SetLisHosPathRoot(string pathRoot)
        {
            Record.LisHospitalRootPath = pathRoot;
        }
        public static string GetLisHosPathRoot()
        {
            return Record.LisHospitalRootPath;
        }
        public static string GetLisClinicPathRoot()
        {
            return Record.LisClinicRootPath;
        }
        public static bool IsChecked(string path)
        {
            if (mySortedList.ContainsValue(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string[] GetLisQueryCondition()
        {
            if (Record.LisFilter != null && !Record.LisFilter.Equals(""))
            {
                return Record.LisFilter.Split(new char[] { ';' });
            }
            else
            {
                return null;
            }
        }
    }
}
