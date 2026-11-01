using PAP.Generators;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View(GeneratorCatalog.GetStatuses());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(string entity)
        {
            GeneratorStatus status = GeneratorCatalog.GetStatuses().FirstOrDefault(x => x.EntityName == entity);
            if (status == null)
            {
                TempData["Error"] = "La entidad '" + entity + "' no existe.";
                return RedirectToAction("Index");
            }

            if (!status.CanGenerate)
            {
                TempData["Error"] = "Primero debe generar: " + string.Join(", ", status.MissingDependencies) + ".";
                return RedirectToAction("Index");
            }

            if (status.HasData)
            {
                TempData["Error"] = "La tabla " + status.EntityName + " ya contiene datos. No se generó nada.";
                return RedirectToAction("Index");
            }

            using (IGenerator generator = GeneratorCatalog.Create(entity))
            {
                try
                {
                    int count = generator.Generate();
                    TempData["Success"] = "Se generaron " + count + " registros en " + generator.EntityName + ".";
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                catch (Exception ex)
                {
                    Exception inner = ex;
                    while (inner.InnerException != null)
                        inner = inner.InnerException;
                    TempData["Error"] = "No se pudo generar " + generator.EntityName + ": " + inner.Message;
                }
            }

            return RedirectToAction("Index");
        }
    }
}