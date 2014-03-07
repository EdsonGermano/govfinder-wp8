using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posh.Socrata.WorkerRole.HelperClasses
{
    public class GlobalVariables : TableEntity
    {
        public GlobalVariables(string UniqueId, bool isTrue)
        {
            this.PartitionKey = UniqueId;
            this.RowKey = isTrue.ToString();
        }
          public GlobalVariables() { }
        public bool isTrue { get; set; }
        public string UniqueId { get; set; }
    }
}
