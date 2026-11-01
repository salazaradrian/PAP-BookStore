using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class ShoppingCartsGenerator : GeneratorBase<ShoppingCart>
    {
        public ShoppingCartsGenerator() : base(new ShoppingCartRepository()) { }

        public override string EntityName { get { return "ShoppingCarts"; } }

        protected override List<ShoppingCart> Build()
        {
            List<int> customerIds = RequireIds(Load(new CustomerRepository()).Select(x => x.Id), "Customers");
            // Roughly 60% of the customers have a cart.
            return customerIds.Where(x => SampleData.Chance(60)).Select(id => new ShoppingCart
            {
                CustomerId = id,
                CreatedAt = SampleData.Recent(30),
                UpdatedAt = SampleData.Recent(5)
            }).ToList();
        }
    }
}