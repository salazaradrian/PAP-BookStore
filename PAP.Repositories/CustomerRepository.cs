using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>
    {
        public override IEnumerable<Customer> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Customer GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
