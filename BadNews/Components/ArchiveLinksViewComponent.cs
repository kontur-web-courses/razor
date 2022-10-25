using BadNews.Repositories.News;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BadNews.Components
{
    public class ArchiveLinksViewComponent : ViewComponent
    {
        private readonly INewsRepository newsRepository;
        private IMemoryCache memoryCache;
        public ArchiveLinksViewComponent(IMemoryCache memoryCache, INewsRepository newsRepository)
        {
            this.memoryCache = memoryCache;
            this.newsRepository = newsRepository;
        }

        public IViewComponentResult Invoke()
        {
            string cacheKey = nameof(ArchiveLinksViewComponent);
            if (!memoryCache.TryGetValue(cacheKey, out IList<int> years))
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