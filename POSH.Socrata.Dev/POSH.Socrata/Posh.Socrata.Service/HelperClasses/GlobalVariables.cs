using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Posh.Socrata.Service.HelperClasses
{
    public class GlobalVariables : TableEntity
    {
        public bool isTrue { get; set; }
        public string UniqueId { get; set; }
    }
}