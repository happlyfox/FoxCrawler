using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using JD.Product.Crawler.Lucene.Interface;
using JD.Product.Crawler.Lucene.Service;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;
using JD.Product.Crawler.Service;
using JD.Product.Crawler.Utils;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using LuceneNetUtil = Lucene.Net.Util;

namespace JD.Product.Crawler
{
    public partial class LuceneIndexDemo : Form
    {
        public LuceneIndexDemo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //ILuceneBulid builder = new LuceneBulid();

            //string[] strings = new string[10];
            //for (int i = 0; i < 10; i++)
            //{
            //    strings[i] = i.ToString("000");
            //}
            //builder.MergeAllLuceneIndex(strings);

            //CategoryService cs = new CategoryService();
            //List<Category> cateList = cs.SelectAll();

            //for (int i = 0; i < 10; i++)
            //{
            //    FSDirectory directory = FSDirectory.Open(System.IO.Path.Combine(PublicConst.LucenePath, i.ToString("000")));//文件夹
            //    using (IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), true, IndexWriter.MaxFieldLength.LIMITED))//索引写入器
            //    {
            //        foreach (Category commdity in cateList)
            //        {
            //            Document doc = new Document();//一条数据
            //            //字段  列名  值   是否保存值  是否分词
            //            doc.Add(new Field("Id", commdity.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            //            doc.Add(new Field("Code", commdity.Code, Field.Store.YES, Field.Index.NOT_ANALYZED));
            //            doc.Add(new Field("PCode", commdity.PCode, Field.Store.YES, Field.Index.NOT_ANALYZED));
            //            doc.Add(new Field("Name", commdity.Name, Field.Store.YES, Field.Index.ANALYZED));
            //            writer.AddDocument(doc);//写进去
            //        }
            //        writer.Optimize();//优化  就是合并
            //    }
            //}

            MessageBox.Show("索引建立成功!", "提示");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FSDirectory dir = FSDirectory.Open(PublicConst.LucenePath);
            //IndexSearcher searcher = new IndexSearcher(dir);//查找器
            //{
            //    TermQuery query = new TermQuery(new Term("Name", "配件"));//包含
            //    TopDocs docs = searcher.Search(query, null, 1000);//找到的数据
            //    foreach (ScoreDoc sd in docs.ScoreDocs)
            //    {
            //        Document doc = searcher.Doc(sd.Doc);
            //        Console.WriteLine("***************************************");
            //        Console.WriteLine(string.Format("Id={0}", doc.Get("Id")));
            //        Console.WriteLine(string.Format("Code={0}", doc.Get("Code")));
            //        Console.WriteLine(string.Format("PCode={0}", doc.Get("PCode")));
            //        Console.WriteLine(string.Format("Name={0}", doc.Get("Name")));
            //        Console.WriteLine(string.Format("Levels={0}", doc.Get("Levels")));
            //    }
            //    Console.WriteLine("一共命中了{0}个", docs.TotalHits);
            //}

            ILuceneQuery query = new LuceneQuery();
            var list = query.QueryIndex("手机");

            foreach (var item in list)
            {
                Console.WriteLine($"Title{item.Title} Price{item.Price}  ProductId{item.ProductId} CateId{item.CateId} ");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            QueryParser parser = new QueryParser(LuceneNetUtil.Version.LUCENE_30, "test", new PanGuAnalyzer());
            Query query = parser.Parse("周AND 测 试!&'");
            Console.WriteLine(query.ToString());
        }

    }
}
