using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Util;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using LuceneNet = Lucene.Net;
using Token = Lucene.Net.QueryParsers.Token;

namespace Fox.ClawerSN.TaskForm
{
    public partial class LuceneSearch : Form
    {
        public LuceneSearch()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Segment.Init();
            Segment segment = new Segment();
            ICollection<WordInfo> textwords = segment.DoSegment(Title.Text);
            ICollection<WordInfo> descwords = segment.DoSegment(Description.Text);
            string word1Str = string.Empty, word2Str = string.Empty;
            foreach (var word in textwords)
            {
                word1Str += word.Word + " ";
                Console.WriteLine(word.Word);
            }
            foreach (var word in descwords)
            {
                word2Str += word.Word + " ";
                Console.WriteLine(word.Word);
            }

            Result.Text = $"{word1Str}\n\r{word2Str}";

            // 索引目录
            LuceneNet.Store.Directory dir = FSDirectory.Open(new DirectoryInfo(StaticConst.LucenePath));
            // 索引搜索器
            IndexSearcher searcher = new IndexSearcher(dir, true);
            QueryParser qp1 = new QueryParser(LuceneNet.Util.Version.LUCENE_29, "Title", new PanGuAnalyzer(true));
            QueryParser qp2 = new QueryParser(LuceneNet.Util.Version.LUCENE_29, "Description", new PanGuAnalyzer(true));
            BooleanQuery bq = new BooleanQuery();
            bq.Add(qp1.Parse(word1Str), Occur.SHOULD);
            if (!string.IsNullOrEmpty(word2Str))
            {
                bq.Add(qp2.Parse(word2Str), Occur.SHOULD);

            }
            TopDocs topDocs = searcher.Search(bq, 20);
            dataGridView1.DataSource = PrintfDocs(searcher, topDocs);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PanGu.Segment.Init();
            PanGu.Segment segment = new PanGu.Segment();
            ICollection<PanGu.WordInfo> words = segment.DoSegment("山东落花生花落东山，长春市长春花店");
            foreach (var word in words)
            {
                Console.WriteLine(word.Word);
            }

            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter = new PanGu.HighLight.SimpleHTMLFormatter("<font color=\"red\">", "</font>");
            PanGu.HighLight.Highlighter highlighter = new PanGu.HighLight.Highlighter(simpleHTMLFormatter, new PanGu.Segment());
            highlighter.FragmentSize = 100; // 设置每个摘要段的字符数
            string keywords = "信号/道路/开通";
            string content = @"高德完胜百度。我专门花了几个星期，在我所在的城市测试两个地图，高德数据不准确在少数，而百度就是家常便饭了，表现为：
已经管制一年的道路（双向变单向），百度仍然提示双向皆可走。
已经封闭数年的道路，百度仍然说是通的。
新修道路，还没有开通，百度居然让走。
有时候规划路线时明明是正确的，但是导航过程中，就出乱子，信号没问题、路线不复杂，明明是要左转，百度却叫右转。";
            string abs = highlighter.GetBestFragment(keywords, content);
            Console.WriteLine(abs);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string q = GetKeyWordsSplitBySpace("潮流新配色", new PanGuTokenizer()); ;
            // 索引目录
            LuceneNet.Store.Directory dir = FSDirectory.Open(new DirectoryInfo(StaticConst.LucenePath));
            // 索引搜索器
            IndexSearcher searcher = new IndexSearcher(dir, true);
            // 查询器
            // QueryParser qp = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Title", new PanGuAnalyzer(true));
            MultiFieldQueryParser qp = new MultiFieldQueryParser(LuceneNet.Util.Version.LUCENE_29, new string[] { "Title", "Description" }, new PanGuAnalyzer(true));
            Query query = qp.Parse(q);
            TopDocs topDocs = searcher.Search(query, 20);
            PrintfDocs(searcher, topDocs);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string q = GetKeyWordsSplitBySpace("4GB+128GB", new PanGuTokenizer()); ;
            // 索引目录
            LuceneNet.Store.Directory dir = FSDirectory.Open(new DirectoryInfo(StaticConst.LucenePath));
            // 索引搜索器
            IndexSearcher searcher = new IndexSearcher(dir, true);
            QueryParser qp1 = new QueryParser(LuceneNet.Util.Version.LUCENE_29, "Title", new PanGuAnalyzer(true));
            QueryParser qp2 = new QueryParser(LuceneNet.Util.Version.LUCENE_29, "Description", new PanGuAnalyzer(true));
            BooleanQuery bq = new BooleanQuery();
            bq.Add(qp1.Parse(q), Occur.SHOULD);
            bq.Add(qp2.Parse(q), Occur.SHOULD);
            TopDocs topDocs = searcher.Search(bq, 20);
            PrintfDocs(searcher, topDocs);
        }

        private List<LuceneShowOutput> PrintfDocs(IndexSearcher searcher, TopDocs topDocs)
        {
            List<LuceneShowOutput> comList = new List<LuceneShowOutput>();
            for (int i = 0; i < topDocs.ScoreDocs.Count(); i++)
            {
                Document doc = searcher.Doc(topDocs.ScoreDocs[i].Doc);
                string title = doc.GetField("Title").StringValue;
                string url = doc.GetField("Url").StringValue;
                Console.WriteLine(title + "\r\n" + url + "\r\n" + topDocs.ScoreDocs[i].Score);
                comList.Add(new LuceneShowOutput()
                {
                    Title = title,
                    Url = url
                });
            }

            return comList;
        }

        public static string GetKeyWordsSplitBySpace(string keywords, PanGuTokenizer ktTokenizer)
        {
            StringBuilder result = new StringBuilder();
            ICollection<WordInfo> words = ktTokenizer.SegmentToWordInfos(keywords);
            foreach (WordInfo word in words)
            {
                if (word == null)
                {
                    continue;
                }
                result.AppendFormat("{0}^{1}.0 ", word.Word, (int)Math.Pow(3, word.Rank));
            }
            return result.ToString().Trim();
        }
    }
}
