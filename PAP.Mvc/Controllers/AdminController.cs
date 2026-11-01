using PAP.Generators;
using System;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View(GeneratorCatalog.EntityNames());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Generate(string entity)
        {
            using (IGenerator generator = GeneratorCatalog.Create(entity))
            {
                if (generator == null)
                {
                    TempData["Error"] = "La entidad '" + entity + "' no existe.";
                    return RedirectToAction("Index");
                }

                try
                {
                    if (generator.HasData())
                    {
                        TempData["Error"] = "La tabla " + generator.EntityName + " ya contiene datos. No se generó nada.";
                        return RedirectToAction("Index");
                    }

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