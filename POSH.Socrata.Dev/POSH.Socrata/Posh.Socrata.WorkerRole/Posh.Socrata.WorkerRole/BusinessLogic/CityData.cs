using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Posh.Socrata.WorkerRole.HelperClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Posh.Socrata.WorkerRole.BusinessLogic
{
    public class CityData
    {
        /// <summary>
        /// Global Variables
        /// </summary>
        string city = string.Empty;
        string Dataset = string.Empty;
        string Api_Url = string.Empty;
        string RowId = string.Empty;
        string Uuid = string.Empty;

        #region Methodss

        /// <summary>
        /// Method for load xml data
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<CityRecord> LoadPortalContent()
        {
            List<CityRecord> cityRecordList = new List<CityRecord>();
            try
            {
                XmlDocument _Doc = new XmlDocument();
                _Doc.Load(Constatns.XmlUrl);

                foreach (XmlNode node in _Doc.DocumentElement.ChildNodes[0].ChildNodes)
                {
                    string isvalid = node["isvalid"].InnerText;
                    if (isvalid.Equals("1"))
                    {
                        if (node.SelectSingleNode("api_url") != null)
                        {
                            Api_Url = Convert.ToString(node["api_url"].InnerText);
                        }
                        else
                        {
                            Api_Url = string.Empty;
                        }
                        if (node.SelectSingleNode("city") != null)
                        {
                            city = Convert.ToString(node["city"].InnerText);
                        }
                        else
                        {
                            city = string.Empty;
                        }
                        if (node.SelectSingleNode("dataset_name") != null)
                        {
                            Dataset = Convert.ToString(node["dataset_name"].InnerText);
                        }
                        else
                        {
                            Dataset = string.Empty;
                        }

                        CityRecord cityRecord = new CityRecord(Convert.ToString(node["city"].InnerText), Convert.ToInt32(node.Attributes["_id"].InnerText))
                        {
                            CityName = city,
                            DatasetName = Dataset,
                            APIURL =Api_Url,
                            RowId = Convert.ToInt32(node.Attributes["_id"].InnerText),
                            UUID = node.Attributes["_uuid"].InnerText,
                            AddDate = DateTime.Now,
                            ExpiryDate = DateTime.Now.AddDays(1),
                        };
                        cityRecordList.Add(cityRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return cityRecordList;
        }

        public int GetTotalDataSet(string Uri)
        {
            if (!string.IsNullOrEmpty(Uri))
            {
                string jsonString = new System.Net.WebClient().DownloadString(Uri);
                JArray jsonArray = JArray.Parse(jsonString);
                int count = jsonArray.Count;
                return count;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
