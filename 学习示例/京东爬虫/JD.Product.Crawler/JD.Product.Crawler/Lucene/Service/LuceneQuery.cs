using System;
using System.Collections.Generic;
using JD.Product.Crawler.Lucene.Interface;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneNetUtil = Lucene.Net.Util;

namespace JD.Product.Crawler.Lucene.Service
{
    public class LuceneQuery: ILuceneQuery
    {
        public List<ProductUnit> QueryIndex(string queryString)
        {
            IndexSearcher searcher = null;
            try
            {
                List<ProductUnit> ciList = new List<ProductUnit>();
                Directory dir = FSDirectory.Open(System.IO.Path.Combine(PublicConst.LucenePath));
                searcher = new IndexSearcher(dir);
                Analyzer analyzer = new PanGuAnalyzer();
                //--------------------------------------这里配置搜索条件
                QueryParser parser = new QueryParser(LuceneNetUtil.Version.LUCENE_30, "Title", analyzer);
                Query query = parser.Parse(queryString);
                Console.WriteLine(query.ToString()); //显示搜索表达式
                TopDocs docs = searcher.Search(query, null, 10);

                foreach (ScoreDoc sd in docs.ScoreDocs)
                {
                    Document doc = searcher.Doc(sd.Doc);
                    ciList.Add(new ProductUnit()
                    {
                        CateId = int.Parse(doc.Get("CateId")),
                        Price = double.Parse(doc.Get("Price")),
                        ProductId = long.Parse(doc.Get("ProductId")),
                        Title = doc.Get("Title"),
                        Url = doc.Get("Url"),
                    });
                }

                return ciList;
            }
            finally
            {
                if (searcher != null)
                {
                    searcher.Dispose();
                }
            }
        }
    }
}
