using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class ReviewsGenerator : GeneratorBase<Review>
    {
        public ReviewsGenerator() : base(new ReviewRepository()) { }

        public override string EntityName { get { return "Reviews"; } }

        protected override List<Review> Build()
        {
            List<int> customerIds = RequireIds(Load(new CustomerRepository()).Select(x => x.Id), "Customers");
            List<int> bookIds = RequireIds(Load(new BookRepository()).Select(x => x.Id), "Books");

            string[] comments =
            {
                "Excelente lectura, muy recomendado.", "Me gusto bastante.", "Entretenido de principio a fin.",
                "Esperaba un poco mas.", "Una historia memorable.", "Buena edicion y buen precio."
            };

            // (BookId, CustomerId) is unique, so each pair is used only once.
            var used = new HashSet<string>();
            var list = new List<Review>();
            for (int i = 0; i < 150; i++)
            {
                int bookId = SampleData.Pick(bookIds);
                int customerId = SampleData.Pick(customerIds);
                if (!used.Add(bookId + "-" + customerId))
                    continue;

                list.Add(new Review
                {
                    BookId = bookId,
                    CustomerId = customerId,
                    Rating = (byte)SampleData.Between(1, 5),
                    Comment = SampleData.Pick(comments),
                    CreatedAt = SampleData.Recent(200)
                });
            }
            return list;
        }
    }
}