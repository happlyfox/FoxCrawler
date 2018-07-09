using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fox.ClawerSN.Model;
using Fox.ClawerSN.Service;
using Fox.ClawerSN.Util;

namespace Fox.ClawerSN.LongTimeWork
{
    public class CategoryUpdateWork
    {
        private readonly CategoryService _categoryService = new CategoryService();

        // 定义事件使用的委托
        public delegate void ValueChangedEventHandler(object sender, ValueEventArgs e);

        // 定义一个事件来提示界面工作的进度
        public event ValueChangedEventHandler ValueChanged;

        // 触发事件的方法
        protected void OnValueChanged(ValueEventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        public void CategoryUpdate(List<POCO_Category> list)
        {
            Task task = Task.Factory.StartNew(() => { _categoryService.StartUpdateCategory(list); });

            while (!task.IsCompleted)
            {
                int nowCount = _categoryService.GetCount();
                // 触发事件
                ValueEventArgs e = new ValueEventArgs() { Value = (int)((nowCount * 1.0 / list.Count) * 100) };
                OnValueChanged(e);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
