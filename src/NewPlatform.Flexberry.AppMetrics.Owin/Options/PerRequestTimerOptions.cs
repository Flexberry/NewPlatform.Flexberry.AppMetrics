namespace NewPlatform.Flexberry.AppMetrics.Owin.Options
{
    using System.Collections.Generic;
    using System.Linq;

    public class PerRequestTimerOptions
    {
        /// <summary>
        /// Добавлять ли имя HTTP-метода в имя роута.
        /// </summary>
        public bool AppendHttpMethod { get; set; }

        /// <summary>
        /// Список роутов, с которых необходимо снимать метрики. Если список пуст - метрики записываются со ВСЕХ роутов.
        /// </summary>
        public IEnumerable<string> IncludeRouteList { get; set; } = Enumerable.Empty<string>();
    }
}
