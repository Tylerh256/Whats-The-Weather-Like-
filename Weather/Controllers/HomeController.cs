using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Weather.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.UI;
namespace Weather.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            string cityname = Request.QueryString["City"];
            if (!string.IsNullOrEmpty(cityname))
            {
                try
                {
                    Forecast forecast = WeatherApi.GetForecast(cityname);
                    List<Forecast> test = WeatherApi.GetFiveDayForecastByCityId(forecast.Id.ToString());
                    return View(forecast);
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Oops... Something went wrong :(. Please enter in the name of a city.";
                    return View();
                }
            }
            return View(new Forecast());
        }

        [HttpPost]
        public ActionResult Index(Forecast forecast)
        {
            Forecast fc = WeatherApi.GetForecast("Seattle");
            return RedirectToAction("Index", new { City = forecast.City });
        }
    }
}