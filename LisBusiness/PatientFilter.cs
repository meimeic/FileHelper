using System.Collections.Generic;
using System.Text;

namespace LisBusiness
{
    public class PatientFilter:DecoratorFilter
    {
        private List<string> patients = new List<string>();
        public void AddPatient(string patient)
        {
            this.patients.Add(patient);
        }
        public void RemovePatient(string patient)
        {
            this.patients.Remove(patient);
        }
        public void Clear()
        {
            this.patients.Clear();
        }
        public override string GetFilter()
        {
            string temp = base.GetFilter();
            string result = temp + patientWhere();
            return result;
        }
        private string patientWhere() {
            if (this.patients.Count == 0)
            {
                return "";
            }
            else if (this.patients.Count == 1)
            {
                return "and patno='" + this.patients[0] + "' ";
            }
            else
            {
                StringBuilder patientStr = new StringBuilder();
                patientStr.Append("and patno in ('");
                foreach (string patient in this.patients)
                {
                    patientStr.Append(patient);
                    patientStr.Append("','");
                }
                patientStr.Remove(patientStr.Length - 2, 2);
                patientStr.Append(") ");
                return patientStr.ToString();
            }
        } 
    }
}
