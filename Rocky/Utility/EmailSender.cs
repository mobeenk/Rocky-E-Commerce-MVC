using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources.SMS;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;


namespace Rocky.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public MailJetSettings _mailJetSettings { get; set; }

        public EmailSender(IConfiguration config )
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Excecute(email, subject, htmlMessage);
        }
        public async Task<MailjetResponse> Excecute(string email, string subject, string htmlMessage)
        {
            //it will look in this class we created MailJetSettings and populate all properties in it
             _mailJetSettings = _config.GetSection("MailJet").Get<MailJetSettings>();


             var client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey);
           // var client = new MailjetClient("", "_mailJetSettings.SecretKey");

            var request = new MailjetRequest { Resource = Mailjet.Client.Resources.Send.Resource }
                .Property(Mailjet.Client.Resources.Send.FromEmail, "moubien.kayali@protonmail.com")
                .Property(Mailjet.Client.Resources.Send.FromName, "Activation Mail")
                .Property(Mailjet.Client.Resources.Send.Subject, subject)
                .Property(Mailjet.Client.Resources.Send.HtmlPart, htmlMessage)
                .Property(Mailjet.Client.Resources.Send.Recipients, new JArray
                {
                    new JObject
                    {
                        { "Email", email }
                    }
                });

            return await client.PostAsync(request);
        }
    }
}
