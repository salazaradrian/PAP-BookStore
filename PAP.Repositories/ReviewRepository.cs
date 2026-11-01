using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class ReviewRepository : RepositoryBase<Review>
    {
        private IQueryable<Review> WithRelations()
        {
            return _set
                .Include(x => x.Book)
                .Include(x => x.Customer);
        }

        public override IEnumerable<Review> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Review GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
