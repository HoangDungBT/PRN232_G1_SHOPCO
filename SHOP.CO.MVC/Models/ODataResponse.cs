using Newtonsoft.Json;
using System.Collections.Generic;

namespace SHOP.CO.MVC.Models
{
    public class ODataResponse<T>
    {
        [JsonProperty("@odata.count")]
        public int Count { get; set; }

        [JsonProperty("value")]
        public List<T> Value { get; set; } = new();
    }
}
