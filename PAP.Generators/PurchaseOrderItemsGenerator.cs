using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class PurchaseOrderItemsGenerator : GeneratorBase<PurchaseOrderItem>
    {
        public PurchaseOrderItemsGenerator() : base(new PurchaseOrderItemRepository()) { }

        public override string EntityName { get { return "PurchaseOrderItems"; } }

        protected override List<PurchaseOrderItem> Build()
        {
            List<PurchaseOrder> purchases = Load(new PurchaseOrderRepository());
            List<Book> books = Load(new BookRepository());
            RequireIds(purchases.Select(x => x.Id), "PurchaseOrders");
            RequireIds(books.Select(x => x.Id), "Books");

            var list = new List<PurchaseOrderItem>();
            foreach (PurchaseOrder purchase in purchases)
            {
                foreach (Book book in books.OrderBy(x => SampleData.Rng.Next()).Take(SampleData.Between(2, 6)))
                {
                    list.Add(new PurchaseOrderItem
                    {
                        PurchaseOrderId = purchase.Id,
                        BookId = book.Id,
                        Quantity = SampleData.Between(10, 100),
                        UnitCost = Math.Round(book.Price * 0.6m, 2)
                    });
                }
            }
            return list;
        }

        protected override void Insert(List<PurchaseOrderItem> items)
        {
            base.Insert(items);

            // Keep each purchase order's total consistent with its items.
            using (var purchases = new PurchaseOrderRepository())
            {
                foreach (PurchaseOrder purchase in purchases.GetAll())
                    purchase.TotalCost = items.Where(x => x.PurchaseOrderId == purchase.Id)
                                              .Sum(x => x.Quantity * x.UnitCost);
                purchases.Save();
            }
        }
    }
}