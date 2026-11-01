
using PAP.DataAccess;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Repositories
{
    public partial class BookRepository : RepositoryBase<Book>
    {
        public IEnumerable<Book> MyBooks()
        {
            // queryable
            var test = from x in _context.Books
                       where !x.IsInStock && x.Name.Contains("a")
                       select x;

            return test.ToList();
        }
    }
}
