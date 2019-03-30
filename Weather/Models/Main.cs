using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Weather.Models
{
    public class Main
    {
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }
        public double Temp_Min { get; set; }
        public double Temp_Max { get; set; }
        public double Sea_Level { get; set; }
        public double Groud_Level { get; set; }
    }
}