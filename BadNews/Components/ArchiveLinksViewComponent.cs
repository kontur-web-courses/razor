using System;
using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BadNews.Components
{
    public class ArchiveLinksViewComponent : ViewComponent
    {
        private readonly IMemoryCache memoryCache;
        private readonly INewsRepository newsRepository;

        public ArchiveLinksViewComponent(INewsRepository newsRepository, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.newsRepository = newsRepository;
        }

        public IViewComponentResult Invoke()
        {
            string cacheKey = nameof(ArchiveLinksViewComponent);
            if (!memoryCache.TryGetValue(cacheKey, out var years))
            {
                years = newsRepository.GetYearsWithArticles();
                if (years != null)
                {
                    memoryCache.Set(cacheKey, years, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    });
                }
            }
            return View(years);
        }
    }
}