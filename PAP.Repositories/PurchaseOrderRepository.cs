using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>
    {
        private IQueryable<PurchaseOrder> WithRelations()
        {
            return _set
                .Include(x => x.Supplier);
        }

        public override IEnumerable<PurchaseOrder> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override PurchaseOrder GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
