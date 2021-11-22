using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BadNews.Repositories.News
{
    public class NewsRepository : INewsRepository
    {
        private static readonly string recordSeparator = "<--e891b395-4498-4f93-84a5-19b867d826ae-->";
        private readonly string dataFileName;
        private string DataFilePath => $"./.db/{dataFileName}";

        private readonly object dataFileLocker = new object();
        private static readonly Dictionary<Guid, long> UnorderedIndex = new Dictionary<Guid, long>();
        private static Guid _lastArticle = Guid.Empty;

        public NewsRepository(string dataFileName = "news.txt")
        {
            this.dataFileName = dataFileName;
        }

        public void InitializeDataBase(IEnumerable<NewsArticle> articles)
        {
            var dataFileInfo = new FileInfo(DataFilePath);
            if (dataFileInfo.Exists)
                dataFileInfo.Delete();
            if (!dataFileInfo.Directory.Exists)
                dataFileInfo.Directory.Create();

            lock (dataFileLocker)
            {
                var file = dataFileInfo.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None);
                using (var fileWriter = new StreamWriter(file))
                {
                    foreach (var article in articles)
                    {
                        var storedArticle = new NewsArticle(article);
                        if (storedArticle.Id == Guid.Empty)
                            storedArticle.Id = Guid.NewGuid();
                        AppendArticle(fileWriter, storedArticle);
                    }
                }
            }
            
            IndexData();
        }

        private void IndexData()
        {
            ReadFromFile((meta, data, offset) =>
            {
                var article = JsonConvert.DeserializeObject<NewsArticle>(data.ToString());
                if (!article.IsDeleted)
                {
                    UnorderedIndex[article.Id] = offset;
                    _lastArticle = article.Id;
                }

                return false;
            }, _lastArticle);
        }

        public NewsArticle GetArticleById(Guid id)
        {
            if (!UnorderedIndex.ContainsKey(id))
                return null;
            
            NewsArticle article = null;
            var idString = id.ToString();
            
            ReadFromFile((meta, data, offset) =>
            {
                if (idString == meta.ToString())
                {
                    var obj = JsonConvert.DeserializeObject<NewsArticle>(data.ToString());
                    if (id != obj.Id)
                        throw new InvalidDataException();
                    article = obj;
                }
                return true;
            }, id);

            return article != null && !article.IsDeleted ? article : null;
        }

        public IList<NewsArticle> GetArticles(Func<NewsArticle, bool> predicate = null)
        {
            var articles = new Dictionary<Guid, NewsArticle>();

            ReadFromFile((meta, data, offset) =>
            {
                var id = Guid.Parse(meta.ToString());
                var obj = JsonConvert.DeserializeObject<NewsArticle>(data.ToString());
                if (id != obj.Id)
                    throw new InvalidDataException();
                if (obj.IsDeleted || predicate == null || predicate(obj))
                    articles[id] = obj;
                return false;
            });

            return articles
                .Where(it => !it.Value.IsDeleted)
                .Select(it => it.Value)
                .OrderByDescending(it => it.Date)
                .ToList();
        }

        public IList<int> GetYearsWithArticles()
        {
            var years = new Dictionary<Guid, DateTime>();

            ReadFromFile((meta, data, offset) =>
            {
                var obj = JsonConvert.DeserializeObject<NewsArticle>(data.ToString());
                if (!obj.IsDeleted)
                    years[obj.Id] = obj.Date;
                else
                    years.Remove(obj.Id);
                return false;
            });

            return years.Select(it => it.Value.Year)
                .Distinct()
                .OrderByDescending(it => it)
                .ToArray();
        }

        public Guid CreateArticle(NewsArticle article)
        {
            if (article.Id != Guid.Empty)
                throw new InvalidOperationException("Creating article should not have id");

            NewsArticle storedArticle;
            lock (dataFileLocker)
            {
                var file = new FileStream(DataFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                
                using (var fileWriter = new StreamWriter(file))
                {
                    storedArticle = new NewsArticle(article)
                    {
                        Id = Guid.NewGuid()
                    };
                    AppendArticle(fileWriter, storedArticle);
                }
            }
            IndexData();
            return storedArticle.Id;
        }

        public void DeleteArticleById(Guid id)
        {
            lock (dataFileLocker)
            {
                var file = new FileStream(DataFilePath, FileMode.Append, FileAccess.Write, FileShare.None);
                using (var fileWriter = new StreamWriter(file))
                {
                    var storedArticle = new NewsArticle()
                    {
                        Id = id,
                        IsDeleted = true
                    };
                    AppendArticle(fileWriter, storedArticle);
                }
            }
            IndexData();
        }

        private static void AppendArticle(StreamWriter file, NewsArticle article)
        {
            var meta = article.Id.ToString();
            var data = JsonConvert.SerializeObject(article, Formatting.Indented);
            file.WriteLine(recordSeparator);
            file.WriteLine(meta);
            file.WriteLine(data);
        }

        private void ReadFromFile(
            Func<StringBuilder, StringBuilder, long, bool> onObjectRead, Guid? articleToSeek = null)
        {
            if (!File.Exists(DataFilePath))
                return;

            lock (dataFileLocker)
            {
                long offset = 0;
                var file = new FileStream(DataFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (var fileReader = new SeekableStreamTextReader(file, Encoding.UTF8))
                {
                    if (articleToSeek != null && articleToSeek.Value != Guid.Empty)
                    {
                        offset = UnorderedIndex[articleToSeek.Value];
                        fileReader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    }
                    int objectLine = 0;
                    var metaBuilder = new StringBuilder();
                    var dataBuilder = new StringBuilder();

                    string line = fileReader.ReadLine();
                    while (line != null)
                    {
                        var off = fileReader.UsedBytes;
                        if (line != recordSeparator)
                        {
                            if (objectLine++ > 0)
                            {
                                dataBuilder.Append(line);
                            }
                            else
                            {
                                metaBuilder.Append(line);
                            }
                        }
                        else
                        {
                            if (metaBuilder.Length > 0 || dataBuilder.Length > 0)
                            {
                                if (onObjectRead(metaBuilder, dataBuilder, offset))
                                    return;
                            }
                            offset = fileReader.UsedBytes;

                            objectLine = 0;
                            metaBuilder = new StringBuilder();
                            dataBuilder = new StringBuilder();
                        }

                        line = fileReader.ReadLine();
                    }

                    if (dataBuilder.Length > 0)
                    {
                        if (onObjectRead(metaBuilder, dataBuilder, offset))
                            return;
                    }
                }
            }
        }
    }
}
