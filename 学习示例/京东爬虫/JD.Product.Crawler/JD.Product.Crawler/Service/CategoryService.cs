using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;

namespace JD.Product.Crawler.Service
{
    public class CategoryService
    {
        /// <summary>
        /// 重置类别表
        /// </summary>
        public void ResetTable()
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            var enumberQuery = conn.Query<dynamic>(@"select * FROM sysobjects where id = object_id('Catagory') 
and OBJECTPROPERTY(id, 'IsUserTable') = 1");

            if (enumberQuery.Any())
            {
                conn.Execute(DropTable());
                conn.Execute(CreateTable());
            }
            else
            {
                conn.Execute(CreateTable());
            }
        }


        private string CreateTable()
        {
            string sql = @"CREATE TABLE [dbo].[Catagory](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[Code] [NVARCHAR](200) NULL,
	[PCode] [NVARCHAR](200) NULL,
	[Name] [NVARCHAR](200) NULL,
	[Levels] [INT] NULL,
	[Url] [NVARCHAR](200) NULL,
 CONSTRAINT [PK_Catagory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]";
            return sql;
        }

        private string DropTable()
        {
            string sql = @"DROP TABLE [dbo].[Catagory]";
            return sql;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        public int InsertBulk(List<Category> cateList)
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            return conn.Execute("insert [dbo].[Catagory](Code,PCode,Name,Url,Levels) values(@Code,@PCode,@Name,@Url,@Levels)",
                 cateList);
        }

        /// <summary>
        /// 清除垃圾数据
        /// </summary>
        public int DeleteRubbish()
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            return conn.Execute("DELETE FROM [JDCrawler].[dbo].[Catagory] WHERE pcode = ','");
        }


        public List<Category> SelectAll()
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
           return conn.Query<Category>("SELECT * FROM [JDCrawler].[dbo].[Catagory]").ToList();
        }
    }
}
