using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace PAP.Business
{
    /// <summary>
    /// Small reusable validation helpers shared by all managers. Messages are in Spanish.
    /// </summary>
    public static class ValidationRules
    {
        public static string Clean(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static void Required(IList<string> errors, string label, string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add("El campo '" + label + "' es obligatorio.");
                return;
            }
            MaxLength(errors, label, value, maxLength);
        }

        public static void Optional(IList<string> errors, string label, string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value))
                MaxLength(errors, label, value, maxLength);
        }

        public static void Email(IList<string> errors, string label, string value, bool required, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (required)
                    errors.Add("El campo '" + label + "' es obligatorio.");
                return;
            }

            if (!MaxLength(errors, label, value, maxLength))
                return;

            try
            {
                var address = new MailAddress(value);
                if (address.Address != value)
                    errors.Add("El campo '" + label + "' no es un correo válido.");
            }
            catch (FormatException)
            {
                errors.Add("El campo '" + label + "' no es un correo válido.");
            }
        }

        public static void Url(IList<string> errors, string label, string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (!MaxLength(errors, label, value, maxLength))
                return;

            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                errors.Add("El campo '" + label + "' debe ser una URL http o https válida.");
        }

        public static void Selected(IList<string> errors, string label, int? id)
        {
            if (!id.HasValue || id.Value <= 0)
                errors.Add("Debe seleccionar un valor en '" + label + "'.");
        }

        public static void Range(IList<string> errors, string label, decimal value, decimal min, decimal max)
        {
            if (value < min || value > max)
                errors.Add("El campo '" + label + "' debe estar entre " + min + " y " + max + ".");
        }

        public static void Password(IList<string> errors, string password, bool required)
        {
            if (string.IsNullOrEmpty(password))
            {
                if (required)
                    errors.Add("La contraseña es obligatoria.");
                return;
            }

            if (password.Length < 8 || password.Length > 100)
                errors.Add("La contraseña debe tener entre 8 y 100 caracteres.");
        }

        private static bool MaxLength(IList<string> errors, string label, string value, int maxLength)
        {
            if (value.Length > maxLength)
            {
                errors.Add("El campo '" + label + "' no puede superar los " + maxLength + " caracteres.");
                return false;
            }
            return true;
        }
    }
}
