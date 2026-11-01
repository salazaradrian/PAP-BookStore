using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class ShoppingCartRepository : RepositoryBase<ShoppingCart>
    {
        private IQueryable<ShoppingCart> WithRelations()
        {
            return _set
                .Include(x => x.Customer)
                .Include(x => x.CartItems);
        }

        public override IEnumerable<ShoppingCart> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override ShoppingCart GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
