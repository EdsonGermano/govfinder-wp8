using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace POSH.Socrata.ViewModel.HelperClasses
{
    public static class JSON_Helper
    {
        /// <summary>
        /// Serializes data to specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                var msArray = ms.ToArray();
                string retVal = Encoding.UTF8.GetString(msArray, 0, msArray.Length);
                return retVal;
            }
        }

        /// <summary>
        /// Deserialize data to specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                obj = (T)JsonConvert.DeserializeObject(json, obj.GetType());
                return obj;
            }
        }
    }
}