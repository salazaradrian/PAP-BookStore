using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class PublishersGenerator : GeneratorBase<Publisher>
    {
        public PublishersGenerator() : base(new PublisherRepository()) { }

        public override string EntityName { get { return "Publishers"; } }

        protected override List<Publisher> Build()
        {
            var list = new List<Publisher>();
            string[] names = { "Editorial Planeta", "Alfaguara", "Anagrama", "Santillana", "Penguin Books", "Editorial Norma", "Tusquets", "Siruela", "Editorial Universitaria", "Ediciones B" };
            for (int i = 0; i < names.Length; i++)
            {
                string slug = names[i].ToLowerInvariant().Replace(" ", "");
                list.Add(new Publisher
                {
                    Name = names[i],
                    Country = SampleData.Pick(SampleData.Countries),
                    Website = "https://www." + slug + ".example.com",
                    Email = "contacto" + i + "@" + slug + ".example.com",
                    CreatedAt = DateTime.UtcNow
                });
            }
            return list;
        }
    }
}