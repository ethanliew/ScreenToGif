﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ScreenToGif.FileWriters;

namespace ScreenToGif.Util
{
    internal static class FeedbackHelper
    {
        internal static async Task<bool> Send(string html, List<string> files = null)
        {
            //Please, don't try to log with this e-mail and password. :/
            //Everytime someone does this, I have to change the password and the Feedback feature stops working until I update the app.
            var passList = (Secret.Password ?? "").Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var t in passList)
            {
                if (await SendInternal(t, html, files ?? new List<string>()))
                    return true;
            }

            return false;
        }

        private static async Task<bool> SendInternal(string pass, string html, List<string> files)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Timeout = 5 * 60 * 1000, //Minutes, seconds, miliseconds
                    Port = Secret.Port,
                    Host = Secret.Host,
                    EnableSsl = true,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(Secret.Email, pass)
                };

                //Please, don't try to log with this e-mail and password. :/
                //Everytime someone does this, I have to change the password and the Feedback feature stops working until I update the app.
                var mail = new MailMessage
                {
                    From = new MailAddress("screentogif@outlook.com"),
                    Subject = "ScreenToGif - Feedback",
                    IsBodyHtml = true
                };
                mail.To.Add("nicke@outlook.com.br");
                mail.Body = html;

                foreach (var file in files)
                    mail.Attachments.Add(new Attachment(file));

                smtp.SendCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                        throw args.Error;
                };

                await smtp.SendMailAsync(mail);

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex, "Error while sending email");
                return false;
            }
        }
    }
}