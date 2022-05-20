namespace App.Metrics.Extensions.Owin.Extensions
{
    /// <summary>
    /// Вспомогательные методы работы со строками.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Добавить слэш в начало строки при его отсутствии.
        /// </summary>
        /// <param name="url">Строка.</param>
        /// <returns>Строка со слэшем в начале.</returns>
        internal static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }

        /// <summary>
        /// Проверить что строка содержит значение.
        /// </summary>
        /// <param name="value">Строка.</param>
        /// <returns>true если в строке есть символы отличные от заполнителей.</returns>
        internal static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Удалить слэш в начале строки при его наличии.
        /// </summary>
        /// <param name="url">Строка.</param>
        /// <returns>Строка без слэша в начале.</returns>
        internal static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            return url;
        }
    }
}