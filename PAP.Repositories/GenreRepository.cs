using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class GenreRepository : RepositoryBase<Genre>
    {
        public override IEnumerable<Genre> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Genre GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
