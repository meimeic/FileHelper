using System;
using System.Collections.Generic;
using System.Text;

namespace LisBusiness
{
    public class SerialFilter : DecoratorFilter
    {
        private HashSet<string> serials = new HashSet<string>();
        public void AddSerial(string serial)
        {
            this.serials.Add(serial);
        }
        public void RemoveSerial(string serial)
        {
            this.serials.Remove(serial);
        }
        public void Clear()
        {
            this.serials.Clear();
        }
        public override string GetFilter()
        {
            string temp = base.GetFilter();
            string result = temp + serialWhere();
            return result;
        }
        private string serialWhere()
        {
            if (this.serials.Count == 0)
            {
                return "";
            }
            else
            {
                StringBuilder serialStr = new StringBuilder();
                serialStr.Append("and serialno in ('");
                foreach (string serial in this.serials)
                {
                    serialStr.Append(serial);
                    serialStr.Append("','");
                }
                serialStr.Remove(serialStr.Length - 2, 2);
                serialStr.Append(") ");
                return serialStr.ToString();
            }
        }
    }
}
