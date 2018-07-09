using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.DAL
{
    /// <summary>
    /// 商品数据访问类
    /// </summary>
    public class CommodityDAL
    {
        public static string GetTName(int Index)
        {
            return Index.ToString("00");
        }

        public static int InitDb()
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);

            var createSql = string.Join(" ", Enumerable.Range(1, 20).Select(u => $@"CREATE TABLE [dbo].[Commodity_{
                    GetTName(u)
                }]
            (
              [Id] [BIGINT] IDENTITY(1, 1) NOT NULL ,
              [SUId] [NVARCHAR](200) NULL ,
              [CategoryId] [NVARCHAR](200) NULL ,
              [Title] [NVARCHAR](200) NULL ,
              [Description] [NVARCHAR](200) NULL ,
              [Price] [DECIMAL](18, 2) NULL ,
              [Url] [NVARCHAR](100) NULL ,
              [ShopName] [NVARCHAR](50) NULL ,
              CONSTRAINT [PK_Commodity_{GetTName(u)}] PRIMARY KEY CLUSTERED ( [Id] ASC )
                WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,
                       IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
                       ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
            )
        ON  [PRIMARY];"));
            var truncateSql = string.Join(" ", Enumerable.Range(1, 20).Select(u => $@"TRUNCATE TABLE Commodity_{GetTName(u)}"));

            return conn.Execute($@"IF NOT EXISTS ( SELECT  *
                FROM    dbo.sysobjects
                WHERE   id = OBJECT_ID(N'[dbo].[Commodity_01]')
                        AND OBJECTPROPERTY(id, N'IsUserTable') = 1 )
                BEGIN
                    {createSql}
                END
ELSE
    {truncateSql}");
        }

        public void InsertGroupBulk(List<CommodityGroupInput> groupList)
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);

            using (IDbConnection dbConnection = conn)
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    foreach (var group in groupList)
                    {
                        string sheetSuffix = (group.Id + 1).ToString().PadLeft(2, '0');
                        string tableName = $"Commodity_{sheetSuffix}";
                        dbConnection.Execute(
                            $"insert [dbo].[{tableName}](SUId,CategoryId,Title,Description,Price,Url,ShopName) values(@SUId,@CategoryId,@Title,@Description,@Price,@Url,@ShopName)",
                            group.Units, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }

        public static string GetTableName(int sheetIndex)
        {
            string sheetSuffix = (sheetIndex + 1).ToString().PadLeft(2, '0');
            string tableName = $"Commodity_{sheetSuffix}";
            return tableName;
        }

        public int GetTableCount(int sheetIndex)
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            string tableName = GetTableName(sheetIndex);
            string sql = $"SELECT COUNT(*) FROM [dbo].[{tableName}]";
            return conn.Query<int>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 得到对应表分页数据
        /// </summary>
        /// <param name="sheetIndex">表索引</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        public  List<POCO_Commodity> GetPageList(int sheetIndex, int pageIndex, int pageSize)
        {
            string tableName = GetTableName(sheetIndex);
            string sql = $@"SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id ASC ) AS 'RowNumber' ,
                    *
          FROM      dbo.{tableName}
        ) AS a
WHERE   RowNumber BETWEEN ( {(pageIndex - 1) * pageSize + 1} ) AND ( {pageIndex * pageSize} );";

            var conn = new SqlConnection(StaticConst.SqlDbCon);
            return conn.Query<POCO_Commodity>(sql).ToList();
        }
    }
}
