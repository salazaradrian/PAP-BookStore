using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class ReviewsController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "BookId", "Libro" },
            { "CustomerId", "Cliente" },
            { "Rating", "Calificación (1 a 5)" },
            { "Comment", "Comentario" }
        };

        private readonly ReviewManager _manager = new ReviewManager();

        public ActionResult Index()
        {
            ViewBag.Books = _manager.GetBooksOptions();
            ViewBag.Customers = _manager.GetCustomersOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Review x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                BookId = x.BookId,
                BookTitle = x.Book?.Title,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.FirstName + " " + x.Customer?.LastName,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookId,CustomerId,Rating,Comment")] Review model)
        {
            IList<string> errors = BindingErrors(Labels);
            if (errors.Count > 0)
                return Failure(errors);

            try
            {
                errors = _manager.Create(model);
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

            return errors.Count > 0 ? Failure(errors) : Success("Reseña creada correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BookId,CustomerId,Rating,Comment")] Review model)
        {
            IList<string> errors = BindingErrors(Labels);
            if (errors.Count > 0)
                return Failure(errors);

            try
            {
                errors = _manager.Update(model);
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }

            return errors.Count > 0 ? Failure(errors) : Success("Reseña actualizada correctamente.");
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

            return Success("Reseña eliminada correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
