using System;
using System.Collections.Generic;
using System.Text;
namespace LisBusiness
{
    public class SectionFilter : DecoratorFilter
    {
        private HashSet<int> sections = new HashSet<int>();
        public void AddSection(int section) {
            this.sections.Add(section);
        }
        public void RemoveSection(int section) {
            this.sections.Remove(section);
        }
        public void Clear() {
            this.sections.Clear();
        }
        public override string GetFilter()
        {
            string temp=base.GetFilter();
            string result = temp + sectionWhere();
            return result;
        }
        private string sectionWhere()
        {
            if (this.sections.Count == 0)
            {
                return "";
            }
            else {
                StringBuilder sectionStr = new StringBuilder();
                sectionStr.Append("and sectionno in (");
                foreach (int section in this.sections) {
                    sectionStr.Append(section);
                    sectionStr.Append(',');
                }
                sectionStr.Remove(sectionStr.Length - 1, 1);
                sectionStr.Append(") ");
                return sectionStr.ToString();
            }
        }
    }
}
