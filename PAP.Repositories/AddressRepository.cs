using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class AddressRepository : RepositoryBase<Address>
    {
        private IQueryable<Address> WithRelations()
        {
            return _set
                .Include(x => x.Customer);
        }

        public override IEnumerable<Address> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Address GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
