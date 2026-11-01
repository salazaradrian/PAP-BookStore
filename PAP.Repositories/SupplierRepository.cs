using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class SupplierRepository : RepositoryBase<Supplier>
    {
        public override IEnumerable<Supplier> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Supplier GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
