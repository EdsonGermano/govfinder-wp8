using Posh.Socrata.Service.Entity;
using Posh.Socrata.Service.HelperClasses;
using System.Collections.Generic;

namespace Posh.Socrata.Service.DAL
{
    /// <summary>
    /// Interface for IRepository
    /// </summary>
    internal interface IRepository
    {
        List<CommentData> GetCommentsByCityAndReport(string Id, string CityName, string ReportName);

        int GetCommentsCountByCityAndReportName(string Id, string CityName, string ReportName);

        bool AddComments(CommentData commentValue);

        UpdatedRecord GetNotification();
    }
}