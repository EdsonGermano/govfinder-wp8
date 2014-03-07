using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POSH.Socrata.Entity.Models
{
    public class SocrataUserDetail
    {
        public string id { get; set; }
        public int createdAt { get; set; }
        public int createdOnDomainId { get; set; }
        public string displayName { get; set; }
        public string email { get; set; }
        public bool emailUnsubscribed { get; set; }
        public int lastLogin { get; set; }
        public int numberOfFollowers { get; set; }
        public int numberOfFriends { get; set; }
        public int oid { get; set; }
        public string privacyControl { get; set; }
        public int profileLastModified { get; set; }
        public int publicTables { get; set; }
        public int publicViews { get; set; }
        public string screenName { get; set; }
    }
}
