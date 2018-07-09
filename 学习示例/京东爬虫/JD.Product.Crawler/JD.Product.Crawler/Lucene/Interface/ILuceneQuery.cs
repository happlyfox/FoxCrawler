using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JD.Product.Crawler.Model;

namespace JD.Product.Crawler.Lucene.Interface
{
    public interface ILuceneQuery
    {
        List<ProductUnit> QueryIndex(string queryString);
    }
}
