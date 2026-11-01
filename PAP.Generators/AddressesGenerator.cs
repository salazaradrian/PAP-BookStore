using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class AddressesGenerator : GeneratorBase<Address>
    {
        public AddressesGenerator() : base(new AddressRepository()) { }

        public override string EntityName { get { return "Addresses"; } }

        protected override List<Address> Build()
        {
            List<int> customerIds = RequireIds(Load(new CustomerRepository()).Select(x => x.Id), "Customers");
            var list = new List<Address>();
            foreach (int customerId in customerIds)
            {
                // Every customer gets a default shipping address and, sometimes, a billing one.
                list.Add(NewAddress(customerId, "Shipping", true));
                if (SampleData.Chance(40))
                    list.Add(NewAddress(customerId, "Billing", false));
            }
            return list;
        }

        private static Address NewAddress(int customerId, string type, bool isDefault)
        {
            return new Address
            {
                CustomerId = customerId,
                AddressType = type,
                Street = "Calle " + SampleData.Between(1, 40) + ", casa " + SampleData.Between(1, 200),
                City = SampleData.Pick(SampleData.Cities),
                State = "Provincia " + SampleData.Between(1, 7),
                PostalCode = SampleData.Between(10000, 99999).ToString(),
                Country = SampleData.Pick(SampleData.Countries),
                IsDefault = isDefault
            };
        }
    }
}