using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoStore.Business.Components.Interfaces;
using VideoStore.Business.Entities;
using System.Transactions;
using Microsoft.Practices.ServiceLocation;
using DeliveryCo.MessageTypes;
using VideoStore.Services;
using System.Threading;

namespace VideoStore.Business.Components
{
    public class OrderProvider : IOrderProvider
    {
        public IEmailProvider EmailProvider
        {
            get { return ServiceLocator.Current.GetInstance<IEmailProvider>(); }
        }

        public IUserProvider UserProvider
        {
            get { return ServiceLocator.Current.GetInstance<IUserProvider>(); }
        }

        public void SubmitOrder(Entities.Order pOrder)
        {

            using (TransactionScope lScope = new TransactionScope())
            {
                LoadMediaStocks(pOrder);
                MarkAppropriateUnchangedAssociations(pOrder);
                using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
                {
                    try
                    {
                        Status.bankInfoStatus = BankStatus.Failed;
                        pOrder.OrderNumber = Guid.NewGuid();

                        Thread thread1 = new Thread(delegate ()
                        {
                            TransferFundsFromCustomer(UserProvider.ReadUserById(pOrder.Customer.Id).BankAccountNumber, pOrder.Total ?? 0.0);
                        });
                        thread1.Start();
                        Thread.Sleep(3500);

                        if (Status.bankInfoStatus == BankStatus.Failed)
                            throw new Exception("Insufficient balance");
                        pOrder.UpdateStockLevels();

                        PlaceDeliveryForOrder(pOrder);
                        SendOrderPlacedConfirmation(pOrder);
                        lContainer.Orders.ApplyChanges(pOrder);
                        lContainer.SaveChanges();
                        lScope.Complete();

                    }
                    catch (Exception lException)
                    {
                        SendOrderErrorMessage(pOrder, lException);
                        throw;
                    }
                }
            }    
        }

        

        private void MarkAppropriateUnchangedAssociations(Order pOrder)
        {
            pOrder.Customer.MarkAsUnchanged();
            pOrder.Customer.LoginCredential.MarkAsUnchanged();
            foreach (OrderItem lOrder in pOrder.OrderItems)
            {
                lOrder.Media.Stocks.MarkAsUnchanged();
                lOrder.Media.MarkAsUnchanged();
            }
        }

        private void LoadMediaStocks(Order pOrder)
        {
            using (VideoStoreEntityModelContainer lContainer = new VideoStoreEntityModelContainer())
            {
                foreach (OrderItem lOrder in pOrder.OrderItems)
                {
                    lOrder.Media.Stocks = lContainer.Stocks.Where((pStock) => pStock.Media.Id == lOrder.Media.Id).FirstOrDefault();    
                }
            }
        }

        

        private void SendOrderErrorMessage(Order pOrder, Exception pException)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "There was an error in processsing your order " + pOrder.OrderNumber + ": "+ pException.Message +". Please contact Video Store"
            });
        }

        private void SendOrderPlacedConfirmation(Order pOrder)
        {
            EmailProvider.SendMessage(new EmailMessage()
            {
                ToAddress = pOrder.Customer.Email,
                Message = "Your order " + pOrder.OrderNumber + " has been placed"
            });
        }

        private void PlaceDeliveryForOrder(Order pOrder)
        {
            Guid identifier = Guid.NewGuid();
            Delivery lDelivery = new Delivery()
            {
                ExternalDeliveryIdentifier = identifier,
                DeliveryStatus = DeliveryStatus.Submitted,
                SourceAddress = "Video Store Address",
                DestinationAddress = pOrder.Customer.Address,
                Order = pOrder
            };

            DeliveryService.DeliveryServiceClient lClient = new DeliveryService.DeliveryServiceClient();
            lClient.SubmitDelivery(new DeliveryInfo()
            {
                OrderNumber = lDelivery.Order.OrderNumber.ToString(),
                SourceAddress = lDelivery.SourceAddress,
                DestinationAddress = lDelivery.DestinationAddress,
                DeliveryNotificationAddress = "net.tcp://localhost:9010/DeliveryNotificationService"
            });
            lDelivery.ExternalDeliveryIdentifier = identifier;
            pOrder.Delivery = lDelivery;
            
        }

        private void TransferFundsFromCustomer(int pCustomerAccountNumber, double pTotal)
        {
            try
            {
                //ExternalServiceFactory.Instance.TransferService.Transfer(pTotal, pCustomerAccountNumber, RetrieveVideoStoreAccountNumber());
                TransferService.TransferServiceClient lClient = new TransferService.TransferServiceClient();
                lClient.Transfer(pTotal, pCustomerAccountNumber, RetrieveVideoStoreAccountNumber(),"net.tcp://localhost:9010/BankNotificationService");
           
            }
            catch(Exception e)
            {
                // throw new Exception(e.Message);
            }
        }

        

        private int RetrieveVideoStoreAccountNumber()
        {
            return 123;
        }


    }
}
