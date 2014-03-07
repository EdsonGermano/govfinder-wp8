namespace POSH.Socrata.Entity.Models
{
    public class Constant
    {
        /// <summary>
        /// For testing purpose
        /// </summary>
        //public const string xmlUrl ="https://govfinder.socrata.com/api/views/vef6-vqfd/rows.xml?accessType=DOWNLOAD";
        /// <summary>
        /// For Production
        /// </summary>
        
       // public const string xmlUrl = "https://govfinder.socrata.com/api/views/hwum-ssyq/rows.xml?accessType=DOWNLOAD";
         
        public const string xmlUrl = "https://govfinder.socrata.com/api/views/rqrg-e5jx/rows.xml?accessType=DOWNLOAD";
        public const string ProductionNotificationUrl = "http://poshsocratacommentservice.cloudapp.net/api/comment/GetNotifications?key=POSHSocrataComments";
        //public const string TestingNotificationUrl = "http://socratacommentcloudservice.cloudapp.net/api/comment/GetNotifications?key=POSHSocrataComments";
        public const string AzureServiceURL = "http://poshsocratacommentservice.cloudapp.net/api/comment/addcomment?key={0}";
        public const string CommentsBaseAddress = "http://poshsocratacommentservice.cloudapp.net/";
        public const string GetCommentsUrl = "api/comment/GetComments?key={0}&Id={1}&cityName={2}&reportName={3}";
        public const string GetCommentsCountUrl = "api/comment/GetComments?key={0}&Id={1}&cityName={2}&reportName={3}&IsCommentCount=true";
        public const string CommentsAzureServiceKey = "POSHSocrataComments";
        public const string SocrataLoginHttpBase = "https://opendata.socrata.com";
        public const string SocrataLoginAppToken = "ZtjV2EeHKtfsMGW1Kcns82goW";
        public const string SocrataUserDetailUrl = "/users/current.json";
        public const int MaxTextBlockLength = 2048;
        public const string LiveSdkClientIdWP = "0000000044102812";
        public const string NotificationMsg = "item updated";
        public const string Seprator = "@@@@@@";
    }
}