using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Posh.Socrata.Service.Entity
{
    public class CommentData : TableEntity
    {
        public CommentData()
        {
            this.PartitionKey = Convert.ToString((new Random()).Next());
            this.RowKey = Convert.ToString((new Random()).Next());
        }

        private string _id;

        public string Id { get { return _id; } set { if (!string.IsNullOrWhiteSpace(value)) { _id = value.ToLower(); } else { _id = value; } } }

        private string _cityName;

        public string CityName { get { return _cityName; } set { _cityName = value; } }

        private string _reportName;

        public string ReportName { get { return _reportName; } set { _reportName = value; } }

        public string Author { get; set; }

        public string CommentMessage { get; set; }

        public string CommentTitle { get; set; }

        public DateTime CommentPublishAt { get; set; }
    }
}