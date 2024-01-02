using System;
namespace reservation_de_vol.Models
{
    public class Confidentialite
    {
        public Confidentialite()
        {
        }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }
}
