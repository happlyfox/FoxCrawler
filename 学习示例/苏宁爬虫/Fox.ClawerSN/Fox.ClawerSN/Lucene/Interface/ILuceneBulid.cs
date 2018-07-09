using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.Model;

namespace Fox.ClawerSN.Lucene.Interface
{
    public interface ILuceneBulid
    {
        /// <summary>
        /// 批量创建索引
        /// </summary>
        /// <param name="ciList"></param>
        /// <param name="pathSuffix">索引目录后缀，加在电商的路径后面，为空则为根目录.如sa\1</param>
        /// <param name="isCreate">默认为false 增量索引  true的时候删除原有索引</param>
        void BuildIndex(List<TableIndexModel> ciList, string pathSuffix = "", bool isCreate = false);

        void MergeAllLuceneIndex(string[] v);
    }
}
