using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class AddressesController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "CustomerId", "Cliente" },
            { "AddressType", "Tipo" },
            { "Street", "Calle" },
            { "City", "Ciudad" },
            { "State", "Estado / Provincia" },
            { "PostalCode", "Código postal" },
            { "Country", "País" },
            { "IsDefault", "Predeterminada" }
        };

        private readonly AddressManager _manager = new AddressManager();

        public ActionResult Index()
        {
            ViewBag.Customers = _manager.GetCustomersOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Address x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.FirstName + " " + x.Customer?.LastName,
                AddressType = x.AddressType,
                Street = x.Street,
                City = x.City,
                State = x.State,
                PostalCode = x.PostalCode,
                Country = x.Country,
                IsDefault = x.IsDefault
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,AddressType,Street,City,State,PostalCode,Country,IsDefault")] Address model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Dirección creada correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerId,AddressType,Street,City,State,PostalCode,Country,IsDefault")] Address model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Dirección actualizada correctamente.");
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

            return Success("Dirección eliminada correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
