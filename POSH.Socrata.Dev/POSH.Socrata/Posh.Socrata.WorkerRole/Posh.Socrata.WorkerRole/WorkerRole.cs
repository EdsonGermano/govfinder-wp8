using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Posh.Socrata.WorkerRole.HelperClasses;
using Posh.Socrata.WorkerRole.BusinessLogic;

namespace Posh.Socrata.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        #region Methods
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            while (true)
            {
                CityStorage cityStorage = new CityStorage();
                cityStorage.SaveCityAllRecord();
                Thread.Sleep(1000);
            }
        }

        public override bool OnStart()
        {
            //// Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            //IsStopped = true;
            //Client.Close();
            base.OnStop();
        }
        #endregion
    }
}
