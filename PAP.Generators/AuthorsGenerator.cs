using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class AuthorsGenerator : GeneratorBase<Author>
    {
        public AuthorsGenerator() : base(new AuthorRepository()) { }

        public override string EntityName { get { return "Authors"; } }

        protected override List<Author> Build()
        {
            var list = new List<Author>();
            for (int i = 0; i < 30; i++)
            {
                string first = SampleData.FirstNames[i % SampleData.FirstNames.Length];
                string last = SampleData.Pick(SampleData.LastNames);
                list.Add(new Author
                {
                    FirstName = first,
                    LastName = last,
                    Biography = first + " " + last + " es autor(a) con varias obras publicadas.",
                    BirthDate = SampleData.DateBetween(1940, 1995),
                    Country = SampleData.Pick(SampleData.Countries),
                    CreatedAt = DateTime.UtcNow
                });
            }
            return list;
        }
    }
}