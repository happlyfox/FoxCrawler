using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.DAL
{
    public class DBInit
    {
        public static bool InitDb()
        {
            bool f = true;
            try
            {
                CategoryDAL.InitDb();
                CommodityDAL.InitDb();
            }
            catch (Exception e)
            {
                f = false;
                throw e;
            }
            return f;
        }

        public static float GetDBSize()
        {
            var conn = new SqlConnection(StaticConst.SqlDbCon);
            var dbsize = conn.Query<float>(@"select convert(float,size) * (8192.0/1024.0)/1024 AS Size from SNCrawler.dbo.sysfiles
            where NAME = 'SNCrawler'").FirstOrDefault();
            return dbsize;
        }
    }
}
