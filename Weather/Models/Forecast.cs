using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather.Models
{
    public class Forecast
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int Cod { get; set; }
        public Coordinates Coordinates = new Coordinates();
        public Weather Weather = new Weather();
        public Main Main = new Main();
        public Wind Wind = new Wind();
        public Clouds Clouds = new Clouds();
        public Rain Rain = new Rain();
        public Snow Snow = new Snow();
        public Dt Dt = new Dt();
        public System System = new System();
        public List<Forecast> FiveDayForecast { get; set; }
    }
}