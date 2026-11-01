using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class PaymentsGenerator : GeneratorBase<Payment>
    {
        public PaymentsGenerator() : base(new PaymentRepository()) { }

        public override string EntityName { get { return "Payments"; } }

        protected override List<Payment> Build()
        {
            List<Order> orders = Load(new OrderRepository());
            RequireIds(orders.Select(x => x.Id), "Orders");

            string[] methods = { "CreditCard", "DebitCard", "PayPal", "BankTransfer", "Cash" };
            var list = new List<Payment>();
            foreach (Order order in orders)
            {
                bool cancelled = order.Status == "Cancelled";
                bool pending = order.Status == "Pending";
                list.Add(new Payment
                {
                    OrderId = order.Id,
                    PaymentMethod = SampleData.Pick(methods),
                    Amount = order.Total,
                    PaymentDate = order.OrderDate.AddMinutes(SampleData.Between(1, 120)),
                    Status = cancelled ? "Refunded" : pending ? "Pending" : "Completed",
                    TransactionRef = "TX-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant()
                });
            }
            return list;
        }
    }
}