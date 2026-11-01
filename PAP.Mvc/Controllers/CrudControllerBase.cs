using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace PAP.Mvc.Controllers
{
    /// <summary>
    /// Shared helpers for the CRUD controllers (messages, error translation, binding errors).
    /// </summary>
    public abstract class CrudControllerBase : Controller
    {
        protected ActionResult Success(string message)
        {
            TempData["Success"] = message;
            return RedirectToAction("Index");
        }

        protected ActionResult Failure(IEnumerable<string> errors)
        {
            TempData["Error"] = string.Join("\n", errors);
            return RedirectToAction("Index");
        }

        protected ActionResult Failure(Exception ex)
        {
            return Failure(new[] { Translate(ex) });
        }

        // Only conversion errors (a value was submitted but could not be parsed) are reported.
        protected IList<string> BindingErrors(IDictionary<string, string> labels)
        {
            var errors = new List<string>();
            foreach (var entry in ModelState)
            {
                if (entry.Value.Errors.Count == 0 || entry.Value.Value == null)
                    continue;

                string label;
                if (!labels.TryGetValue(entry.Key, out label))
                    label = entry.Key;

                errors.Add("El valor del campo '" + label + "' no es válido.");
            }
            return errors;
        }

        protected static string Translate(Exception ex)
        {
            for (Exception current = ex; current != null; current = current.InnerException)
            {
                var sql = current as SqlException;
                if (sql == null)
                    continue;

                switch (sql.Number)
                {
                    case 2601:
                    case 2627:
                        return "Ya existe un registro con ese valor único (por ejemplo correo, ISBN, nombre o libro duplicado).";
                    case 547:
                        return "La operación viola una restricción de la base de datos: el registro está relacionado con otros datos o un valor no está permitido.";
                }
            }

            if (ex is InvalidOperationException)
                return ex.Message;

            return "Ocurrió un error inesperado al guardar los datos.";
        }
    }
}
