using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JD.Product.Crawler.Model;
using JD.Product.Crawler.Other;
using JD.Product.Crawler.Utils;

namespace JD.Product.Crawler.Service
{
    /// <summary>
    /// 产品表(分表)
    /// </summary>
    public class ProductService
    {
        public static string GetTableName(int sheetIndex)
        {
            string sheetSuffix = (sheetIndex + 1).ToString().PadLeft(3, '0');
            string tableName = $"Product_{sheetSuffix}";
            return tableName;
        }

        /// <summary>
        /// 初始化表
        /// </summary>
        public static void Init()
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            for (int sheetIndex = 0; sheetIndex < PublicConst.SheetNum; sheetIndex++)
            {
                string tableName = GetTableName(sheetIndex);
                conn.Execute($"IF  EXISTS ( SELECT * FROM sysobjects WHERE   id = OBJECT_ID('{tableName}') AND OBJECTPROPERTY(id, 'IsUserTable') = 1 )DROP TABLE dbo.{tableName};");

                conn.Execute($@"
CREATE TABLE [dbo].[{tableName}](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[ProductId] [BIGINT] NOT NULL,
	[CateId] [INT] NOT NULL,
	[Title] [NVARCHAR](200) NULL,
	[Price] [DECIMAL](18, 2) NULL,
	[Url] [NVARCHAR](100) NULL,
 CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY];");
            }
        }

        /// <summary>
        /// 批量插入，如果表不存在，创建
        /// </summary>
        /// <param name="cateList">集合</param>
        /// <param name="sheetIndex">表序号</param>
        /// <returns></returns>
        public static int InsertBulk(List<ProductUnit> cateList, int sheetIndex)
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            string sheetSuffix = (sheetIndex + 1).ToString().PadLeft(3, '0');
            string tableName = $"Product_{sheetSuffix}";
            return conn.Execute($"insert [dbo].[{tableName}](ProductId,CateId,Title,Price,Url) values(@ProductId,@CateId,@Title,@Price,@Url)",
                cateList);
        }

        public static void InsertGroupBulk(List<ProductGroupInput> groupList)
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);

            using (IDbConnection dbConnection = conn)
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    foreach (var group in groupList)
                    {
                        string sheetSuffix = (group.Id + 1).ToString().PadLeft(3, '0');
                        string tableName = $"Product_{sheetSuffix}";
                        dbConnection.Execute(
                            $"insert [dbo].[{tableName}](ProductId,CateId,Title,Price,Url) values(@ProductId,@CateId,@Title,@Price,@Url)",
                            group.ProductUnits, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    LogHelper.WriteLog($"InsertGroupBulk {e.Message}", e);
                }
            }
        }

        /// <summary>
        /// 得到对应表分页数据
        /// </summary>
        /// <param name="sheetIndex">表索引(1-40)</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        public static List<ProductUnit> GetPageList(int sheetIndex, int pageIndex, int pageSize)
        {
            string tableName = GetTableName(sheetIndex);
            string sql = $@"SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id ASC ) AS 'RowNumber' ,
                    *
          FROM      dbo.{tableName}
        ) AS a
WHERE   RowNumber BETWEEN ( {(pageIndex - 1) * pageSize + 1} ) AND ( {pageIndex * pageSize} );";

            var conn = new SqlConnection(PublicConst.ConnectiongString);
            return conn.Query<ProductUnit>(sql).ToList();
        }

        public static int GetTableCount(int sheetIndex)
        {
            var conn = new SqlConnection(PublicConst.ConnectiongString);
            string tableName = GetTableName(sheetIndex);
            string sql = $"SELECT COUNT(*) FROM [JDCrawler].[dbo].[{tableName}]";
            return conn.Query<int>(sql).FirstOrDefault();
        }
    }
}
