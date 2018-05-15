using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace DeliveryCo.Business.Components.Interfaces
{
    public interface IDeliveryProvider
    {        
        void SubmitDelivery(DeliveryCo.Business.Entities.DeliveryInfo pDeliveryInfo);
    }
}
