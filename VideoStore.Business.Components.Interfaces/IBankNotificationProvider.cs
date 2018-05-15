using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Entities;

namespace VideoStore.Business.Components.Interfaces
{
    //public enum BankStatus { Transfered, Failed }
    public interface IBankNotificationProvider
    {

        void NotifyBankCompletion(int pCustomerAccountNumber, BankStatus status);
    }
}
