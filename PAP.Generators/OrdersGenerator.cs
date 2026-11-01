using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class OrdersGenerator : GeneratorBase<Order>
    {
        public OrdersGenerator() : base(new OrderRepository()) { }

        public override string EntityName { get { return "Orders"; } }

        protected override List<Order> Build()
        {
            List<Customer> customers = Load(new CustomerRepository());
            List<Address> addresses = Load(new AddressRepository());
            RequireIds(customers.Select(x => x.Id), "Customers");
            RequireIds(addresses.Select(x => x.Id), "Addresses");

            string[] statuses = { "Pending", "Processing", "Shipped", "Delivered", "Delivered", "Cancelled" };
            var list = new List<Order>();
            foreach (Customer customer in customers)
            {
                // The shipping address must belong to the same customer.
                List<Address> own = addresses.Where(a => a.CustomerId == customer.Id && a.AddressType == "Shipping").ToList();
                if (own.Count == 0)
                    continue;

                int count = SampleData.Between(0, 3);
                for (int i = 0; i < count; i++)
                {
                    var date = SampleData.Recent(180);
                    decimal shipping = SampleData.Money(2, 10);
                    // Subtotal, tax and total are recalculated by OrderItemsGenerator once the items exist.
                    list.Add(new Order
                    {
                        CustomerId = customer.Id,
                        ShippingAddressId = SampleData.Pick(own).Id,
                        OrderDate = date,
                        Status = SampleData.Pick(statuses),
                        Subtotal = 0,
                        TaxAmount = 0,
                        ShippingCost = shipping,
                        Total = shipping,
                        Notes = "Pedido generado automaticamente",
                        UpdatedAt = date
                    });
                }
            }
            return list;
        }
    }
}