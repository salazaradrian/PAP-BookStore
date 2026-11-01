using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class PurchaseOrderItemRepository : RepositoryBase<PurchaseOrderItem>
    {
        private IQueryable<PurchaseOrderItem> WithRelations()
        {
            return _set
                .Include(x => x.Book)
                .Include(x => x.PurchaseOrder.Supplier);
        }

        public override IEnumerable<PurchaseOrderItem> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override PurchaseOrderItem GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
