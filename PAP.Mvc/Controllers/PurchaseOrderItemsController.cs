using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class PurchaseOrderItemsController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "PurchaseOrderId", "Orden de compra" },
            { "BookId", "Libro" },
            { "Quantity", "Cantidad" },
            { "UnitCost", "Costo unitario" }
        };

        private readonly PurchaseOrderItemManager _manager = new PurchaseOrderItemManager();

        public ActionResult Index()
        {
            ViewBag.PurchaseOrders = _manager.GetPurchaseOrdersOptions();
            ViewBag.Books = _manager.GetBooksOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            PurchaseOrderItem x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                PurchaseOrderId = x.PurchaseOrderId,
                PurchaseOrderNumber = "Orden #" + x.PurchaseOrderId,
                BookId = x.BookId,
                BookTitle = x.Book?.Title,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                UnitCostText = x.UnitCost.ToString("N2")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseOrderId,BookId,Quantity,UnitCost")] PurchaseOrderItem model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Artículo de orden de compra creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PurchaseOrderId,BookId,Quantity,UnitCost")] PurchaseOrderItem model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Artículo de orden de compra actualizado correctamente.");
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

            return Success("Artículo de orden de compra eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
