using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class ShoppingCartsController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "CustomerId", "Cliente" }
        };

        private readonly ShoppingCartManager _manager = new ShoppingCartManager();

        public ActionResult Index()
        {
            ViewBag.Customers = _manager.GetCustomersOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            ShoppingCart x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.FirstName + " " + x.Customer?.LastName,
                CreatedAt = x.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                UpdatedAt = x.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                ItemCount = x.CartItems.Count
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId")] ShoppingCart model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Carrito de compras creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerId")] ShoppingCart model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Carrito de compras actualizado correctamente.");
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

            return Success("Carrito de compras eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
