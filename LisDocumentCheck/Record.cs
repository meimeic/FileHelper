using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LisDocumentCheck
{
    class Record
    {
        private static SortedList mySortedList = new SortedList();
        private static string pathRoot;
        private static string checkingPath; //当前检查路径
        private static string lastCheckName; //最后检查元素
        public static void init()
        {
            XDocument myDoc = XDocument.Load("XMLRecord.xml");
            XElement root = myDoc.Root;
            XElement xRecord = root.Element("record");
            XElement xPathRoot = xRecord.Element("pathroot");
            pathRoot = xPathRoot.Value;
            XElement xChecked = xRecord.Element("checked");
            IEnumerable<XElement> xResultSet = xChecked.Elements("item");

            XAttribute xCheckedDate;
            XAttribute xCheckedPath;
            //XAttribute xIsChecked;
            //XAttribute xLastCheckName;
            foreach (XElement el in xResultSet)
            {
                 xCheckedDate=el.Attribute("checkedDate");
                 xCheckedPath = el.Attribute("checkedPath");
                 if (el.Attribute("IsChecked").Value.Equals("0"))
                 {
                     Record.checkingPath = el.Attribute("checkedPath").Value;
                     Record.lastCheckName = el.Attribute("lastCheckName").Value;
                 }
                 else
                 {
                     mySortedList.Add(Int64.Parse(xCheckedDate.Value), xCheckedPath.Value);
                 }
            }
        }
        //public static void AddRecord(string path)
        //{
        //    if (!mySet.Contains(path))
        //    {
        //        mySet.Add(path);
        //    }
        //}
        public static string Intercepts(string path)
        {
            //char[] chars = pathRoot.ToCharArray();
            //string s=path.TrimStart(pathRoot.ToCharArray());
            //return s;
            return path.Replace(Record.pathRoot, "");
        }
        public static void SetPathRoot(string pathRoot)
        {
            Record.pathRoot = pathRoot;
        }
        public static string GetPathRoot()
        {
            return Record.pathRoot;
        }
        public static bool AddCheckedFolder(bool addXML)
        {
            if (mySortedList != null)
            {
                long checkDate = DateTime.Now.Ticks;
                mySortedList.Add(checkDate.ToString(), Record.checkingPath);
                if (addXML)
                {
                    SaveToXML();
                }
                return true;
            }
            else
            {
                return false;
            }
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
        public static void SaveToXML()
        {
            XDocument myDoc = XDocument.Load("XMLRecord.xml");
            XElement root = myDoc.Root;
            XElement xRecord = root.Element("record");
            XElement xPathRoot = xRecord.Element("pathroot");
            pathRoot = xPathRoot.Value;
            XElement xChecked = xRecord.Element("checked");
            XElement xNew = new XElement("item", new XAttribute("checkedDate", DateTime.Now.Ticks.ToString()), new XAttribute("checkedPath", Record.checkingPath), new XAttribute("IsChecked", "1"), new XAttribute("lastCheckName",Record.lastCheckName));
            xChecked.Add(xNew);
            myDoc.Save("XMLRecord.xml");
        }
    }
}
