using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class PublisherRepository : RepositoryBase<Publisher>
    {
        public override IEnumerable<Publisher> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Publisher GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
