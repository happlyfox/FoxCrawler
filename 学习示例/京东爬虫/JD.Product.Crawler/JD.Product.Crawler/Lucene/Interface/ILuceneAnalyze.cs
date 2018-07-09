using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JD.Product.Crawler.Lucene.Interface
{
    public  interface ILuceneAnalyze
    {
        string[] AnalyzerKey(string field, string keyword);

        string CleanKeyword(string keyword);
    }
}
