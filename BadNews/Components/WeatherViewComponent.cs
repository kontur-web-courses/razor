using System.Threading.Tasks;
using BadNews.Repositories.Weather;
using Microsoft.AspNetCore.Mvc;

namespace BadNews.Components
{
    public class WeatherViewComponent: ViewComponent
    {
        private IWeatherForecastRepository weatherForecastRepository;

        public WeatherViewComponent(IWeatherForecastRepository weatherForecastRepository)
        {
            this.weatherForecastRepository = weatherForecastRepository;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await weatherForecastRepository.GetWeatherForecastAsync();
            return View(data);
        }
    }
}