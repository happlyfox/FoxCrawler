using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using Dapper;

namespace YouKuCrawlerAsync
{
    /// <summary>
    ///     数据库任务
    /// </summary>
    public class DBMission
    {
        private static readonly string _connectionStr =
            ConfigurationManager.ConnectionStrings["ConnectDB"].ConnectionString;

        /// <summary>
        ///     插入爬取的视频类别
        /// </summary>
        public static void InsertTypes()
        {
            var conn = new SqlConnection(_connectionStr);
            conn.Execute("insert VideoType(Code,Name) values(@Code,@Name)", VideoCrawer.GetVideoTypes());
        }

        /// <summary>
        ///     插入爬取的视频内容
        /// </summary>
        public static void InsertContent()
        {
            //限定20个线程运行
            ThreadPool.SetMaxThreads(20, 20);
            foreach (var type in VideoCrawer.GetVideoTypes())
            {
                var node = type;
                ThreadPool.QueueUserWorkItem(u =>
                {
                    var conn = new SqlConnection(_connectionStr);
                    //得到当前类别总分页数
                    var pageCount = VideoCrawer.GetPageCountByCode(node.Code);
                    //遍历分页得到内容后插入数据库
                    for (var i = 1; i <= pageCount; i++)
                    {
                        conn.Execute(
                            "insert VideoContent(Title,Href,ImgHref,Hits,Code,PageIndex) values(@Title,@Href,@ImgHref,@Hits,@Code,@PageIndex)",
                            VideoCrawer.GetContentsByCode(node.Code, i));

                        Console.WriteLine($"编码{node.Code}\t页码{i}\t成功");
                    }
                });
            }
        }
    }
}