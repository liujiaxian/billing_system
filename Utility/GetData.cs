using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class GetData
    {
        public static string GetIPAddress()
        {
            string ip = "";
            try
            {

                WebClient MyWebClient = new WebClient();


                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据

                Byte[] pageData = MyWebClient.DownloadData("http://www.net.cn/static/customercare/yourip.asp"); //从指定网站下载数据

                string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            

                //string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句

                string[] str = HtmlHelper.GetElementsByTagName(pageHtml, "h2");
                string[] str1 = str[0].Replace("<h2>", "").Split(',');
            
                ip = str1[0];
            }

            catch (WebException webEx)
            {
                //webEx.Message.ToString()
            }
            return ip;
        }
    }
}
