using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class InventoryRepository : RepositoryBase<Inventory>
    {
        private IQueryable<Inventory> WithRelations()
        {
            return _set
                .Include(x => x.Book);
        }

        public override IEnumerable<Inventory> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Inventory GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
