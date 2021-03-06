using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class TextHide
    {
        /// <summary>
        /// 内容摘要：按字节截断字符串。
        /// </summary>
        public static string GetSubString(string mText, int startIndex, int byteCount)
        {
            if (string.IsNullOrEmpty(mText))
            {
                return "该用户太懒没有留下任何内容...";
            }

            if (byteCount < 1) return string.Empty;
            if (System.Text.Encoding.Default.GetByteCount(mText) <= byteCount)
            {
                return mText;
            }
            else
            {
                if (startIndex == 0)
                {
                    byte[] txtBytes = System.Text.Encoding.Default.GetBytes(mText);
                    byte[] newBytes = new byte[byteCount];
                    for (int i = 0; i < byteCount; i++)
                        newBytes[i] = txtBytes[i];
                    return System.Text.Encoding.Default.GetString(newBytes) + "...";
                }
                else
                {
                    string tmp = GetSubString(mText, 0, startIndex - 1);
                    mText = mText.Substring(tmp.Length);
                    return GetSubString(mText, 0, byteCount) + "...";
                }
            }
        }
        //public static string GetSubString(string mText, int startIndex)
        //{
        //    return GetSubString(mText, startIndex, System.Text.Encoding.Default.GetByteCount(mText) - startIndex + 1);
        //}

        public static string GetSubString(string mText, int byteCount)
        {
            return GetSubString(mText, 0, byteCount);
        }
    }
}
