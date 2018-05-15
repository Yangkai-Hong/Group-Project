using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Components.Interfaces;


namespace VideoStore.Business.Components
{
    public class EmailProvider : IEmailProvider
    {
        public void SendMessage(EmailMessage pMessage)
        {
            global::EmailService.MessageTypes.EmailMessage Message =
            new global::EmailService.MessageTypes.EmailMessage()
            {
                Message = pMessage.Message,
                ToAddresses = pMessage.ToAddress,
                Date = DateTime.Now
            };
           
            EmailTransferService.EmailServiceClient Client = new EmailTransferService.EmailServiceClient();
            Client.SendEmail(Message);

            /* global::EmailService.MessageTypes.EmailMessage Message = 
             new global::EmailService.MessageTypes.EmailMessage()
             {
                 Message = pMessage.Message,
                 ToAddresses = pMessage.ToAddress,
                 Date = DateTime.Now
             };
             //VideoStore.Business.Components.EmailService.
             EmailService.EmailServiceClient lClient = new EmailService.EmailServiceClient();
             lClient.SendEmail(Message);
             ExternalServiceFactory.Instance.EmailService.SendEmail
                 (
                     new global::EmailService.MessageTypes.EmailMessage()
                     {
                         Message = pMessage.Message,
                         ToAddresses = pMessage.ToAddress,
                         Date = DateTime.Now
                     }
                 );*/
        }
    }
}
