using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class PurchaseOrdersGenerator : GeneratorBase<PurchaseOrder>
    {
        public PurchaseOrdersGenerator() : base(new PurchaseOrderRepository()) { }

        public override string EntityName { get { return "PurchaseOrders"; } }

        protected override List<PurchaseOrder> Build()
        {
            List<int> supplierIds = RequireIds(Load(new SupplierRepository()).Select(x => x.Id), "Suppliers");
            string[] statuses = { "Pending", "Received", "Received", "Cancelled" };

            var list = new List<PurchaseOrder>();
            foreach (int supplierId in supplierIds)
            {
                int count = SampleData.Between(1, 3);
                for (int i = 0; i < count; i++)
                {
                    var date = SampleData.Recent(240);
                    // TotalCost is recalculated by PurchaseOrderItemsGenerator.
                    list.Add(new PurchaseOrder
                    {
                        SupplierId = supplierId,
                        OrderDate = date,
                        ExpectedDate = date.AddDays(SampleData.Between(7, 30)),
                        Status = SampleData.Pick(statuses),
                        TotalCost = 0
                    });
                }
            }
            return list;
        }
    }
}