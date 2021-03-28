// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MailHelper.cs" company="Wedn.Net">
//     Copyright © 2014 Wedn.Net. All Rights Reserved.
// </copyright>
// <summary>
//   邮件发送助手类  0.10
//   Verion:0.10
//   Description:通过SMTP发送邮件
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

/// <summary>
/// 邮件发送助手类
/// </summary>
/// <remarks>
///  2013-11-18 18:56 Created By iceStone
/// </remarks>
public static class MailHelper
{
    private readonly static string SmtpServer = "smtp.163.com";
    private readonly static int SmtpServerPort = 25;
    private readonly static bool SmtpEnableSsl = true;
    private readonly static string SmtpUsername = "";
    private readonly static string SmtpDisplayName = "记账系统";
    private readonly static string SmtpPassword = "";

    /// <summary>
    /// 发送邮件到指定收件人
    /// </summary>
    /// <remarks>
    ///  2013-11-18 18:55 Created By iceStone
    /// </remarks>
    /// <param name="to">收件人地址</param>
    /// <param name="subject">主题</param>
    /// <param name="mailBody">正文内容(支持HTML)</param>
    /// <param name="copyTos">抄送地址列表</param>
    /// <returns>是否发送成功</returns>
    public static bool Send(string to, string subject, string mailBody, params string[] copyTos)
    {
        return Send(new[] { to }, subject, mailBody, copyTos, new string[] { }, MailPriority.Normal);
    }

    /// <summary>
    /// 发送邮件到指定收件人
    /// </summary>
    /// <remarks>
    ///  2013-11-18 18:55 Created By iceStone
    /// </remarks>
    /// <param name="tos">收件人地址列表</param>
    /// <param name="subject">主题</param>
    /// <param name="mailBody">正文内容(支持HTML)</param>
    /// <param name="ccs">抄送地址列表</param>
    /// <param name="bccs">密件抄送地址列表</param>
    /// <param name="priority">此邮件的优先级</param>
    /// <param name="attachments">附件列表</param>
    /// <returns>是否发送成功</returns>
    /// <exception cref="System.ArgumentNullException">attachments</exception>
    public static bool Send(string[] tos, string subject, string mailBody, string[] ccs, string[] bccs, MailPriority priority, params Attachment[] attachments)
    {
        if (attachments == null) throw new ArgumentNullException("attachments");
        if (tos.Length == 0) return false;
        //创建Email实体
        var message = new MailMessage();
        message.From = new MailAddress(SmtpUsername, SmtpDisplayName);
        message.Subject = subject;
        message.Body = mailBody;
        message.BodyEncoding = Encoding.UTF8;
        message.IsBodyHtml = true;
        message.Priority = priority;
        //插入附件
        foreach (var attachment in attachments)
        {
            message.Attachments.Add(attachment);
        }
        //插入收件人地址,抄送地址和密件抄送地址
        foreach (var to in tos.Where(c => !string.IsNullOrEmpty(c)))
        {
            message.To.Add(new MailAddress(to));
        }
        foreach (var cc in ccs.Where(c => !string.IsNullOrEmpty(c)))
        {
            message.CC.Add(new MailAddress(cc));
        }
        foreach (var bcc in bccs.Where(c => !string.IsNullOrEmpty(c)))
        {
            message.CC.Add(new MailAddress(bcc));
        }
        //创建SMTP客户端
        var client = new SmtpClient
        {
            Host = SmtpServer,
            Credentials = new System.Net.NetworkCredential(SmtpUsername, SmtpPassword),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = SmtpEnableSsl,
            Port = SmtpServerPort
        };
        //client.SendCompleted += Client_SendCompleted;
        try
        {
            //发送邮件
            client.Send(message);
            //client.SendAsync(message, DateTime.Now.ToString());

            //client.Dispose();
            //message.Dispose();
            return true;
        }
        catch (Exception ex)
        {          
            return false;
        }
    }


    #region 忘记密码生成发送邮件的模板静态页内容发给用户邮箱
    /// <summary>
    /// 生成邮件内容页面
    /// </summary>
    /// <param name="id"></param>
    public static string CreateHtmlPage(string email, string url)
    {
        string html = HttpContext.Current.Request.MapPath("/Templates/sendemail.html");
        html = File.ReadAllText(html);
        html = html.Replace("$Email", email).Replace("$Url", url).Replace("$Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return html;
    }
    #endregion
}