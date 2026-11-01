using PAP.DataAccess;
using PAP.Repositories;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace PAP.Business
{
    public partial class BookManager
    {
        private readonly BookRepository _repository;

        public BookManager()
        {
            _repository = new BookRepository();
        }

        public IEnumerable<Book> GetAllBooksFromDataModel()
        {
            var books = _repository.GetAll();

            // DOT NOTATION
            // Multiple Enumeration / Deferred Execution



            var myEnumerable= books
                .Where(x => x.IsInStock && x.Price > 20)
                .Where(x => x.Name.Contains("a"))
                .Where(x => x.Category == "Fiction")
                .Select(x => new Book
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    Category = x.Category,
                    Publisher = x.Publisher,
                    Author = x.Author,
                    Description = x.Description,
                    PublicationDate = x.PublicationDate,
                    Height = x.Height,
                    Width = x.Width,
                    Price = x.Price,
                    IsBestSeller = x.IsBestSeller,
                    Genre = x.Genre,
                    IsInStock = x.IsInStock,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                });

            // 10,000 || 100,000
            // for (Kb time) = i F(x)
            // foreach (^ Kb time) objects i=0 | x = object[i] | F(x) = x.Price > 50
            // Linq.ForEach

            // Francisco Rojas = 001110101000010000100010 /  1

            // quick sort / topo sort / merge sort / bubble sort / insertion sort / selection sort
            // binary srch / linear search / hash table / dictionary / set / list / array / linked list
            // queues / stacks / trees / graphs

            foreach (var book in myEnumerable)
            {
                if(book.Price > 50)
                {
                    book.IsBestSeller = true;
                }   
                yield return book;
            }
            /*return from x in books
                   where x.IsInStock && x.Price > 20
                   select new Book
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Title = x.Title,
                       Category = x.Category,
                       Publisher = x.Publisher,
                       Author = x.Author,
                       Description = x.Description,
                       PublicationDate = x.PublicationDate,
                       Height = x.Height,
                       Width = x.Width,
                       Price = x.Price,
                       IsBestSeller = x.IsBestSeller,
                       Genre = x.Genre,
                       IsInStock = x.IsInStock,
                       CreatedAt = x.CreatedAt,
                       UpdatedAt = x.UpdatedAt
                   };*/
        }   
    }
}
