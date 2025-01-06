using System.Text.RegularExpressions;

namespace Hub.Infrastructure.Architecture.Internacionalization.DocumentValidators
{
    /// <summary>
    /// Validator para documentos dos Estados Unidos
    /// </summary>
    public static class EUADocumentValidator
    {
        /// <summary>
        /// Valida um documento SSN (Security Social Number)
        /// </summary>
        /// <param name="ssn"> documento a ser validado </param>
        /// <returns></returns>
        public static bool IsValidSSN(string ssn)
        {
            // Formato esperado: 123-45-6789
            return Regex.IsMatch(ssn, @"^\d{3}-\d{2}-\d{4}$");
        }
    }
}
