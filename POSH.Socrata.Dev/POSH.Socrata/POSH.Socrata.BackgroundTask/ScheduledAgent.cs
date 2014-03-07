#define DEBUG_AGENT

using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POSH.Socrata.Entity.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;

namespace POSH.Socrata.BackgroundTask
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private ScheduledTask scheduleTask;
        private IsolatedStorageSettings setting = IsolatedStorageSettings.ApplicationSettings;
     
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            scheduleTask = task;
            if (NetworkInterface.GetIsNetworkAvailable())
            {

                System.Uri targetUri = new System.Uri(Constant.ProductionNotificationUrl);
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(targetUri);
                request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
            }
        }

        private void ReadWebRequestCallback(IAsyncResult callbackResult)
        {
            try
            {
                var id = "";
                int count = 0;

                SocrataNotification socrataUpdate = null;
                HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);
                string results = string.Empty;
                using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
                {
                    results = httpwebStreamReader.ReadToEnd();
                    socrataUpdate = JsonHelper.Deserialize<SocrataNotification>(results);
                }
                myResponse.Dispose();
                count = socrataUpdate.UpdatedList.Count;
                if (!setting.Contains("uniqueId"))
                {
                    setting.Add("uniqueId", socrataUpdate.uniqueId);
                    setting.Add("UpdatedItems", count);
                    setting.Add("TotalUpdatedItems", count);
                    setting.Save();
                }
                else
                {
                    id = setting["uniqueId"].ToString();
                }
                if (id != socrataUpdate.uniqueId)
                {
                    if (id != "")
                    {
                        int oldCount = Convert.ToInt32(setting["TotalUpdatedItems"]);
                        for (var counter = 0; counter < oldCount; counter++)
                        {
                            if (setting.Contains("DatasetName" + counter) )
                            {
                                setting.Remove("DatasetName" + counter);
                                setting.Remove("City" + counter);
                                setting.Save();
                            }
                        }
                      
                       
                    }

                    for (var counter = 0; counter < count; counter++)
                    {
                        setting.Add("DatasetName" + counter, socrataUpdate.UpdatedList[counter].DatasetName);
                        setting.Add("City" + counter, socrataUpdate.UpdatedList[counter].city);
                        setting.Save();
                    }
                    setting["UpdatedItems"] = count;
                    setting["TotalUpdatedItems"] = count;
                    setting["uniqueId"] = socrataUpdate.uniqueId;
                    setting.Save();
                }
                else
                {
                    count = Convert.ToInt32(setting["UpdatedItems"]);
                }

                if (count > 0)
                {
                    var isReceiveNotification = setting["notification"] as Nullable<bool>;
                    if (isReceiveNotification.Value)
                    {
                        UpdatePrimaryTile(count.ToString() + " " + Constant.NotificationMsg);
                        ShellToast toast = new ShellToast();
                        toast.Title = count.ToString();
                        toast.Content = Constant.NotificationMsg;
                        toast.NavigationUri = new Uri("/MainPage.xaml", UriKind.Relative);
                        toast.Show();
                    }
                }
                else
                {
                    UpdatePrimaryTile(string.Empty);
                }

#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(scheduleTask.Name, TimeSpan.FromSeconds(30));
                System.Diagnostics.Debug.WriteLine("Periodic task is started: " + scheduleTask.Name);
#endif

                NotifyComplete();
            }
            catch (Exception ex)
            {
                NotifyComplete();
            }
        }




        public void StorageToIsolated(string key, dynamic value)
        {
            try
            {
                if (!setting.Contains(key))
                {
                    setting.Add(key, value);
                    setting.Save();
                }
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Updates primary tile data
        /// </summary>
        private void UpdatePrimaryTile(string tileText)
        {
            FlipTileData TileData = new FlipTileData()
            {
                // BackTitle = content,

                Title = "Gov Finder",
                BackContent = tileText,
                WideBackContent = tileText,
                BackgroundImage = new Uri("Assets/336-336.png", UriKind.Relative),
                SmallBackgroundImage = new Uri("Assets/159-159.png", UriKind.Relative),
                WideBackgroundImage = new Uri("Assets/691-336.png", UriKind.Relative),
                BackBackgroundImage = new Uri("Assets/CommunityCenters.png", UriKind.Relative),
                WideBackBackgroundImage = new Uri("Assets/CommunityCenters.png", UriKind.Relative),
            };
            ShellTile primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(TileData);
        }
    }
}