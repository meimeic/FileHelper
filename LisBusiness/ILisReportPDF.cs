using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace LisBusiness
{
    public interface ILisReportPDF
    {
        List<IResult> LisReport(string serialNo);
        List<IResult> LisReports(List<string> serialNos);
    }
}
