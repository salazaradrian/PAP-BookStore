using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class CartItemRepository : RepositoryBase<CartItem>
    {
        private IQueryable<CartItem> WithRelations()
        {
            return _set
                .Include(x => x.Book)
                .Include(x => x.ShoppingCart.Customer);
        }

        public override IEnumerable<CartItem> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override CartItem GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
