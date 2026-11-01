using System;
using System.Globalization;
using System.Web.Mvc;

namespace PAP.Mvc.ModelBinders
{
    /// <summary>
    /// Accepts decimals typed with either '.' or ',' as decimal separator.
    /// HTML number inputs always submit '.', which fails to parse under cultures such as es-CR.
    /// </summary>
    public class InvariantDecimalModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == null)
                return null;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);

            string raw = (result.AttemptedValue ?? string.Empty).Trim();
            if (raw.Length == 0)
            {
                if (Nullable.GetUnderlyingType(bindingContext.ModelType) != null)
                    return null;

                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Valor requerido.");
                return null;
            }

            // "1.234,56" -> "1234.56"; "12,5" -> "12.5"; "12.5" stays as is.
            int comma = raw.LastIndexOf(',');
            int dot = raw.LastIndexOf('.');
            if (comma >= 0 && dot >= 0)
                raw = comma > dot ? raw.Replace(".", "").Replace(',', '.') : raw.Replace(",", "");
            else if (comma >= 0)
                raw = raw.Replace(',', '.');

            decimal value;
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return value;

            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Valor decimal no válido.");
            return null;
        }
    }
}
