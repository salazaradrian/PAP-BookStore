using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class BooksGenerator : GeneratorBase<Book>
    {
        private readonly BookRepository _books;
        private readonly Dictionary<Book, List<int>> _authorsByBook = new Dictionary<Book, List<int>>();

        public BooksGenerator() : this(new BookRepository()) { }

        private BooksGenerator(BookRepository repository) : base(repository)
        {
            _books = repository;
        }

        public override string EntityName { get { return "Books"; } }

        protected override List<Book> Build()
        {
            List<Category> categories = Load(new CategoryRepository());
            List<Genre> genres = Load(new GenreRepository());
            List<Publisher> publishers = Load(new PublisherRepository());
            List<int> authorIds = RequireIds(Load(new AuthorRepository()).Select(x => x.Id), "Authors");
            RequireIds(categories.Select(x => x.Id), "Categories");
            RequireIds(genres.Select(x => x.Id), "Genres");
            RequireIds(publishers.Select(x => x.Id), "Publishers");

            string[] starts = { "El", "La", "Los", "Las", "Un", "Mas alla de" };
            string[] nouns = { "viaje", "secreto", "jardin", "camino", "puente", "bosque", "faro", "reino", "silencio", "viento", "archivo", "laberinto" };
            string[] adjectives = { "perdido", "infinito", "oculto", "ultimo", "azul", "olvidado", "eterno", "lejano" };
            string[] languages = { "Espanol", "Espanol", "Espanol", "Ingles", "Frances" };

            var list = new List<Book>();
            for (int i = 0; i < 60; i++)
            {
                Category category = SampleData.Pick(categories);
                Genre genre = SampleData.Pick(genres);
                Publisher publisher = SampleData.Pick(publishers);
                string title = SampleData.Pick(starts) + " " + SampleData.Pick(nouns) + " " + SampleData.Pick(adjectives) + " " + (i + 1);

                // Between one and three distinct authors per book.
                List<int> chosen = authorIds.OrderBy(x => SampleData.Rng.Next()).Take(SampleData.Between(1, 3)).ToList();

                var book = new Book
                {
                    Name = title,
                    Title = title,
                    ISBN = "978" + (1000000000L + i * 7919L).ToString(),
                    CategoryId = category.Id,
                    GenreId = genre.Id,
                    PublisherId = publisher.Id,
                    Category = category.Name,
                    Genre = genre.Name,
                    Publisher = publisher.Name,
                    Author = "Varios autores",
                    Description = "Libro de " + genre.Name.ToLowerInvariant() + " de la categoria " + category.Name.ToLowerInvariant() + ".",
                    PublicationDate = SampleData.DateBetween(1990, 2025),
                    Pages = SampleData.Between(120, 900),
                    Language = SampleData.Pick(languages),
                    Price = SampleData.Money(8, 90),
                    IsBestSeller = SampleData.Chance(15),
                    IsInStock = true,
                    CoverImageUrl = "https://covers.example.com/" + (i + 1) + ".jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                list.Add(book);
                _authorsByBook[book] = chosen;
            }
            return list;
        }

        protected override void Insert(List<Book> items)
        {
            _books.AddRange(items, _authorsByBook);
        }
    }
}