using NetMVCBlogApp.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace NetMVCBlogApp.Models
{
    public class Mail
    {
        public bool SendNewPassword(Smtp smtp, string mail, string passwd)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(smtp.UserName);
            mailMessage.To.Add(mail);
            mailMessage.Subject = "New Password!";
            mailMessage.Body = string.Format("Your new password: {0} ",passwd);

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.UserName, smtp.Password);
            smtpClient.Host = smtp.ServerName;
            smtpClient.Port = smtp.Port;
            smtpClient.EnableSsl = smtp.Ssl;

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ContactMail(ContactMailModel contactMailModel, Smtp smtp)
        {
            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(smtp.UserName);
            mailMessage.To.Add(smtp.ReceiverName);
            mailMessage.Subject = string.Format("Hey! You have a new message. From: {0}", contactMailModel.Name);
            mailMessage.Body = string.Format("{0}\nfrom: {1}", contactMailModel.Text,contactMailModel.Mail);

            SmtpClient smtpClient = new SmtpClient();

            smtpClient.Credentials = new System.Net.NetworkCredential(smtp.UserName, smtp.Password);
            smtpClient.Host = smtp.ServerName;
            smtpClient.Port = smtp.Port;
            smtpClient.EnableSsl = smtp.Ssl;
            


            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}