using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class PaymentRepository : RepositoryBase<Payment>
    {
        private IQueryable<Payment> WithRelations()
        {
            return _set
                .Include(x => x.Order.Customer);
        }

        public override IEnumerable<Payment> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Payment GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
