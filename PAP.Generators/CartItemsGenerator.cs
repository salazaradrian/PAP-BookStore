using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class CartItemsGenerator : GeneratorBase<CartItem>
    {
        public CartItemsGenerator() : base(new CartItemRepository()) { }

        public override string EntityName { get { return "CartItems"; } }

        protected override List<CartItem> Build()
        {
            List<ShoppingCart> carts = Load(new ShoppingCartRepository());
            List<int> bookIds = RequireIds(Load(new BookRepository()).Select(x => x.Id), "Books");
            RequireIds(carts.Select(x => x.Id), "ShoppingCarts");

            var list = new List<CartItem>();
            foreach (ShoppingCart cart in carts)
            {
                // Distinct books per cart, because (CartId, BookId) is unique.
                foreach (int bookId in bookIds.OrderBy(x => SampleData.Rng.Next()).Take(SampleData.Between(1, 4)))
                {
                    list.Add(new CartItem
                    {
                        CartId = cart.Id,
                        BookId = bookId,
                        Quantity = SampleData.Between(1, 3),
                        AddedAt = SampleData.Recent(30)
                    });
                }
            }
            return list;
        }
    }
}