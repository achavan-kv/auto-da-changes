/* 
Author: suresh-IGT
Date: Feb 15th
Description:JM BlueStart
 */

using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Deliveries;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface IProcessDeliveryRepository
    {
        DeliveryNotificationModel GetDeliveryNotification(string accountId, string invoiceNumber, IApiRequest apiRequest);
    }
}
