using System.Collections.Generic;
namespace reservation_de_vol.Models
{

    public class itineraries
    {
        public string duration { get; set; }
        public List<segments> segments { get; set; }
    }
}
