using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{

    public enum Enum_Return
    {
        成功 = 0,
        失败,
        登录失效,
        账号未激活
    }

    public enum Enum_User
    {
        超级管理员 = 1,
        管理员,
        游客
    }

    public enum Enum_Member_Status
    {
        申请中 = 1,
        正常,
        黑名单
    }

    public enum Enum_Type
    {
        支出 = 1,
        收入
    }

    public enum Enum_Notice_Type
    {
        公告 = 1,
        疑问,
        回复
    }

    public enum Enum_Inviter_Type
    {

        邀请中 = 1,
        邀请成功,
        邀请失败
    }

    public enum Enum_Inviter_Status
    {
        正常 = 1,
        邀请中,
        邀请失败
    }
}
