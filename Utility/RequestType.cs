using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class RequestType
    {
        #region 发出get请求
        public static string HttpGet(string urljson)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urljson);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion

        #region 发出post请求
        public static string HttpPost(string Url, string postDataStr)
        {
            System.Net.WebClient webc = new System.Net.WebClient();
            var apiurl = new Uri(Url);
            string sendstr = postDataStr;
            webc.Headers.Add("Content-Type", "text/xml");
            //webc.Headers["Content-Type"] = "application/stream;charset=utf-8";//OK  
            var arr = webc.UploadData(apiurl, Encoding.UTF8.GetBytes(sendstr));
            return Encoding.UTF8.GetString(arr);
        }
        #endregion
    }
}
