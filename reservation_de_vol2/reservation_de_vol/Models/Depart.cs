﻿using System;
namespace reservation_de_vol.Models
{
    public class departure
    {
        public departure() { }
        public string iataCode { get; set; }
        public string terminal { get; set; }
        public DateTime at { get; set; }
    }
}
