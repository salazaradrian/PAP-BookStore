using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItem>
    {
        private IQueryable<OrderItem> WithRelations()
        {
            return _set
                .Include(x => x.Book)
                .Include(x => x.Order.Customer);
        }

        public override IEnumerable<OrderItem> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override OrderItem GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
