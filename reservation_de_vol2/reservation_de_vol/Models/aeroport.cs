using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
namespace reservation_de_vol.Models
{
    public class Airport
    {
        public Airport()
        {
        }
        public string code { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }

    }
}
