using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class OrderItemsGenerator : GeneratorBase<OrderItem>
    {
        public OrderItemsGenerator() : base(new OrderItemRepository()) { }

        public override string EntityName { get { return "OrderItems"; } }

        protected override List<OrderItem> Build()
        {
            List<Order> orders = Load(new OrderRepository());
            List<Book> books = Load(new BookRepository());
            RequireIds(orders.Select(x => x.Id), "Orders");
            RequireIds(books.Select(x => x.Id), "Books");

            var list = new List<OrderItem>();
            foreach (Order order in orders)
            {
                foreach (Book book in books.OrderBy(x => SampleData.Rng.Next()).Take(SampleData.Between(1, 4)))
                {
                    list.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        BookId = book.Id,
                        Quantity = SampleData.Between(1, 3),
                        UnitPrice = book.Price,
                        Discount = SampleData.Chance(20) ? Math.Round(book.Price * 0.10m, 2) : 0m
                    });
                }
            }
            return list;
        }

        protected override void Insert(List<OrderItem> items)
        {
            base.Insert(items);

            // Keep the order totals consistent with the items just inserted.
            using (var orders = new OrderRepository())
            {
                foreach (Order order in orders.GetAll())
                {
                    decimal subtotal = items.Where(x => x.OrderId == order.Id)
                                            .Sum(x => x.Quantity * x.UnitPrice - x.Discount);
                    order.Subtotal = subtotal;
                    order.TaxAmount = Math.Round(subtotal * 0.13m, 2);
                    order.Total = order.Subtotal + order.TaxAmount + order.ShippingCost;
                }
                orders.Save();
            }
        }
    }
}