// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// Настройки метрик, специфичныех для интеграции Owin.
    /// </summary>
    public class OwinMetricsOptions
    {
        /// <summary>
        /// Включить сбор статистик APDEX.
        /// </summary>
        public bool ApdexTrackingEnabled { get; set; }

        /// <summary>
        /// Интервал сбора статистик APDEX.
        /// </summary>
        public double ApdexTSeconds { get; set; } = 0.5;

        /// <summary>
        /// Список регулярных выражений, по которым определяются пути не обрабатываемые метриками.
        /// </summary>
        public IList<string> IgnoredRoutesRegexPatterns { get; set; } = new List<string>();

        /// <summary>
        /// Путь по которому выдаются метрики.
        /// </summary>
        public string MetricsEndpoint { get; set; } = "/metrics";

        /// <summary>
        /// Активировать конечную точку с метриками по пути MetricsEndpoint.
        /// </summary>
        public bool MetricsEndpointEnabled { get; set; } = true;

        /// <summary>
        /// Путь по которому выдаются эхо-ответ.
        /// </summary>
        public string PingEndpoint { get; set; } = "/ping";

        /// <summary>
        /// Активировать конечную точку с эхо-ответом по пути PingEndpoint.
        /// </summary>
        public bool PingEndpointEnabled { get; set; }

        /// <summary>
        /// Активировать обработчик вычисления статистики по времени выполнения конкретных запросов. См. PerRequestTimerOptions.
        /// </summary>
        public bool PerRequestTimerEnabled { get; set; }

        /// <summary>
        /// Параметры сбора статистик на отдельных роутах.
        /// </summary>
        public PerRequestTimerOptions PerRequestTimerOptions { get; set; }

        /// <summary>
        /// Активировать обработчик вычисления статистики по времени выполнения всех запросов.
        /// </summary>
        public bool RequestTimerEnabled { get; set; }

        /// <summary>
        /// Активировать обработчик вычисления размера тела запросов PUT и POST.
        /// </summary>
        public bool PostAndPutRequestSizeEnabled { get; set; }

        /// <summary>
        /// Активировать обработчик счетчика одновременно выполняемых запросов.
        /// </summary>
        public bool ActiveRequestCounterEnabled { get; set; } = true;

        /// <summary>
        /// Активировать обработчик вычисления общей частоты ошибочных запросов.
        /// </summary>
        public bool ErrorRequestMeterEnabled { get; set; }
    }
}