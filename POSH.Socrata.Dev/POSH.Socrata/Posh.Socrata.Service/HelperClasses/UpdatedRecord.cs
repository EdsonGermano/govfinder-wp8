using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Posh.Socrata.Service.HelperClasses
{
    public class UpdatedRecord
    {
        public string uniqueId { get; set; }
        public List<CityDataset> UpdatedList { get; set; }
    }
}