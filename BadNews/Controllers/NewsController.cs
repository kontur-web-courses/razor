using System;
using BadNews.ModelBuilders.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BadNews.Controllers
{
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client, VaryByHeader = "Cookie")]
    public class NewsController : Controller
    {
        private readonly INewsModelBuilder newsModelBuilder;
        private readonly IMemoryCache memoryCache;

        public NewsController(INewsModelBuilder newsModelBuilder, IMemoryCache memoryCache)
        {
            this.newsModelBuilder = newsModelBuilder;
            this.memoryCache = memoryCache;
        }
        
        // GET
        public IActionResult Index(int? year, [FromQuery] int pageIndex = 0)
        {
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle([FromRoute] Guid id)
        {
            string cahceKey = $"{nameof(FullArticle)}_{id.ToString()}";
            if (!memoryCache.TryGetValue(cahceKey, out var model))
            {
                model = newsModelBuilder.BuildFullArticleModel(id);
                if (model != null)
                {
                    memoryCache.Set(cahceKey, model, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(30)
                    });
                }
            }
            
            if (model is null)
            {
                return NotFound();
            }
            return View(model);
        }
    }
}