using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MvcWebApp.Models
{
    public class UserPicture:TableEntity
    {
        public string RawPaths { get; set; }
        public string WatermarkRawPaths { get; set; }
        [IgnoreProperty]
        public List<string> Paths { 
            get=>RawPaths!=null? JsonConvert.DeserializeObject<List<string>>(RawPaths):null;
            set => RawPaths = JsonConvert.SerializeObject(value);
        }
        [IgnoreProperty]
        public List<string> WatermarkPaths
        {
            get => WatermarkRawPaths != null ? JsonConvert.DeserializeObject<List<string>>(WatermarkRawPaths):null;
            set => WatermarkRawPaths = JsonConvert.SerializeObject(value);
        }
    }
}
