using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class MyPageBar
    {
        //会员中心
        public static string PageSortCenter(int pageIndex, int pageCount, string urlparams)
        {
            if (pageCount == 1)
            {
                return string.Empty;
            }
            int start = pageIndex - 5;
            start = start < 1 ? 1 : start;
            int end = start + 9;
            end = end > pageCount ? pageCount : end;
            StringBuilder sb = new StringBuilder();
            if (pageIndex > 1)
            {
                sb.Append(string.Format("<li><a href='" + urlparams + "{0}'><i class='fa fa-chevron-left'></i></a></li>", pageIndex - 1));
            }
            for (int i = start; i <= end; i++)
            {
                if (pageIndex == i)
                {
                    sb.Append(string.Format("<li class='active'><a href='#'>{0}</a></li>", i));
                }
                else
                {
                    sb.Append(string.Format("<li><a href='" + urlparams + "{0}'>{0}</a></li>", i));
                }
            }

            if (pageIndex < pageCount)
            {
                sb.Append(string.Format("<li><a href='" + urlparams + "{0}'><i class='fa fa-chevron-right'></i></a></li>", pageIndex + 1));
            }
            return sb.ToString();
        }
    }
}
