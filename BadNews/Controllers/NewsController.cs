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

        public IActionResult Index(int? year, int pageIndex = 0)
        {
            var indexModel = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(indexModel);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid id)
        {
            string cacheKey = id.ToString();
            if (!memoryCache.TryGetValue(cacheKey, out var fullArticleModel))
            {
                fullArticleModel = newsModelBuilder.BuildFullArticleModel(id);
                if (fullArticleModel != null)
                {
                    memoryCache.Set(cacheKey, fullArticleModel, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration  = TimeSpan.FromSeconds(30)
                    });
                }
            }
            if (fullArticleModel== null)
                return NotFound();
            return View(fullArticleModel);
        }
    }
}