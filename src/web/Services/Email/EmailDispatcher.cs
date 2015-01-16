﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Services.Email.Models;

namespace OxxCommerceStarterKit.Web.Services.Email
{
    public class EmailDispatcher : IEmailDispatcher
    {
        public SendEmailResponse SendEmail(Postal.Email email)
        {
            var log = LogManager.GetLogger();
            return SendEmail(email, log);
        }

        public SendEmailResponse SendEmail(Postal.Email email, ILogger log)
        {
            var output = new SendEmailResponse();

#if !DEBUG
			try
			{
#endif
            // Process email with Postal
            var emailService = ServiceLocator.Current.GetInstance<Postal.IEmailService>();
            using (var message = emailService.CreateMailMessage(email))
            {
                var htmlView = message.AlternateViews.FirstOrDefault(x => x.ContentType.MediaType.ToLower() == "text/html");
                if (htmlView != null)
                {
                    string body = new StreamReader(htmlView.ContentStream).ReadToEnd();

                    // move ink styles inline with PreMailer.Net
                    var result = PreMailer.Net.PreMailer.MoveCssInline(body, false, "#ignore");

                    htmlView.ContentStream.SetLength(0);
                    var streamWriter = new StreamWriter(htmlView.ContentStream);

                    streamWriter.Write(result.Html);
                    streamWriter.Flush();

                    htmlView.ContentStream.Position = 0;
                }

                // send email with default smtp client. (the same way as Postal)
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Send(message);
                }
            }
            output.Success = true;

#if !DEBUG
            }


            catch (Exception ex)
            {
                log.Error(ex.Message);
                output.Exception = ex;
            }

#endif
            return output;
        }
    }

}