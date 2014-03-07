using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Posh.Socrata.WorkerRole.HelperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posh.Socrata.WorkerRole.BusinessLogic
{
    public class CityStorage
    {
        #region Methods

        /// <summary>
        /// Used to save records in city table
        /// </summary>
        public void SaveCityAllRecord()
        {
            try
            {
                CityData cityData = new CityData();
                List<CityRecord> cityRecordList = cityData.LoadPortalContent();
                List<CityRecord> existCityRecordList = GetEntity();
                if (existCityRecordList.Count < 1)
                {
                    AddItems(cityRecordList);
                }
                else
                {
                    UpdateItems(cityRecordList, existCityRecordList);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to get all city entites
        /// </summary>
        /// <returns></returns>
        public List<CityRecord> GetEntity()
        {
            List<CityRecord> cityRecordsList = new List<CityRecord>();
            try
            {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("cityrecords");
                // Get the data service context
                TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                //// Specify a partition query
                cityRecordsList =
                           (from e in serviceContext.CreateQuery<CityRecord>("cityrecords")
                            select e).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return cityRecordsList;
        }

        /// <summary>
        /// Used to delete all records
        /// </summary>
        public void DeleteRecords()
        {
            try
            {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                //get table references
                CloudTable updateTable = tableClient.GetTableReference("updatedcityrecord");
                CloudTable globalTable = tableClient.GetTableReference("globalvariables");

                // Get the data service context
                TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                //// Specify a partition query
                List<UpdatedCityRecord> cityRecordsList =
                           (from e in serviceContext.CreateQuery<UpdatedCityRecord>("updatedcityrecord")
                            select e).ToList();

                foreach (UpdatedCityRecord city in cityRecordsList)
                {
                    city.ETag = "*";
                    TableOperation deleteOperation = TableOperation.Delete(city);
                    updateTable.Execute(deleteOperation);
                }

                //// Specify a partition query
                List<GlobalVariables> global =
                           (from e in serviceContext.CreateQuery<GlobalVariables>("globalvariables")
                            select e).ToList();
                foreach (GlobalVariables city in global)
                {
                    city.ETag = "*";
                    TableOperation deleteOperation = TableOperation.Delete(city);
                    globalTable.Execute(deleteOperation);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to Delete RecordByID
        /// </summary>
        /// <param name="rowIdList"></param>
        public void DeleteRecordByID(List<int> rowIdList)
        {
            try
            {
                if (rowIdList != null)
                {
                    // Retrieve storage account from connection string
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                    // Create the table client
                    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                    //get table references
                    CloudTable cityTable = tableClient.GetTableReference("cityrecords");
                    CloudTable updateTable = tableClient.GetTableReference("updatedcityrecord");

                    // Get the data service context
                    TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                    //// Specify a partition query
                    foreach (int id in rowIdList)
                    {
                        CityRecord city =
                                   (from e in serviceContext.CreateQuery<CityRecord>("cityrecords")
                                    where e.RowKey == id.ToString()
                                    select e).FirstOrDefault();
                        if (city != null)
                        {
                            city.ETag = "*";
                            TableOperation deleteOperation = TableOperation.Delete(city);
                            cityTable.Execute(deleteOperation);
                        }
                    }

                    //// Specify a partition query
                    //// Specify a partition query
                    foreach (int id in rowIdList)
                    {
                        UpdatedCityRecord city =
                                   (from e in serviceContext.CreateQuery<UpdatedCityRecord>("updatedcityrecord")
                                    where e.RowKey == id.ToString()
                                    select e).FirstOrDefault();

                        if (city != null)
                        {
                            city.ETag = "*";
                            TableOperation deleteOperation = TableOperation.Delete(city);
                            updateTable.Execute(deleteOperation);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to Add Items first time
        /// </summary>
        /// <param name="cityRecordList"></param>
        public void AddItems(List<CityRecord> cityRecordList)
        {
            try
            {
                GlobalVariables global = GetGlobalEntity();
                if (cityRecordList.Count < 1 && global != null)
                {
                    /////Do nothing
                }
                else
                    if (cityRecordList.Count > 0 && global != null)
                    {
                        List<CityRecord> existCityRecordList = GetEntity();
                        UpdateItems(cityRecordList, existCityRecordList);
                    }
                    else
                    {
                        // Retrieve the storage account from the connection string.
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                        // Create the table client.
                        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                        CloudTable cityTable = tableClient.GetTableReference("cityrecords");
                        cityTable.CreateIfNotExists();

                        foreach (CityRecord city in cityRecordList)
                        {
                            TableOperation insertOperation = TableOperation.Insert(city);
                            cityTable.Execute(insertOperation);
                        }
                        AddUniqueGuid();
                    }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to Update Items
        /// </summary>
        /// <param name="cityRecordList"></param>
        /// <param name="existCityRecordList"></param>
        public void UpdateItems(List<CityRecord> cityRecordList, List<CityRecord> existCityRecordList)
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable cityTable = tableClient.GetTableReference("cityrecords");
                cityTable.CreateIfNotExists();

                CloudTable updateTable = tableClient.GetTableReference("updatedcityrecord");
                updateTable.CreateIfNotExists();

                ///Deleting the record from table who's id does not exist
                var existRowIdList = existCityRecordList.Select(obj => obj.RowId).ToList();
                var newRowIDList = cityRecordList.Select(obj => obj.RowId).ToList();

                List<UpdatedCityRecord> updateRecordList = GetUpdateCity();
                var updateRowIDList = updateRecordList.Select(obj => obj.RowId).ToList();

                int count = 0;
                foreach (int id in newRowIDList)
                {
                    if (!existRowIdList.Exists(value => value == id))
                    {
                        if (count < 1)
                        {
                            DeleteRecords();
                        }
                        count++;
                        var city = (from specificCity in cityRecordList where specificCity.RowId == id select specificCity).FirstOrDefault();
                        TableOperation insertOperation = TableOperation.Insert(city);
                        cityTable.Execute(insertOperation);
                        updateTable.Execute(insertOperation);
                    }
                }

                bool isCitySame = CheckUpdate(newRowIDList);
                List<int> list = new List<int>();
                foreach (int id in existRowIdList)
                {
                    if (!newRowIDList.Exists(value => value == id))
                    {
                        list.Add(id);
                    }
                }
                if (list.Count > 0)
                {
                    DeleteRecordByID(list);
                    if (isCitySame == false)
                    {
                        DeleteGlobalVariables();
                    }
                }
                /////create unique id if new row updated
                if (count > 0 || isCitySame == false)
                {
                    AddUniqueGuid();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to add UniqueId
        /// </summary>
        public void AddUniqueGuid()
        {
            try
            {
                // Retrieve the storage account from the connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client.
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                CloudTable globalTable = tableClient.GetTableReference("globalvariables");
                globalTable.CreateIfNotExists();

                ////insert the unique id in table
                GlobalVariables global = new GlobalVariables(Guid.NewGuid().ToString(), true);
                global.UniqueId = Guid.NewGuid().ToString();
                global.isTrue = true;
                TableOperation insetOperationGlobal = TableOperation.Insert(global);
                globalTable.Execute(insetOperationGlobal);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        /// <summary>
        /// Used to get all city entites from updated table
        /// </summary>
        /// <returns></returns>
        public List<UpdatedCityRecord> GetUpdateCity()
        {
            List<UpdatedCityRecord> cityRecordsList = new List<UpdatedCityRecord>();
            try
            {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("updatedcityrecord");
                // Get the data service context
                TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                //// Specify a partition query
                cityRecordsList =
                           (from e in serviceContext.CreateQuery<UpdatedCityRecord>("updatedcityrecord")
                            select e).ToList();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return cityRecordsList;
        }

        /// <summary>
        /// Used to check entites to be updated
        /// </summary>
        /// <param name="newRowIDList"></param>
        /// <returns></returns>
        public bool CheckUpdate(List<int> newRowIDList)
        {
            List<UpdatedCityRecord> updateRecordList = GetUpdateCity();
            var updateRowIDList = updateRecordList.Select(obj => obj.RowId).ToList();
            int idCount = 0;
            bool isCitySame = false;

            foreach (int id in newRowIDList)
            {
                if (updateRowIDList.Exists(value => value == id))
                {
                    idCount++;
                }
            }
            if (idCount == updateRowIDList.Count)
            {
                isCitySame = true;
            }
            return isCitySame;
        }

        /// <summary>
        /// Used to get global entites
        /// </summary>
        /// <returns></returns>
        public GlobalVariables GetGlobalEntity()
        {
            GlobalVariables global = new GlobalVariables();
            try
            {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("globalvariables");
                // Get the data service context
                TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                //// Specify a partition query
                global = (from e in serviceContext.CreateQuery<GlobalVariables>("globalvariables")
                          select e).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
            }
            return global;
        }

        /// <summary>
        /// Used to delete global variables from table
        /// </summary>
        public void DeleteGlobalVariables()
        {
            try
            {
                // Retrieve storage account from connection string
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(Constatns.StorageAccountConnectionString));

                // Create the table client
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                //get table references
                CloudTable globalTable = tableClient.GetTableReference("globalvariables");

                // Get the data service context
                TableServiceContext serviceContext = tableClient.GetTableServiceContext();

                //// Specify a partition query
                List<GlobalVariables> global =
                           (from e in serviceContext.CreateQuery<GlobalVariables>("globalvariables")
                            select e).ToList();
                foreach (GlobalVariables city in global)
                {
                    city.ETag = "*";
                    TableOperation deleteOperation = TableOperation.Delete(city);
                    globalTable.Execute(deleteOperation);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        #endregion
    }
}
