using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Utility
{
    public class WordUtility
    {
        /// <summary>
        /// word转pdf
        /// </summary>
        /// <param name="wordpath"></param>
        /// <param name="pdfpath"></param>
        /// <returns></returns>
        public static bool ConvertWordToPDF(string wordpath, string pdfpath)
        {
            bool flag = false;
            try
            {
                //读取doc文档
                Document doc = new Document(wordpath);
                //保存为PDF文件，此处的SaveFormat支持很多种格式，如图片，epub,rtf 等等
                doc.Save(pdfpath, SaveFormat.Pdf);
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        public static bool ConvertWord(string templatepath, DataSet ds, string wordpath)
        {
            bool flag = false;
            try
            {
                Document doc = new Document(templatepath);

                doc.MailMerge.ExecuteWithRegions(ds);
                doc.Save(wordpath, SaveOptions.CreateSaveOptions(SaveFormat.Doc));
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }
        public static Document doc = null;
        public static bool ReplaceWordContent(string templatepath, string wordpath, DataTable dt, DataTable casedt, DataTable skilldt, DataTable serverdt, DataTable achievementsdt, Dictionary<string, string> expPairColumn, Dictionary<string, string> caseexpPairColumn, Dictionary<string, string> skillexpPairColumn, Dictionary<string, string> serverexpPairColumn, Dictionary<string, string> achievementsexpPairColumn, Dictionary<string, string> simpleExpPairValue)
        {
            bool flag = false;
            try
            {
                //使用特殊字符串替换
                doc = new Document(templatepath);
                foreach (var key in simpleExpPairValue.Keys)
                {
                    var repStr = string.Format("{0}", key);
                    doc.Range.Replace(repStr, simpleExpPairValue[key], false, false);
                }


                bool isGenerate = false;

                // 表格有重复 工作经历
                if (dt != null && dt.Rows.Count > 0 && expPairColumn != null && expPairColumn.Count > 0)
                    isGenerate = GenerateTable(dt, expPairColumn, 0);

                // 表格有重复 专业技能
                if (skilldt != null && skilldt.Rows.Count > 0 && skillexpPairColumn != null && skillexpPairColumn.Count > 0)
                    isGenerate = GenerateTable(skilldt, skillexpPairColumn, dt.Rows.Count);

                // 表格有重复 服务领域
                if (serverdt != null && serverdt.Rows.Count > 0 && serverexpPairColumn != null && serverexpPairColumn.Count > 0)
                    isGenerate = GenerateTable(serverdt, serverexpPairColumn, dt.Rows.Count+skilldt.Rows.Count);

                // 表格有重复 成果展示
                if (achievementsdt != null && achievementsdt.Rows.Count > 0 && achievementsexpPairColumn != null && achievementsexpPairColumn.Count > 0)
                    isGenerate = GenerateTable(achievementsdt, achievementsexpPairColumn, dt.Rows.Count + skilldt.Rows.Count+serverdt.Rows.Count);

                // 表格有重复 项目经历
                if (casedt != null && casedt.Rows.Count > 0 && caseexpPairColumn != null && caseexpPairColumn.Count > 0)
                    isGenerate = GenerateTable(casedt, caseexpPairColumn, dt.Rows.Count + skilldt.Rows.Count + serverdt.Rows.Count+achievementsdt.Rows.Count);

                if (!isGenerate)
                {
                    flag = false;
                }
                else
                {
                    doc.Save(wordpath);//也可以保存为1.doc 兼容03-07
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 替换文件
        /// </summary>
        /// <param name="dt">要更新的数据</param>
        /// <param name="expPairColumn">当前要替换的数据字典</param>
        /// <returns></returns>
        private static bool GenerateTable(DataTable dt, Dictionary<string, string> expPairColumn, int index)
        {
            try
            {
                int tableNums = dt.Rows.Count;

                //获取所有表格
                NodeCollection allTables = doc.GetChildNodes(NodeType.Table, true);

                //获取第几个表格
                Table table = allTables[index] as Table;
                Table tableClone = (Table)table.Clone(true);

                for (int i = 0; i < tableNums; i++)
                {
                    if (i == 0)
                    {
                        foreach (string key in expPairColumn.Keys)
                        {
                            var value = dt.Rows[i][expPairColumn[key]].ToString();
                            var repStr = string.Format("{0}", key);
                            doc.Range.Replace(repStr, value.ToString(), false, false);
                        }
                        continue;
                    }

                    Table tableClone1 = (Table)tableClone.Clone(true);

                    table.ParentNode.InsertAfter(tableClone1, table);

                    foreach (string key in expPairColumn.Keys)
                    {
                        var value = dt.Rows[i][expPairColumn[key]].ToString();
                        var repStr = string.Format("{0}", key);
                        doc.Range.Replace(repStr, value, false, false);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
