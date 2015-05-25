using System;
using System.Collections.Generic;
namespace LisBusiness
{
    public interface ILisReportPDF
    {
        List<IResult> LisReport(string serialNo);
        List<IResult> LisReports(List<string> serialNos);
    }
}
