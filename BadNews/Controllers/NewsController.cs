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
            var pageModel = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(pageModel);
        }
        
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid? id)
        {
            var cacheKey = id;
            if (!memoryCache.TryGetValue(cacheKey, out var pageModel))
            {
                pageModel = id.HasValue ? newsModelBuilder.BuildFullArticleModel(id.Value) : null;
                if (pageModel != null)
                {
                    memoryCache.Set(cacheKey, pageModel, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(10)
                    });
                }
                else
                    return NotFound();
            }

            return View(pageModel);
        }
    }
}