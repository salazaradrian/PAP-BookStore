using System.Collections.Generic;

namespace PAP.Generators
{
    /// <summary>
    /// Single entry point for the Admin screen. Generators are listed in foreign-key order,
    /// so each table appears after the tables it depends on.
    /// </summary>
    public static class GeneratorCatalog
    {
        public static IList<string> EntityNames()
        {
            var names = new List<string>();
            foreach (IGenerator generator in CreateAll())
            {
                names.Add(generator.EntityName);
                generator.Dispose();
            }
            return names;
        }

        /// <summary>Creates a new generator by entity name, or null if the name is unknown.</summary>
        public static IGenerator Create(string entityName)
        {
            foreach (IGenerator generator in CreateAll())
            {
                if (generator.EntityName == entityName)
                    return generator;
                generator.Dispose();
            }
            return null;
        }

        private static IEnumerable<IGenerator> CreateAll()
        {
            yield return new RolesGenerator();
            yield return new CategoriesGenerator();
            yield return new GenresGenerator();
            yield return new PublishersGenerator();
            yield return new AuthorsGenerator();
            yield return new SuppliersGenerator();
            yield return new CustomersGenerator();
            yield return new EmployeesGenerator();
            yield return new AddressesGenerator();
            yield return new BooksGenerator();
            yield return new InventoriesGenerator();
            yield return new ShoppingCartsGenerator();
            yield return new CartItemsGenerator();
            yield return new OrdersGenerator();
            yield return new OrderItemsGenerator();
            yield return new PaymentsGenerator();
            yield return new PurchaseOrdersGenerator();
            yield return new PurchaseOrderItemsGenerator();
            yield return new ReviewsGenerator();
        }
    }
}