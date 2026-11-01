using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class OrderRepository : RepositoryBase<Order>
    {
        private IQueryable<Order> WithRelations()
        {
            return _set
                .Include(x => x.Customer)
                .Include(x => x.Address);
        }

        public override IEnumerable<Order> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Order GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
