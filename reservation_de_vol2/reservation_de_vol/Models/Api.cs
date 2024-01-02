using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace reservation_de_vol.Models
{
    public class meta
    {
        public meta() { }
    }
    public class Api
    {
        
        public Api() { }
        [JsonProperty("meta")]
        public meta meta { get; set; }
        [JsonProperty("data")]
        public List<MyOutput> data { get; set; }
    }
}