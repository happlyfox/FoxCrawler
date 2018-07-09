using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.DAL;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.WebAnalysis;

namespace Fox.ClawerSN.Service
{
    public class CategoryService
    {
        private readonly CategoryDAL _categoryDal = new CategoryDAL();

        public int StartUpdateCategory(List<POCO_Category> list)
        {
            return _categoryDal.InsertBulk(list);
        }

        public int Init()
        {
            return CategoryDAL.InitDb();
        }

        public List<POCO_Category> GetAll()
        {
            return _categoryDal.GetAll();
        }

        public List<POCO_Category> GetListByLevel(int level)
        {
            return _categoryDal.GetAll().Where(u => u.Levels == level).ToList();
        }

        public int GetCount()
        {
            return _categoryDal.GetCount();
        }

        public void BatchUpdatePage(Dictionary<string, int> pageDics)
        {
            _categoryDal.BatchUpdatePage(pageDics);
        }
    }
}
