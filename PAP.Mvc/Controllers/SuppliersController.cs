using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class SuppliersController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "Name", "Nombre" },
            { "ContactName", "Contacto" },
            { "Email", "Correo electrónico" },
            { "Phone", "Teléfono" },
            { "Address", "Dirección" },
            { "IsActive", "Activo" }
        };

        private readonly SupplierManager _manager = new SupplierManager();

        public ActionResult Index()
        {
            // sin listas
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Supplier x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                Name = x.Name,
                ContactName = x.ContactName,
                Email = x.Email,
                Phone = x.Phone,
                Address = x.Address,
                IsActive = x.IsActive
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,ContactName,Email,Phone,Address,IsActive")] Supplier model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Proveedor creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ContactName,Email,Phone,Address,IsActive")] Supplier model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Proveedor actualizado correctamente.");
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

            return Success("Proveedor eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
