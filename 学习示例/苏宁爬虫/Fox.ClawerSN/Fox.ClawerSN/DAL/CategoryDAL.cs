using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.DAL
{
    public class CategoryDAL
    {
        public static int InitDb()
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            return conn.Execute(@"IF NOT EXISTS ( SELECT  *
                FROM    dbo.sysobjects
                WHERE   id = OBJECT_ID(N'[dbo].[Category]')
                        AND OBJECTPROPERTY(id, N'IsUserTable') = 1 )
    CREATE TABLE [dbo].[Category]
        (
          [Id] [NVARCHAR](50) NOT NULL ,
          [PId] [NVARCHAR](50) NOT NULL ,                         
          [Levels] [INT] NOT NULL ,
          [Code] [NVARCHAR](50) NOT NULL ,
          [Name] [NVARCHAR](200) NOT NULL ,
          [Url] [NVARCHAR](200) NULL ,
          [PageCount] [INT] NULL ,
          CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ( [Id] ASC )
            WITH ( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF,
                   IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON,
                   ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
        )
    ON  [PRIMARY];
ELSE
    TRUNCATE TABLE dbo.Category;");

        }

        public int InsertBulk(List<POCO_Category> list)
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            return conn.Execute("insert  INTO Category(Id,PId,Levels,Code,Name,Url) values(@Id,@PId,@Levels,@Code,@Name,@Url)", list);

        }

        public List<POCO_Category> GetAll()
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            return conn.Query<POCO_Category>("select * from Category").ToList();
        }

        public int GetCount()
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            return conn.Query<int>("select count(*) from Category").FirstOrDefault();
        }

        public void BatchUpdatePage(Dictionary<string, int> pageDics)
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);

            using (IDbConnection dbConnection = conn)
            {
                dbConnection.Open();
                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    foreach (var page in pageDics)
                    {
                        dbConnection.Execute(
                            $"UPDATE Category SET  PageCount = @PageCount WHERE Url = @Url",
                            new { PageCount = page.Value, Url = page.Key }, transaction);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    transaction.Rollback();
                }
            }
        }
    }
}
