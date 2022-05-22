// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics
{
    using App.Metrics;
    using App.Metrics.Gauge;
    using NewPlatform.Flexberry.AppMetrics.Owin.Internal;

    /// <summary>
    /// Хэлперы для записи метрик.
    /// </summary>
    internal static class MetricsExtensions
    {
        /// <summary>
        /// Увеличить количество активных запросов.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics IncrementActiveRequests(this IMetrics metrics)
        {
            metrics.Measure.Counter.Increment(OwinMetricsRegistry.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        /// <summary>
        /// Уменьшить количество активных запросов.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics DecrementActiveRequests(this IMetrics metrics)
        {
            metrics.Measure.Counter.Decrement(OwinMetricsRegistry.HttpRequests.Counters.ActiveRequests);

            return metrics;
        }

        /// <summary>
        /// Вычислить процент ошибочных запросов.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics ErrorRequestPercentage(this IMetrics metrics)
        {
            var errors = metrics.Provider.Meter.Instance(OwinMetricsRegistry.HttpRequests.Meters.HttpErrorRequests);
            var requests = metrics.Provider.Timer.Instance(OwinMetricsRegistry.HttpRequests.Timers.WebRequestTimer);

            metrics.Measure.Gauge.SetValue(
                OwinMetricsRegistry.HttpRequests.Gauges.PercentageErrorRequests,
                () => new HitPercentageGauge(errors, requests, m => m.OneMinuteRate));

            return metrics;
        }

        /// <summary>
        /// Записать показание для счетчика ошибок обращения к конечным точкам.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <param name="routeTemplate">Путь роута.</param>
        /// <param name="httpStatusCode">Код HTTP-ответа.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics MarkHttpRequestEndpointError(this IMetrics metrics, string routeTemplate, int httpStatusCode)
        {

            metrics.Measure.Meter.Mark(
                OwinMetricsRegistry.HttpRequests.Meters.EndpointHttpErrorRequests(routeTemplate),
                new MetricSetItem("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        /// <summary>
        /// Записать показание для счетчика ошибочных запросов.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <param name="httpStatusCode">Код HTTP-ответа.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics MarkHttpRequestError(this IMetrics metrics, int httpStatusCode)
        {
            metrics.Measure.Meter.Mark(
                OwinMetricsRegistry.HttpRequests.Meters.HttpErrorRequests,
                new MetricSetItem("http_status_code", httpStatusCode.ToString()));

            return metrics;
        }

        /// <summary>
        /// Записать показание для счетчика времени обработки вызова к конечных точек.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <param name="routeTemplate">Путь роута.</param>
        /// <param name="httpStatusCode">Код HTTP-ответа.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics RecordEndpointRequestTime(this IMetrics metrics, string routeTemplate, long elapsed)
        {
            metrics.Provider.Timer
                   .Instance(OwinMetricsRegistry.HttpRequests.Timers.EndpointPerRequestTimer(routeTemplate))
                   .Record(elapsed, TimeUnit.Nanoseconds);

            return metrics;
        }

        /// <summary>
        /// Записать показание для счетчика размера тела запросов PUT и POST.
        /// </summary>
        /// <param name="metrics">Объект учета метрик.</param>
        /// <param name="value">Размер тела HTTP-запроса.</param>
        /// <returns>Объект учета метрик.</returns>
        public static IMetrics UpdatePostAndPutRequestSize(this IMetrics metrics, long value)
        {
            metrics.Measure.Histogram.Update(OwinMetricsRegistry.HttpRequests.Histograms.PostAndPutRequestSize, value);
            return metrics;
        }
    }
}