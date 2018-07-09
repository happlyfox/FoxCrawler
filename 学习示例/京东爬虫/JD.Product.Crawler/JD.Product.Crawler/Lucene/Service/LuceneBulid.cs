using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JD.Product.Crawler.Lucene.Interface;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;
using JD.Product.Crawler.Service;
using JD.Product.Crawler.Utils;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Util;
using LuceneIO = Lucene.Net.Store;
using LuceneNetUtil = Lucene.Net.Util;


namespace JD.Product.Crawler.Lucene.Service
{
    public class LuceneBulid : ILuceneBulid
    {
        public void BuildIndex(List<TableIndexModel> ciList, string pathSuffix = "", bool isCreate = false)
        {
            IndexWriter writer = null;
            try
            {
                if (ciList == null || ciList.Count == 0)
                {
                    return;
                }

                string rootIndexPath = PublicConst.LucenePath;
                string indexPath = string.IsNullOrWhiteSpace(pathSuffix) ? rootIndexPath : string.Format("{0}\\{1}", rootIndexPath, pathSuffix);

                DirectoryInfo dirInfo = Directory.CreateDirectory(indexPath);
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, new PanGuAnalyzer(), isCreate, IndexWriter.MaxFieldLength.LIMITED);
                writer.SetMaxBufferedDocs(100);//控制写入一个新的segent前内存中保存的doc的数量 默认10  
                writer.MergeFactor = 100;//控制多个segment合并的频率，默认10
                writer.UseCompoundFile = true;//创建复合文件 减少索引文件数量

                foreach (var ciModel in ciList)
                {
                    var pageList = ProductService.GetPageList(ciModel.TableIndex, ciModel.PageIndex, PublicConst.PageGetCount);
                    LogHelper.WriteLog($"表{ProductService.GetTableName(ciModel.TableIndex)}:{(ciModel.PageIndex - 1) * 1000}-{ciModel.PageIndex * 1000}");
                    CreateDoxIndex(pageList, writer);
                }
            }
            finally
            {
                if (writer != null)
                {
                    //writer.Optimize();// 创建索引的时候不做合并  merge的时候处理
                    writer.Dispose();
                }
            }
        }

        public static void CreateDoxIndex(List<ProductUnit> list, IndexWriter indexWriter)
        {
            foreach (var pl in list)
            {
                Document doc = new Document();
                doc.Add(new Field("Id", pl.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("ProductId", pl.ProductId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("CateId", pl.CateId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Title", pl.Title.ToString(), Field.Store.YES, Field.Index.ANALYZED));//标题分词
                doc.Add(new NumericField("Price", Field.Store.YES, true).SetDoubleValue(pl.Price));
                doc.Add(new Field("Url", pl.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));

                indexWriter.AddDocument(doc);
            }
        }

        /// <summary>
        /// 将索引合并到上级目录
        /// </summary>
        /// <param name="sourceDir">子文件夹名</param>
        public void MergeAllLuceneIndex(string[] childDirs)
        {
            LogHelper.WriteLog("MergeIndex Start");
            IndexWriter writer = null;
            try
            {
                if (childDirs == null || childDirs.Length == 0) return;
                Analyzer analyzer = new StandardAnalyzer(LuceneNetUtil.Version.LUCENE_30);
                string rootPath = PublicConst.LucenePath;
                DirectoryInfo dirInfo = Directory.CreateDirectory(rootPath);
                LuceneIO.Directory directory = LuceneIO.FSDirectory.Open(dirInfo);
                writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED);//删除原有的
                LuceneIO.Directory[] dirNo = childDirs.Select(dir => LuceneIO.FSDirectory.Open(Directory.CreateDirectory(string.Format("{0}\\{1}", rootPath, dir)))).ToArray();
                writer.MergeFactor = 100;//控制多个segment合并的频率，默认10
                writer.UseCompoundFile = true;//创建符合文件 减少索引文件数量
                writer.AddIndexesNoOptimize(dirNo);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Optimize();
                    writer.Dispose();;
                }
                LogHelper.WriteLog("MergeIndex End");
            }
        }

    }
}
