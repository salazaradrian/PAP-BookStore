using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class PurchaseOrdersController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "SupplierId", "Proveedor" },
            { "ExpectedDate", "Fecha esperada" },
            { "Status", "Estado" },
            { "TotalCost", "Costo total" }
        };

        private readonly PurchaseOrderManager _manager = new PurchaseOrderManager();

        public ActionResult Index()
        {
            ViewBag.Suppliers = _manager.GetSuppliersOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            PurchaseOrder x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                SupplierId = x.SupplierId,
                SupplierName = x.Supplier?.Name,
                OrderDate = x.OrderDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                ExpectedDate = x.ExpectedDate?.ToString("yyyy-MM-dd", inv),
                Status = x.Status,
                TotalCost = x.TotalCost,
                TotalCostText = x.TotalCost.ToString("N2")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,ExpectedDate,Status,TotalCost")] PurchaseOrder model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Orden de compra creada correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SupplierId,ExpectedDate,Status,TotalCost")] PurchaseOrder model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Orden de compra actualizada correctamente.");
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

            return Success("Orden de compra eliminada correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
