using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>    
    /// 实体转换辅助类    
    /// / 获得查询结果  
    //// DataTable dt = DbHelper.ExecuteDataTable(...);  
    // 把DataTable转换为IList<UserInfo>  
    ///IList<UserInfo> users = ModelConvertHelper<UserInfo>.ConvertToModel(dt);
    /// </summary>    
    public static class ModelConvertHelper
    {
        public static DataTable ConvertToDataSet<T>(IList<T> list)
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            System.Reflection.PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();

                for (int i = 0, j = myPropertyInfo.Length; i < j; i++)
                {
                    System.Reflection.PropertyInfo pi = myPropertyInfo[i];

                    string name = pi.Name;
                     //if (pi.PropertyType == System.Nullable<>)
                     //{
                     // pi.PropertyType = System.Int32;
                     //}
                    if (dt.Columns[name] == null)
                    {
                        if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))//这段是我进行修改和判断的地方
                        {
                            Type[] typeArray = pi.PropertyType.GetGenericArguments();
                            Type baseType = typeArray[0];
                            column = new DataColumn(name, baseType);
                        }
                        else
                        {
                            column = new DataColumn(name, pi.PropertyType);
                        }
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null) == null ? DBNull.Value : pi.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}
