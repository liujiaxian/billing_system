using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Jaq.Model.V20161123;

namespace Utility
{
    public class AliyunVcode
    {
        public static bool VcodeIsSuccess(string session,string sig,string token,string scene)
        {
            bool result = false;
            //YOUR ACCESS_KEY、YOUR ACCESS_SECRET请替换成您的阿里云accesskey id和secret 
            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "", "");
            IAcsClient client = new DefaultAcsClient(profile);

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "jaq", "jaq.aliyuncs.com");

            AfsCheckRequest request = new AfsCheckRequest();
            request.Platform = 3;//必填参数，请求来源： 1：Android端； 2：iOS端； 3：PC端及其他
            request.Session = session;// 必填参数，从前端获取，不可更改
            request.Sig = sig;// 必填参数，从前端获取，不可更改
            request.Token = token;// 必填参数，从前端获取，不可更改
            request.Scene = scene;// 必填参数，从前端获取，不可更改

            try
            {
                AfsCheckResponse response = client.GetAcsResponse(request);
                // TODO
                result = (bool)response.Data;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(e.ToString());
                //throw new Exception(ex.Message);
                result = false;
            }

            return result;
        }
    }
}
