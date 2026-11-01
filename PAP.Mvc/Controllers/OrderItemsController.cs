using PAP.Business;
using PAP.DataAccess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class OrderItemsController : CrudControllerBase
    {
        private static readonly IDictionary<string, string> Labels = new Dictionary<string, string>
        {
            { "OrderId", "Pedido" },
            { "BookId", "Libro" },
            { "Quantity", "Cantidad" },
            { "UnitPrice", "Precio unitario" },
            { "Discount", "Descuento" }
        };

        private readonly OrderItemManager _manager = new OrderItemManager();

        public ActionResult Index()
        {
            ViewBag.Orders = _manager.GetOrdersOptions();
            ViewBag.Books = _manager.GetBooksOptions();
            return View(_manager.GetAll());
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            OrderItem x = _manager.GetById(id);
            if (x == null)
                return HttpNotFound();

            CultureInfo inv = CultureInfo.InvariantCulture;
            var result = new
            {
                Id = x.Id,
                OrderId = x.OrderId,
                OrderNumber = "Pedido #" + x.OrderId,
                BookId = x.BookId,
                BookTitle = x.Book?.Title,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                UnitPriceText = x.UnitPrice.ToString("N2"),
                Discount = x.Discount,
                DiscountText = x.Discount.ToString("N2")
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,BookId,Quantity,UnitPrice,Discount")] OrderItem model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Artículo del pedido creado correctamente.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OrderId,BookId,Quantity,UnitPrice,Discount")] OrderItem model)
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

            return errors.Count > 0 ? Failure(errors) : Success("Artículo del pedido actualizado correctamente.");
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

            return Success("Artículo del pedido eliminado correctamente.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _manager.Dispose();

            base.Dispose(disposing);
        }
    }
}
