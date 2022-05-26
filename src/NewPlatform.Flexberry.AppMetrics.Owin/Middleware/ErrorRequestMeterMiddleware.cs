// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using App.Metrics;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    ///  Обработчик вычисления общей частоты ошибочных запросов.
    /// </summary>
    public class ErrorRequestMeterMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="owinOptions">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public ErrorRequestMeterMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics)
            : base(owinOptions, metrics)
        {
        }

        /// <summary>
        /// Метод вызываемый средой Owin для выполнения кода обработчика (Middleware).
        /// </summary>
        /// <param name="environment">Словарь содержащий контекст вызова.</param>
        /// <returns><see cref="Task"/> результат асинхронной операции.</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next(environment).ConfigureAwait(true);

            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                var routeTemplate = GetMetricsCurrentRouteName(environment);

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString(), CultureInfo.InvariantCulture);

                if (!(httpResponseStatusCode >= (int)HttpStatusCode.OK && httpResponseStatusCode <= 299))
                {
                    Metrics.MarkHttpRequestEndpointError(routeTemplate, httpResponseStatusCode);
                    Metrics.MarkHttpRequestError(httpResponseStatusCode);
                    Metrics.ErrorRequestPercentage();
                }
            }

            MiddlewareExecuted();
        }
    }
}