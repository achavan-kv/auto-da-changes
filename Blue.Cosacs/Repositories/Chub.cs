using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Blue.Cosacs.Messages.Service;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Messages.Warranty;
using Blue.Cosacs.Messages.Merchandising.Cints;
using Blue.Hub.Client;

namespace Blue.Cosacs.Repositories
{
    using Blue.Cosacs.Messages.Merchandising.Cints;
using Blue.Cosacs.Messages.CustomerPhoneNumbers;

    public class Chub
    {
        private static readonly IPublisher publisher = new Hub.Client.SqlPublisher(STL.DAL.Connections.DefaultName, new DateTimeClock());       // MV fix

        public void SubmitMany(IEnumerable<BookingSubmit> bookings, SqlConnection c, SqlTransaction t)
        {
            bookings.ToList().ForEach(b => Submit(b, c, t));
        }

        private void Submit(BookingSubmit booking, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warehouse.Booking.Submit", booking, booking.Reference, c, t);
        }

        private void Cancel(Message<BookingCancel> message, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warehouse.Booking.Cancel", message.Payload, message.CorrelationId, c, t);
        }

        public void CancelMany(IEnumerable<Message<BookingCancel>> messages, SqlConnection c, SqlTransaction t)
        {
            messages.ToList().ForEach(m => Cancel(m, c, t));
        }

        public void SubmitMany(IEnumerable<RequestSubmit> installations, SqlConnection c, SqlTransaction t)
        {
            installations.ToList().ForEach(b => Submit(b, c, t));
        }

        private void Submit(RequestSubmit installation, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Service.Request.Submit", installation, installation.Account, c, t);
        }

        public void Submit(SalesOrder warrantySale, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warranty.Sale.Submit", warrantySale, null, c, t);
        }

        public void Cancel(SalesOrderCancel warrantySaleCancel, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warranty.Sale.Cancel", warrantySaleCancel, null, c, t);
        }

        public void Cancel(SalesOrderCancelItem warrantySaleCancelItem, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warranty.Sale.CancelItem", warrantySaleCancelItem, null, c, t);
        }

        public void Submit(PotentialSales potentialSale, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Warranty.Potential.Sales", potentialSale, null, c, t);
        }

        public void Submit(CintSubmit cint, SqlConnection c, SqlTransaction t)
        {
            publisher.Publish("Merchandising.Cints", cint, cint.RunNo.ToString(), c, t);
        }

        public static void SubmitSmsUnsubscriptions(CustomerPhoneNumbers message)
        {
            publisher.Publish("Cosacs.Communication.SmsUnsubscriptions", message);
        }

    }
}
