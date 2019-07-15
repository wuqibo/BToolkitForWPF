using System;
using System.Net;
using System.Net.Mail;

namespace BToolkitForWPF
{
    class Email
    {
        public static void SendEmail()
        {
            try
            {
                MailMessage mailmessage = new MailMessage("wqb2001@163.com", "wqb55@qq.com", "《图影》服务器器异常", "《图影》服务器器异常，请立即排查！");
                mailmessage.Priority = MailPriority.High; //邮件优先级
                SmtpClient smtpClient = new SmtpClient("smtp.163.com", 25); //smtp地址以及端口号
                smtpClient.Credentials = new NetworkCredential("wqb2001", "xxxxxx");//smtp用户名密码
                smtpClient.EnableSsl = true; //启用ssl
                smtpClient.Send(mailmessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("===============" + ex);
            }
        }
    }
}
