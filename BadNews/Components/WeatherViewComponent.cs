using System.Threading.Tasks;
using BadNews.Repositories.News;
using BadNews.Repositories.Weather;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class WeatherViewComponent : ViewComponent
    {
        private readonly IWeatherForecastRepository weatherForecastRepository;

        public WeatherViewComponent(IWeatherForecastRepository newsRepository)
        {
            this.weatherForecastRepository = newsRepository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var weatherForecast = weatherForecastRepository.GetWeatherForecastAsync();
            return View(weatherForecast);
        }
    }
}