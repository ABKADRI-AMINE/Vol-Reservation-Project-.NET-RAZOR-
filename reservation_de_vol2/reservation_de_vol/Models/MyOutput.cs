using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace reservation_de_vol.Models

{

    public class MyOutput
    {
        public MyOutput()
        {
        }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("source")]
        public string source { get; set; }
        [JsonProperty("instantTicketingRequired")]
        public Boolean instantTicketingRequired { get; set; }
        [JsonProperty("nonHomogeneous")]
        public Boolean nonHomogeneous { get; set; }
        [JsonProperty("oneWay")]
        public Boolean oneWay { get; set; }
        [JsonProperty("lastTicketingDate")]
        public DateTime lastTicketingDate { get; set; }
        [JsonProperty("numberOfBookableSeats")]
        public int numberOfBookableSeats { get; set; }
        [JsonProperty("itineraries")]
        public List<itineraries> itineraries { get; set; }
        [JsonProperty("price")]
        public price price { get; set; }
    }
}
