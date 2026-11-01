using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class CustomersGenerator : GeneratorBase<Customer>
    {
        public CustomersGenerator() : base(new CustomerRepository()) { }

        public override string EntityName { get { return "Customers"; } }

        protected override List<Customer> Build()
        {
            var list = new List<Customer>();
            // The hash is computed once because PBKDF2 is slow; every demo customer uses "Cliente123!".
            string hash = PAP.Business.PasswordHasher.Hash("Cliente123!");
            for (int i = 0; i < 50; i++)
            {
                string first = SampleData.Pick(SampleData.FirstNames);
                string last = SampleData.Pick(SampleData.LastNames);
                list.Add(new Customer
                {
                    FirstName = first,
                    LastName = last,
                    Email = SampleData.Email(first, last, i),
                    PasswordHash = hash,
                    Phone = SampleData.Phone(),
                    BirthDate = SampleData.DateBetween(1960, 2005),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return list;
        }
    }
}