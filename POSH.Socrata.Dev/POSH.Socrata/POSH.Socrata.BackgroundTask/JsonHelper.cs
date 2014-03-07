using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSH.Socrata.BackgroundTask
{
    public class JsonHelper
    {
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
