using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class InventoriesGenerator : GeneratorBase<Inventory>
    {
        public InventoriesGenerator() : base(new InventoryRepository()) { }

        public override string EntityName { get { return "Inventories"; } }

        protected override List<Inventory> Build()
        {
            List<int> bookIds = RequireIds(Load(new BookRepository()).Select(x => x.Id), "Books");
            return bookIds.Select(id => new Inventory
            {
                BookId = id,
                QuantityInStock = SampleData.Between(0, 200),
                ReorderLevel = SampleData.Between(5, 25),
                LastRestockedAt = SampleData.Recent(120)
            }).ToList();
        }
    }
}