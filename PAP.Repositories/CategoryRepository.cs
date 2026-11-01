using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>
    {
        public override IEnumerable<Category> GetAll()
        {
            return _set.OrderBy(x => x.Id).ToList();
        }

        public override Category GetById(int id)
        {
            return _set.FirstOrDefault(x => x.Id == id);
        }
    }
}
