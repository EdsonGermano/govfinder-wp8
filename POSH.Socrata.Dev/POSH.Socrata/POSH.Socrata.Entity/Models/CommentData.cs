using System;

namespace POSH.Socrata.Entity.Models
{
    public class CommentData
    {
        public string Id { get; set; }

        private string _cityName;

        public string CityName
        {
            get { return _cityName; }
            set { _cityName = value; }
        }

        private string _reportName;

        public string ReportName
        {
            get { return _reportName; }
            set { _reportName = value; }
        }

        public string Author { get; set; }

        public string CommentMessage { get; set; }

        public string CommentTitle { get; set; }

        public DateTime CommentPublishAt { get; set; }
    }
}