using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace LisBusiness
{
   public abstract class SuperLisBusiness
    {
       private string _checkCondition;
       public string CheckCondition
       {
           get { return this._checkCondition; }
           set { this._checkCondition = value; }
       }
       public virtual void Check()
       {
           CheckOnDB();
       }
       public abstract bool IsChecked();
       protected abstract void CheckOnDB();

       public abstract List<IResult> LisReport(string serialNo);
       public abstract List<IResult> LisReports(List<string> serialNos);
    }
}
