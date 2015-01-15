using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace LisDocumentCheck
{
   public abstract class SuperBusiness
    {
       private string _checkCondition;
       public string CheckCondition
       {
           get { return this._checkCondition; }
           set { this._checkCondition = value; }
       }
       public virtual void Check()
       {
           if (IsChecked())
           {
               //已检测
           }
           else
           {
               CheckOnDB();
           }
       }
       public abstract bool IsChecked();
       protected abstract void CheckOnDB();
    }
}
