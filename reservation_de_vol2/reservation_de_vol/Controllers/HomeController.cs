using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using reservation_de_vol.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;

namespace reservation_de_vol.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Privacy()
        {
            return View();
        }
		[HttpPost]
		public JsonResult Index(string Prefix)
        {
			StreamReader r = new StreamReader("wwwroot/js/airports.json");
			string jsonString = r.ReadToEnd();
			List<Vol> airports = JsonConvert.DeserializeObject<List<Vol>>(jsonString);
			var cities = (from N in airports
						  where N.city.StartsWith(Prefix)
						  select new { N.city });
			return Json(cities);
		}

			public static string getAirportFullName(string code)
        {

            StreamReader r = new StreamReader("wwwroot/js/airports.json");
            string jsonString = r.ReadToEnd();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);

            foreach (var i in airports)
            {
                if (i.code == code)
                {
                    return i.name;
                }
            }
            return "";
        }
        public static string getAirportcode(string city)
        {

            StreamReader r = new StreamReader("wwwroot/js/airports.json");
            string jsonString = r.ReadToEnd();
            List<Airport> airports = JsonConvert.DeserializeObject<List<Airport>>(jsonString);

            foreach (var i in airports)
            {
                if (i.city == city)
                {
                    return i.code;
                }
            }
            return "";
        }
        public IActionResult Result(Vol vol)
        {
            

            Vol newVol = new Vol();
            newVol.departure = vol.departure;
            newVol.arrival = vol.arrival;
            //v.flight_date_d = Convert.ToDateTime("25/12/2023 00:00:00");
            //v.flight_date_a = Convert.ToDateTime("28/12/2023 00:00:00");
            newVol.flight_date_d = vol.flight_date_d;
            newVol.flight_date_a = vol.flight_date_a;
            System.Diagnostics.Debug.WriteLine("Departure Date: " + newVol.flight_date_d);
            System.Diagnostics.Debug.WriteLine("Return Date: " + newVol.flight_date_a);

            newVol.adults = vol.adults;
            newVol.children = vol.children;
            newVol.infants = vol.infants;

            System.Diagnostics.Debug.WriteLine("------------dep   " + vol.departure);
            System.Diagnostics.Debug.WriteLine("------------arv   " + vol.arrival);
            System.Diagnostics.Debug.WriteLine("------------deb   " + vol.flight_date_d);
            System.Diagnostics.Debug.WriteLine("------------ret   " + vol.flight_date_a);
            System.Diagnostics.Debug.WriteLine("------------adu   " + vol.adults);
            System.Diagnostics.Debug.WriteLine("------------child   " + vol.children);
            System.Diagnostics.Debug.WriteLine("------------enf   " + vol.infants);


            newVol.flight_type = vol.flight_type;
            newVol.travel_class = vol.travel_class;
            newVol.non_stop = vol.non_stop;
            System.Diagnostics.Debug.WriteLine("------------class   " + vol.travel_class);

            System.Diagnostics.Debug.WriteLine("------------non_stop   " + vol.non_stop);


            if (newVol.flight_type == "One way")
            {
                System.Diagnostics.Debug.WriteLine("Departure Date: " + newVol.flight_date_d);
                System.Diagnostics.Debug.WriteLine("Return Date: " + newVol.flight_date_a);

               
                    var date_dep = newVol.flight_date_d.ToString("yyyy-MM-dd");
                    var resp = getFlightsOneWay(newVol.departure, newVol.arrival, date_dep, newVol.adults, newVol.children, newVol.infants, newVol.travel_class, newVol.non_stop);
                if (resp is string)
                {
                    ModelState.AddModelError("", "Please fill in all required fields.");
					return View("index");

				}
				else
                {
                    ViewBag.Message = resp;
                    var mydataa = ViewBag.Message.data;

                    foreach (var obj in mydataa)
                    {
                        foreach (var it in obj.itineraries)
                        {
                            foreach (var seg in it.segments)
                            {
                                var mycodeAller = getAirportFullName(seg.departure.iataCode.ToString());
                                seg.departure.iataCode = mycodeAller;
                                var mycodeArrivER = getAirportFullName(seg.arrival.iataCode.ToString());
                                seg.arrival.iataCode = mycodeArrivER;

                            }

                        }
                    }
                    ViewBag.Vol = newVol;
                    return View("Aller");

                }


            }
            else
            {
                
                var date_dep = newVol.flight_date_d.ToString("yyyy-MM-dd");
                var date_arr = newVol.flight_date_a.ToString("yyyy-MM-dd");
                System.Diagnostics.Debug.WriteLine(date_arr);
                System.Diagnostics.Debug.WriteLine(date_dep);


                var resp = getFlightsRoundtrip(newVol.departure, newVol.arrival, date_dep, date_arr, newVol.adults, newVol.children, newVol.infants, newVol.travel_class, newVol.non_stop);
                System.Diagnostics.Debug.WriteLine("Resultat de reponse" + resp);
				if (resp is string)
				{
                    ModelState.AddModelError("", "Please fill in all required fields.");
                    return View("index");
                }
                else
                {
					ViewBag.Message = resp;

					var mydataa = ViewBag.Message.data;
					foreach (var obj in mydataa)
					{
						foreach (var it in obj.itineraries)
						{
							foreach (var seg in it.segments)
							{
								var mycodeAller = getAirportFullName(seg.departure.iataCode.ToString());
								seg.departure.iataCode = mycodeAller;
								var mycodeArrivER = getAirportFullName(seg.arrival.iataCode.ToString());
								seg.arrival.iataCode = mycodeArrivER;

							}

						}
					}
					ViewBag.Vol = newVol;
					return View("AllerRetour");
				}

					
            }

        }


        public Object getFlightsOneWay(string depart, string arriver, string d, int adults, int children, int infants, string travel_class, string non_stop)
        {
            var codeAiroDepart = getAirportcode(depart);
            var codeAiroArrive = getAirportcode(arriver);
            string accessToken = getAccessToken();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.BaseAddress = new Uri("https://test.api.amadeus.com/v2/shopping/");

            var obj = httpClient.GetAsync("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArrive + "&departureDate=" + d + "&adults=" + adults + "&children=" + children + "&infants=" + infants + "&travelClass=" + travel_class + "&nonStop=" + non_stop).Result;
            System.Diagnostics.Debug.WriteLine("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArrive + "&departureDate=" + d + "&adults=" + adults + "&children=" + children + "&infants=" + infants + "&travelClass=" + travel_class + "&nonStop=" + non_stop);
            System.Diagnostics.Debug.WriteLine(obj);
            if (obj.IsSuccessStatusCode)
            {
                var response = obj.Content.ReadAsStringAsync().Result;

                Api list = JsonConvert.DeserializeObject<Api>(response);

                System.Diagnostics.Debug.WriteLine("resultat  : " + list);
                System.Diagnostics.Debug.WriteLine("resultat  : " + list.data);
                foreach (var item in list.data)
                {
                    System.Diagnostics.Debug.WriteLine("resultat  : " + item);
                }
                return list;
            }


            return "error ... ";
        }
        public Object getFlightsRoundtrip(string depart, string arriver, string date_dep, string date_arr, int adults, int children, int infants, string travel_class, string non_stop)
        {
            var codeAiroDepart = getAirportcode(depart);
            var codeAiroArri = getAirportcode(arriver);
            string accessToken = getAccessToken();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.BaseAddress = new Uri("https://test.api.amadeus.com/v2/shopping/");
            var obj = httpClient.GetAsync("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArri + "&departureDate=" + date_dep + "&returnDate=" + date_arr + "&adults=" + adults + "&children=" + children + "&infants=" + infants + "&travelClass=" + travel_class + "&nonStop=" + non_stop).Result;
            System.Diagnostics.Debug.WriteLine("flight-offers?originLocationCode=" + codeAiroDepart + "&destinationLocationCode=" + codeAiroArri + "&departureDate=" + date_dep + "&returnDate=" + date_arr + "&adults=" + adults + "&children=" + children + "&infants=" + infants + "&travelClass=" + travel_class + "&nonStop=" + non_stop);
            if (obj.IsSuccessStatusCode)
            {
                var response = obj.Content.ReadAsStringAsync().Result;

                Api list = JsonConvert.DeserializeObject<Api>(response);

                System.Diagnostics.Debug.WriteLine("resultat  : " + list);
                System.Diagnostics.Debug.WriteLine("resultat  : " + list.data);
                foreach (var item in list.data)
                {
                    System.Diagnostics.Debug.WriteLine("resultat  : " + item);
                }
                return list;
            }


            return "error ... ";
        }
        


        public string getAccessToken()
        {
            string accessToken = "";
            HttpClient httpClient = new HttpClient();

            Confidentialite credentials = new Confidentialite();
            credentials.grant_type = "client_credentials";
            credentials.client_id = "bTMGY1T2GupcXUwd5LJrYV7W79xlRfV8";
            credentials.client_secret = "AioZgmpPVDcRKeBl";

            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("client_id", "bTMGY1T2GupcXUwd5LJrYV7W79xlRfV8"));
            nvc.Add(new KeyValuePair<string, string>("client_secret", "AioZgmpPVDcRKeBl"));
            nvc.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

            var req = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token")
            {
                Content = new FormUrlEncodedContent(nvc)
            };

            using (HttpResponseMessage result = httpClient.SendAsync(req).Result)
            {
                string resultJson = result.Content.ReadAsStringAsync().Result;
                Jeton list = JsonConvert.DeserializeObject<Jeton>(resultJson);
                accessToken = list.access_token;
                System.Diagnostics.Debug.WriteLine("resultat  TOKEN: {} = " + list.access_token);
            }



            return accessToken;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
