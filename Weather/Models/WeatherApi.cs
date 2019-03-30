using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Weather.Models;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weather.Models
{

    public class DataObject
    {
        public string Name { get; set; }
    }
    public class WeatherApi
    {
        private const string CurrentWeatherURL = "https://api.openweathermap.org/data/2.5/weather?";
        private const string FiveDayForecast = "https://api.openweathermap.org/data/2.5/forecast?";
        private const string APIKey = "b72ebdc8a45151dd3c86c7ea4a695266";
        //private const string RectangelZoneURL = "http://api.openweathermap.org/data/2.5/box/city?";
        //private const string CircleZoneURL = "http://api.openweathermap.org/data/2.5/find"; 
        //private const string Rectangle = "?bbox=";
        private const string CityParam = "q=";
        private const string CityIdParam = "id=";
        private const string LatitudeParam = "lat=";
        private const string LongitudeParam = "lon=";
        private const string ZipCodeParam = "zip=";
        private const string APIKeyParam = "&APPID=" + APIKey;


        public static Forecast GetForecast(string CityName)
        {
            string apiurl = CurrentWeatherURL + CityParam + CityName + APIKeyParam;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(apiurl).Result;
            Forecast forecast = new Forecast();
            if (response.IsSuccessStatusCode)
            {
                var json = new WebClient().DownloadString(apiurl);
                JObject data = JObject.Parse(json);
                forecast = GetForecastFromJson(data);
                forecast.FiveDayForecast = GetFiveDayForecastByCityId(forecast.Id.ToString());
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return forecast;
        }

        public static List<Forecast> GetFiveDayForecastByCityId(string cityId)
        {
            string apiurl = FiveDayForecast + CityIdParam + cityId + APIKeyParam;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(apiurl).Result;
            var json = new WebClient().DownloadString(apiurl);
            JObject data = JObject.Parse(json);
            List<Forecast> fivedayforecast = GetFiveDayForecastFromJson(data);

            return fivedayforecast;
        }

        public static List<Forecast> GetFiveDayForecastFromJson(JObject data)
        {
            List<Forecast> FiveDays = new List<Forecast>();

            //We use 40 becase the api returns returns data for 5 days every 3 hours. 
            for (int i = 0; i < 39; i++)
            {
                Forecast forecast = new Forecast();
                forecast.Weather.Id = Int32.Parse(data["list"][i]["weather"][0]["id"].ToString());
                forecast.Weather.Main = data["list"][i]["weather"][0]["main"].ToString();
                forecast.Weather.Description = data["list"][i]["weather"][0]["description"].ToString();
                forecast.Weather.Icon = data["list"][i]["weather"][0]["icon"].ToString();
                forecast.Main.Temperature = ConvertKelvinToFahrenheit(Double.Parse(data["list"][i]["main"]["temp"].ToString()));
                forecast.Main.Pressure = Double.Parse(data["list"][i]["main"]["pressure"].ToString());
                forecast.Main.Humidity = Double.Parse(data["list"][i]["main"]["humidity"].ToString());
                forecast.Main.Temp_Min = ConvertKelvinToFahrenheit(Double.Parse(data["list"][i]["main"]["temp_min"].ToString()));
                forecast.Main.Temp_Max = ConvertKelvinToFahrenheit(Double.Parse(data["list"][i]["main"]["temp_max"].ToString()));
                JToken token = data["list"][i]["rain"];
                if (token != null)
                {
                    if (token.HasValues)
                    {
                        forecast.Rain.Volume = data["list"][i]["rain"]["3h"].ToString();
                    }
                }
                else
                {
                    forecast.Rain.Volume = "None";
                }

                token = data["list"][i]["snow"];
                if (token != null)
                {
                    if (token.HasValues)
                    {
                        forecast.Snow.Volume = data["snow"]["1h"].ToString();
                    }
                }
                else
                {
                    forecast.Snow.Volume = "None";
                }

                token = data["list"][i]["wind"];
                if (token != null)
                {
                    if (token.HasValues && token != null)
                    {
                        forecast.Wind.Speed = Double.Parse(data["list"][i]["wind"]["speed"].ToString());
                    }
                }

                token = data["list"][i]["clouds"];
                if (token != null)
                {
                    if (token.HasValues && token != null)
                    {
                        forecast.Clouds.Cloudiness = Double.Parse(data["list"][i]["clouds"]["all"].ToString());
                    }
                }

                FiveDays.Add(forecast);
            }

            return FiveDays;
        }


        public static Forecast GetForecastFromJson(JObject data)
        {
            //Need to add other values from forecase object like snow and rain. API apparently only returns values that are currently relevant.
            //I'll have to check for null values also when dooing this because some of these values might not exist on certain days.

            //JArray a = JArray.Parse(data.ToString());
            Forecast forecast = new Forecast();
            double test = Double.Parse(data["coord"]["lon"].ToString());
            forecast.Coordinates.Longitude = Double.Parse(data["coord"]["lon"].ToString());
            forecast.Coordinates.Latitude = Double.Parse(data["coord"]["lat"].ToString());
            forecast.Weather.Id = Int32.Parse(data["weather"][0]["id"].ToString());
            forecast.Weather.Main = data["weather"][0]["main"].ToString();
            forecast.Weather.Description = data["weather"][0]["description"].ToString();
            forecast.Weather.Icon = data["weather"][0]["icon"].ToString();
            //forecast.Wind.Gust = Double.Parse(data["wind"]["gust"].ToString());
            if (data["sys"]["type"] != null)
                forecast.System.Type = Int32.Parse(data["sys"]["type"].ToString());
            else
                forecast.System.Type = 0;
            //forecast.System.Type = Int32.Parse(data["sys"]["type"].ToString());
            if (data["sys"]["id"] != null)
                forecast.System.Id = Int32.Parse(data["sys"]["id"].ToString());
            else
                forecast.System.Id = 0;
            //forecast.System.Id = Int32.Parse(data["sys"]["id"].ToString());
            forecast.System.Message = Double.Parse(data["sys"]["message"].ToString());
            forecast.System.Country = data["sys"]["country"].ToString();
            forecast.System.Sunrise = Int32.Parse(data["sys"]["sunrise"].ToString());
            forecast.System.Sunset = Int32.Parse(data["sys"]["sunset"].ToString());
            forecast.Main.Temperature = ConvertKelvinToFahrenheit(Double.Parse(data["main"]["temp"].ToString()));
            forecast.Main.Pressure = Double.Parse(data["main"]["pressure"].ToString());
            forecast.Main.Humidity = Double.Parse(data["main"]["humidity"].ToString());
            forecast.Main.Temp_Min = ConvertKelvinToFahrenheit(Double.Parse(data["main"]["temp_min"].ToString()));
            forecast.Main.Temp_Max = ConvertKelvinToFahrenheit(Double.Parse(data["main"]["temp_max"].ToString()));
            forecast.Id = Int32.Parse(data["id"].ToString());
            forecast.City = data["name"].ToString();
            forecast.Cod = Int32.Parse(data["cod"].ToString());
            if (data["rain"] != null)
            {
                forecast.Rain.Volume = data["rain"]["1h"].ToString();
            }
            else
            {
                forecast.Rain.Volume = "None";
            }

            if (data["snow"] != null)
            {
                forecast.Snow.Volume = data["snow"]["1h"].ToString();
            }
            else
            {
                forecast.Snow.Volume = "None";
            }

            if (data["wind"] != null)
            {
                forecast.Wind.Speed = Double.Parse(data["wind"]["speed"].ToString());
                //forecast.Wind.Degree = Double.Parse(data["wind"]["deg"].ToString());
            }
            if (data["clouds"] != null)
            {
                forecast.Clouds.Cloudiness = Double.Parse(data["clouds"]["all"].ToString());
            }

            return forecast;
        }

        public static double ConvertKelvinToFahrenheit(double k)
        {
            return 1.8 * (k - 273) + 32; ;
        }
    }
}