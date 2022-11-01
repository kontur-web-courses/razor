using BadNews.ModelBuilders.News;
using BadNews.Models.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

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
            var model = newsModelBuilder.BuildIndexModel(pageIndex, true, year);
            return View(model);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult FullArticle(Guid id)
        {
            var model = BuildFullArticleModel(id);
            return model == null ? NotFound() : View(model);
        }

        private FullArticleModel BuildFullArticleModel(Guid id)
        {
            const string cacheKeyBase = nameof(NewsController) + "_" + nameof(BuildFullArticleModel) + "_";
            string cacheKey = cacheKeyBase + id;
            if (!memoryCache.TryGetValue(cacheKey, out var years))
            {
                years = newsModelBuilder.BuildFullArticleModel(id);
                if (years != null)
                {
                    memoryCache.Set(cacheKey, years, new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromSeconds(30)
                    });
                }
            }

            return (FullArticleModel)years;
        }
    }
}