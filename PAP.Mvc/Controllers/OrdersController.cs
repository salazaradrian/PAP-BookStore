using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class OrdersController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "CustomerId", "Cliente" },
            { "ShippingAddressId", "Dirección de envío" },
            { "Status", "Estado" },
            { "Subtotal", "Subtotal" },
            { "TaxAmount", "Impuestos" },
            { "ShippingCost", "Costo de envío" },
            { "Notes", "Notas" }
        };

        private readonly OrderManager _manager = new OrderManager();

        public ActionResult Index()
        {
            ViewBag.Customers = _manager.GetCustomersOptions();
            ViewBag.Addresses = _manager.GetAddressesOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Order x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer?.FirstName + " " + x.Customer?.LastName,
                ShippingAddressId = x.ShippingAddressId,
                ShippingAddress = x.Address == null ? null : x.Address.Street + ", " + x.Address.City,
                OrderDate = x.OrderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                Status = x.Status,
                Subtotal = x.Subtotal,
                SubtotalText = x.Subtotal.ToString("N2"),
                TaxAmount = x.TaxAmount,
                TaxAmountText = x.TaxAmount.ToString("N2"),
                ShippingCost = x.ShippingCost,
                ShippingCostText = x.ShippingCost.ToString("N2"),
                Total = x.Total,
                TotalText = x.Total.ToString("N2"),
                Notes = x.Notes,
                UpdatedAt = x.UpdatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv)
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,ShippingAddressId,Status,Subtotal,TaxAmount,ShippingCost,Notes")] Order model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Pedido creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerId,ShippingAddressId,Status,Subtotal,TaxAmount,ShippingCost,Notes")] Order model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Pedido actualizado correctamente.");
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

            return Success("Pedido eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
