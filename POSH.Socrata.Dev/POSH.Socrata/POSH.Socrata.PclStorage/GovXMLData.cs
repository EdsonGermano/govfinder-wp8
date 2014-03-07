using PCLStorage;
using POSH.Socrata.Entity.Models;
using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace POSH.Socrata.PclStorage
{
    public class GovXMLData
    {
        public string responseText = null;
        public bool hasError = false;
        public string errorMessage = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public GovXMLData()
        {
        }

        /// <summary>
        /// Gets xmlResponse from existed file or from url.
        /// </summary>
        /// <returns></returns>
        async public Task<bool> GetData()
        {
            try
            {
                var isExist = await IsFileExist();
                if (!isExist)
                {
                    hasError = await DownloadFileAndSave();
                }
                else
                {
                    IFolder localFolder = FileSystem.Current.LocalStorage;
                    IFile file = await localFolder.GetFileAsync("GovFinderData.xml");
                    responseText = await file.ReadAllTextAsync();
                    hasError = false;
                }
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                UpdateFileInLocal();
            }
            return false;
        }

        /// <summary>
        /// Checks if file exists or not in isolated storage
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFileExist()
        {
            try
            {
                IFolder localFolder = FileSystem.Current.LocalStorage;
                IFile file = await localFolder.GetFileAsync("GovFinderData.xml");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Downloads XML data from url.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DownloadFileAndSave()
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(Constant.xmlUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        responseText = await response.Content.ReadAsStringAsync(); 

                        IFolder localFolder = FileSystem.Current.LocalStorage;
                        IFile file = await localFolder.CreateFileAsync("GovFinderData.xml", CreationCollisionOption.ReplaceExisting);

                        await file.WriteAllTextAsync(responseText);
                    }
                    return false;
                }
                else
                {
                    errorMessage = "Internet connection required. Please check your connection and try again.";
                    return true;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Downloads XML data from url and replaces stored file from new
        /// </summary>
        /// <returns></returns>
        private async Task UpdateFileInLocal()
        {
            try
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(Constant.xmlUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();
                        responseText = await response.Content.ReadAsStringAsync();

                        IFolder localFolder = FileSystem.Current.LocalStorage;
                        IFile file = await localFolder.CreateFileAsync("GovFinderData.xml", CreationCollisionOption.ReplaceExisting);

                        await file.WriteAllTextAsync(responseText);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}