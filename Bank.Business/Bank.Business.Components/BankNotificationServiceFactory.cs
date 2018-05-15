using Bank.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Bank.Business.Components
{

        public class BankNotificationServiceFactory
        {
            public static IBankNotificationService GetBankNotificationService(String pAddress)
            {
                ChannelFactory<IBankNotificationService> lChannelFactory = new ChannelFactory<IBankNotificationService>(new NetTcpBinding(), new EndpointAddress(pAddress));
                return lChannelFactory.CreateChannel();
            }
        }
    
}
