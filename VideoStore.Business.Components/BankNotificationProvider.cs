using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStore.Business.Entities;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Services;

namespace VideoStore.Business.Components
{
    public class BankNotificationProvider : IBankNotificationProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public void NotifyBankCompletion(int pCustomerAccountNumber, Entities.BankStatus status)
        {
           string Emailadress = RetrieveUserEmailByAccountnumber(pCustomerAccountNumber);          
            if (status == Entities.BankStatus.Transfered)
            {          
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = Emailadress,
                    Message = "Our records show that your cardnumber: " + pCustomerAccountNumber + " has paid successfuly. Please wait for delivery!"
                });
            }
            if (status == Entities.BankStatus.Failed)
            {
                EmailProvider.SendMessage(new EmailMessage()
                {
                    ToAddress = Emailadress,
                    Message = "Our records show that there was a problem when are paying. Your CardNumber: " + pCustomerAccountNumber + ". Please contact Video Store"
                });
            }
        }
            
        private string RetrieveUserEmailByAccountnumber(int accountNumber)
        {
            using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
            {
                User lUser = lContainer.Users.Where((pDel) => pDel.BankAccountNumber == accountNumber).FirstOrDefault();
                return lUser.Email;
            }
                
        }
    }

}
