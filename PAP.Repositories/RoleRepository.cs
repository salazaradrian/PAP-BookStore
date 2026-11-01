using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class RoleRepository : RepositoryBase<Role>
    {
        public override IEnumerable<Role> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Role GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
