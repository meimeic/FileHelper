using System;
using System.Collections.Generic;
using System.Text;

namespace LisBusiness
{
    public class VisitFilter : DecoratorFilter
    {
        private int _visitTimes;
        //门诊类型 0门诊  1住院  2其他
        private int _clinicType;
        public void SetTimes(int time)
        {
            this._clinicType = 1;
            this._visitTimes = time;
        }
        public int GetTimes()
        {
            return this._visitTimes;
        }
        public void SetClinicType(int clinic) {
            this._clinicType = clinic;
        }
        public int GetClinicType()
        {
            return this._clinicType;
        }
        public override string GetFilter()
        {
            string temp = base.GetFilter();
            string result = temp + ClinicTypeWhere();
            return result;
        }
        private string ClinicTypeWhere() {
            string clinicWhere="";
            switch (this._clinicType)
            {
                    //门诊
                case 0:
                    clinicWhere = "and sicktypeno in (2,3) ";
                    break;
                case 1:
                    if (this._visitTimes > 0)
                    {
                        clinicWhere = "and sicktypeno in (1,4) and hospitalizedtimes=" + this._visitTimes + " ";
                    }
                    else {
                        clinicWhere = "and sicktypeno in (1,4) ";
                    }
                    break;
                case 2:
                    break;
            }
            return clinicWhere;
        }
    }
}
