using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace WebApp.Controllers
{
    public class UploadController : BaseController
    {
        #region 上传图片
        public ActionResult UploadImage()
        {
            string imgurl = "";
            int width = 0, height = 0;
            foreach (string key in Request.Files)
            {
                //这里只测试上传第一张图片file[0]
                HttpPostedFileBase file0 = Request.Files[key];

                //转换成byte,读取图片MIME类型
                Stream stream;
                int size = file0.ContentLength / 1024; //文件大小KB

                if (size > 5120)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "图片不能超过5M：", null));
                }

                byte[] fileByte = new byte[2];//contentLength，这里我们只读取文件长度的前两位用于判断就好了，这样速度比较快，剩下的也用不到。
                stream = file0.InputStream;
                stream.Read(fileByte, 0, 2);//contentLength，还是取前两位

                string fileFlag = "";
                if (fileByte != null && fileByte.Length > 0)//图片数据是否为空
                {
                    fileFlag = fileByte[0].ToString() + fileByte[1].ToString();
                }

                string exName = System.IO.Path.GetExtension(file0.FileName); //得到扩展名

                string[] fileTypeStr = { "255216", "7173", "6677", "13780" };//对应的图片格式jpg,gif,bmp,png
                if (fileTypeStr.Contains(fileFlag))
                {
                    //获取图片宽和高
                    System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                    width = image.Width;
                    height = image.Height;

                    string path = "/uploads/images/";
                    if (!Directory.Exists(Server.MapPath(path)))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }

                    string NewName = DateTime.Now.ToString("yyyyMMddhhmmss") + exName;//重命名上传文件

                    Request.Files[key].SaveAs(Server.MapPath(path + NewName));
                    imgurl = path + NewName;
                }
                else
                {
                    return Content(ReturnMsg(Enum_Return.失败, "图片格式不正确：" + fileFlag, null));
                }

                stream.Close();
            }

            return Content(ReturnMsg(Enum_Return.成功, "上传成功", imgurl));
        }
        #endregion

        #region 上传文件
        public ActionResult UploadFile()
        {
            string fileurl = "";
            foreach (string key in Request.Files)
            {
                //这里只测试上传第一张图片file[0]
                HttpPostedFileBase file0 = Request.Files[key];

                //转换成byte,读取图片MIME类型
                int size = file0.ContentLength / 1024; //文件大小KB

                if (size > 512000)
                {
                    return Content(ReturnMsg(Enum_Return.失败, "文件不能超过500M：", null));
                }

                string exName = System.IO.Path.GetExtension(file0.FileName); //得到扩展名

                string path = "/uploads/website/file/"+DateTime.Now.ToString("yyyyMMdd")+"/";
                if (!Directory.Exists(Server.MapPath(path)))
                {
                    Directory.CreateDirectory(Server.MapPath(path));
                }

                string NewName = Guid.NewGuid().ToString().Replace("-","") + exName;//重命名上传文件

                Request.Files[key].SaveAs(Server.MapPath(path + NewName));
                fileurl = path + NewName;
            }

            return Content(ReturnMsg(Enum_Return.成功, "上传成功", fileurl));
        }
        #endregion
    }
}
