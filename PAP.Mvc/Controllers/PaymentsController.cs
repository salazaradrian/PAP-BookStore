using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class PaymentsController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "OrderId", "Pedido" },
            { "PaymentMethod", "Método de pago" },
            { "Amount", "Monto" },
            { "Status", "Estado" },
            { "TransactionRef", "Referencia de transacción" }
        };

        private readonly PaymentManager _manager = new PaymentManager();

        public ActionResult Index()
        {
            ViewBag.Orders = _manager.GetOrdersOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            Payment x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                OrderId = x.OrderId,
                OrderNumber = "Pedido #" + x.OrderId,
                PaymentMethod = x.PaymentMethod,
                Amount = x.Amount,
                AmountText = x.Amount.ToString("N2"),
                PaymentDate = x.PaymentDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm", inv),
                Status = x.Status,
                TransactionRef = x.TransactionRef
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,PaymentMethod,Amount,Status,TransactionRef")] Payment model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Pago creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderId,PaymentMethod,Amount,Status,TransactionRef")] Payment model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Pago actualizado correctamente.");
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

            return Success("Pago eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
