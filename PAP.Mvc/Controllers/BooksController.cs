using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class BooksController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "Title", "Título" },
            { "Name", "Nombre interno (opcional)" },
            { "ISBN", "ISBN" },
            { "PublisherId", "Editorial" },
            { "CategoryId", "Categoría" },
            { "GenreId", "Género" },
            { "Language", "Idioma" },
            { "Pages", "Páginas" },
            { "PublicationDate", "Fecha de publicación" },
            { "Price", "Precio" },
            { "Height", "Alto" },
            { "Width", "Ancho" },
            { "IsBestSeller", "Más vendido" },
            { "IsInStock", "En stock" },
            { "CoverImageUrl", "URL de la portada" },
            { "Description", "Descripción" },
            { "AuthorIds", "Autores" }
        };

        private readonly BookManager _manager = new BookManager();

        public ActionResult Index()
        {
            ViewBag.Publishers = _manager.GetPublishersOptions();
            ViewBag.Categories = _manager.GetCategoriesOptions();
            ViewBag.Genres = _manager.GetGenresOptions();
            ViewBag.Authors = _manager.GetAuthorsOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Book x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                Title = x.Title,
                Name = x.Name,
                ISBN = x.ISBN,
                PublisherId = x.PublisherId,
                PublisherName = x.Publisher1?.Name,
                CategoryId = x.CategoryId,
                CategoryName = x.Category1?.Name,
                GenreId = x.GenreId,
                GenreName = x.Genre1?.Name,
                Language = x.Language,
                Pages = x.Pages,
                PublicationDate = x.PublicationDate?.ToString("yyyy-MM-dd", inv),
                Price = x.Price,
                PriceText = x.Price.ToString("N2"),
                Height = x.Height,
                HeightText = x.Height?.ToString("N2"),
                Width = x.Width,
                WidthText = x.Width?.ToString("N2"),
                IsBestSeller = x.IsBestSeller,
                IsInStock = x.IsInStock,
                CoverImageUrl = x.CoverImageUrl,
                Description = x.Description,
                CreatedAt = x.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                UpdatedAt = x.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                Authors = string.Join(", ", x.Authors.Select(a => a.FirstName + " " + a.LastName)),
                AuthorIds = x.Authors.Select(a => a.Id).ToArray()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Name,ISBN,PublisherId,CategoryId,GenreId,Language,Pages,PublicationDate,Price,Height,Width,IsBestSeller,IsInStock,CoverImageUrl,Description")] Book model, int[] authorIds)
        {
            IList<string> errors = BindingErrors(Labels);
            if (errors.Count > 0)
                return Failure(errors);

            try
            {
                errors = _manager.Create(model, authorIds == null ? new List<int>() : authorIds.ToList());
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

            return errors.Count > 0 ? Failure(errors) : Success("Libro creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Name,ISBN,PublisherId,CategoryId,GenreId,Language,Pages,PublicationDate,Price,Height,Width,IsBestSeller,IsInStock,CoverImageUrl,Description")] Book model, int[] authorIds)
        {
            IList<string> errors = BindingErrors(Labels);
            if (errors.Count > 0)
                return Failure(errors);

            try
            {
                errors = _manager.Update(model, authorIds == null ? new List<int>() : authorIds.ToList());
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

            return errors.Count > 0 ? Failure(errors) : Success("Libro actualizado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _manager.Delete(id);
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

            return Success("Libro eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
