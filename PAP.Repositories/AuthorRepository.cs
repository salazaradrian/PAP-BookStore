using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class AuthorRepository : RepositoryBase<Author>
    {
        private IQueryable<Author> WithRelations()
        {
            return _set
                .Include(x => x.Books);
        }

        public override IEnumerable<Author> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Author GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
