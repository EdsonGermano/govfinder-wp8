using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Posh.Socrata.Service.Entity;
using Posh.Socrata.Service.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Posh.Socrata.Service.DAL
{
    /// <summary>
    /// Class to implement IRepository interface
    /// </summary>
    public class SqlRepository : IRepository
    {
        /// <summary>
        /// Gets the comments by city and report.
        /// </summary>
        /// <param name="CityName">Name of the city.</param>
        /// <param name="ReportName">Name of the report.</param>
        /// <returns></returns>
        public List<CommentData> GetCommentsByCityAndReport(string Id, string CityName, string ReportName)
        {
            List<CommentData> comment = new List<CommentData>();

            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=socratacomments;AccountKey=CPUzxusfkGMWv+3cStItzMOjJna7Qyugz/dQDsSzxCg/1oFMaxcWtp7qsC5wb2guaDaSjqubNZ5Qw8f18lV6MQ==");

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                CloudTable table = tableClient.GetTableReference("Comments");
                //table.CreateIfNotExists();

                // Create the table query.
                TableQuery<CommentData> rangeQuery = new TableQuery<CommentData>().Where(
                     TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, (Id.ToLower())),
                        TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("CityName", QueryComparisons.Equal, CityName),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("ReportName", QueryComparisons.Equal, ReportName))));

                IEnumerable<CommentData> commentDataLst = table.ExecuteQuery(rangeQuery);

                if (commentDataLst != null)
                {
                    foreach (var item in commentDataLst)
                    {
                        comment.Add(item);
                    }
                }
            }
            catch (Exception)
            {
                comment = new List<CommentData>();
            }

            return comment;
        }

        /// <summary>
        /// Adds the comments.
        /// </summary>
        /// <param name="commentValue">The comment value.</param>
        /// <returns></returns>
        public bool AddComments(CommentData commentValue)
        {
            bool IsCommentAdded = false;

            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=socratacomments;AccountKey=CPUzxusfkGMWv+3cStItzMOjJna7Qyugz/dQDsSzxCg/1oFMaxcWtp7qsC5wb2guaDaSjqubNZ5Qw8f18lV6MQ==");//ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                CloudTable table = tableClient.GetTableReference("Comments");
                table.CreateIfNotExists();

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(commentValue);

                // Execute the insert operation.
                table.Execute(insertOperation);

                IsCommentAdded = true;
            }
            catch (Exception)
            {
                IsCommentAdded = false;
            }

            return IsCommentAdded;
        }

        /// <summary>
        /// Gets the name of the comments count by city and report.
        /// </summary>
        /// <param name="CityName">Name of the city.</param>
        /// <param name="ReportName">Name of the report.</param>
        /// <returns></returns>
        public int GetCommentsCountByCityAndReportName(string Id, string CityName, string ReportName)
        {
            int commentCount = 0;

            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=socratacomments;AccountKey=CPUzxusfkGMWv+3cStItzMOjJna7Qyugz/dQDsSzxCg/1oFMaxcWtp7qsC5wb2guaDaSjqubNZ5Qw8f18lV6MQ==");

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                // Create the table if it doesn't exist.
                CloudTable table = tableClient.GetTableReference("Comments");
                table.CreateIfNotExists();

                // Create the table query.
                TableQuery<CommentData> rangeQuery = new TableQuery<CommentData>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, (Id.ToLower())),
                        TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("CityName", QueryComparisons.Equal, CityName),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("ReportName", QueryComparisons.Equal, ReportName))));

                IEnumerable<CommentData> commentDataLst = table.ExecuteQuery(rangeQuery);

                commentCount = commentDataLst != null ? commentDataLst.Count() : 0;
            }
            catch (Exception)
            {
                commentCount = 0;
            }

            return commentCount;
        }

        /// <summary>
        /// Gets the Notification for a change in datasets
        /// </summary>
        /// <returns></returns>
        public UpdatedRecord GetNotification()
        {
            // Retrieve storage account from connection string
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=socratacomments;AccountKey=CPUzxusfkGMWv+3cStItzMOjJna7Qyugz/dQDsSzxCg/1oFMaxcWtp7qsC5wb2guaDaSjqubNZ5Qw8f18lV6MQ==");

            // Create the table client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("updatedcityrecord");
            // Get the data service context
            TableServiceContext serviceContext = tableClient.GetTableServiceContext();

            //// Specify a partition query
            var cityRecordsList = (from e in serviceContext.CreateQuery<UpdatedCityRecord>("updatedcityrecord") select e).ToList();

            UpdatedRecord updateCityList = new UpdatedRecord();
            updateCityList.UpdatedList = new List<CityDataset>();

            GlobalVariables global =
                     (from e in serviceContext.CreateQuery<GlobalVariables>("globalvariables")
                      select e).FirstOrDefault();
            if (global != null)
            {
                updateCityList.uniqueId = global.UniqueId;
            }
            foreach (UpdatedCityRecord list in cityRecordsList)
            {
                updateCityList.UpdatedList.Add(new CityDataset() { city = list.CityName, DatasetName = list.DatasetName });
            }

            if (cityRecordsList.Count < 1)
            {
                updateCityList.UpdatedList.Add(new CityDataset() { city = "", DatasetName = "" });
            }

            ////Reset the count
            return updateCityList;
        }
    }
}