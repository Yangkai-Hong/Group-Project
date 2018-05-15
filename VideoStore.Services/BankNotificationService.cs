using Bank.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Business.Entities;

namespace VideoStore.Services
{
    public class BankNotificationService : IBankNotificationService
    {
        IBankNotificationProvider Provider
        {
            get { return ServiceLocator.Current.GetInstance<IBankNotificationProvider>(); }
        }

        public void NotifyBankCompletion(int pCustomerAccountNumber, BankInfoStatus status)
        {
            Provider.NotifyBankCompletion(pCustomerAccountNumber, GetBankStatusFromBankInfoStatus(status));
        }

        private BankStatus GetBankStatusFromBankInfoStatus(BankInfoStatus status)
        {
            Status.bankInfoStatus = BankStatus.Transfered;
            if (status == BankInfoStatus.Transfered)
            {
                Status.bankInfoStatus = BankStatus.Transfered;
                return BankStatus.Transfered;
            }
            else if (status == BankInfoStatus.Failed)
            {
                Status.bankInfoStatus = BankStatus.Failed;
                return BankStatus.Failed;
            }         
            else
            {
                throw new Exception("Unexpected delivery status received");
            }
        }

    }
}
