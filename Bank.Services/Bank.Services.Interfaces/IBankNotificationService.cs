using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Bank.Services.Interfaces
{  
        public enum BankInfoStatus { Transfered, Failed }

        [ServiceContract]
        public interface IBankNotificationService
        {
            [OperationContract]
            void NotifyBankCompletion(int pCustomerAccountNumber, BankInfoStatus status);
        }
}
