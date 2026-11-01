using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    /// <summary>
    /// Single entry point for the Admin screen. Generators are listed in foreign-key order,
    /// so each table appears after the tables it depends on.
    /// </summary>
    public static class GeneratorCatalog
    {
        // Parent tables that must already contain data before an entity can be generated.
        private static readonly Dictionary<string, string[]> Dependencies = new Dictionary<string, string[]>
        {
            { "Roles", new string[0] },
            { "Categories", new string[0] },
            { "Genres", new string[0] },
            { "Publishers", new string[0] },
            { "Authors", new string[0] },
            { "Suppliers", new string[0] },
            { "Customers", new string[0] },
            { "Employees", new[] { "Roles" } },
            { "Addresses", new[] { "Customers" } },
            { "Books", new[] { "Authors", "Categories", "Genres", "Publishers" } },
            { "Inventories", new[] { "Books" } },
            { "ShoppingCarts", new[] { "Customers" } },
            { "CartItems", new[] { "ShoppingCarts", "Books" } },
            { "Orders", new[] { "Customers", "Addresses" } },
            { "OrderItems", new[] { "Orders", "Books" } },
            { "Payments", new[] { "Orders", "OrderItems" } },
            { "PurchaseOrders", new[] { "Suppliers" } },
            { "PurchaseOrderItems", new[] { "PurchaseOrders", "Books" } },
            { "Reviews", new[] { "Customers", "Books" } }
        };

        /// <summary>
        /// One status per entity, in generation order. A button is enabled when every parent table has data.
        /// </summary>
        public static IList<GeneratorStatus> GetStatuses()
        {
            var hasData = new Dictionary<string, bool>();
            var order = new List<string>();
            foreach (IGenerator generator in CreateAll())
            {
                using (generator)
                {
                    order.Add(generator.EntityName);
                    hasData[generator.EntityName] = generator.HasData();
                }
            }

            return order.Select(name =>
            {
                List<string> missing = Dependencies[name].Where(parent => !hasData[parent]).ToList();
                return new GeneratorStatus
                {
                    EntityName = name,
                    TableName = "dbo." + (name == "Inventories" ? "Inventory" : name),
                    HasData = hasData[name],
                    CanGenerate = missing.Count == 0,
                    MissingDependencies = missing
                };
            }).ToList();
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