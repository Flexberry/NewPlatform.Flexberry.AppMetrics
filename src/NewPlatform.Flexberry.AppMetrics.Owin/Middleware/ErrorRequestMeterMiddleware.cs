// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using App.Metrics;
    using Options;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///  Обработчик вычисления общей частоты ошибочных запросов.
    /// </summary>
    public class ErrorRequestMeterMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public ErrorRequestMeterMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {

        }

        /// <summary>
        /// Метод вызываемый средой Owin для выполнения кода обработчика (Middleware).
        /// </summary>
        /// <param name="environment">Словарь содержащий контекст вызова.</param>
        /// <returns><see cref="Task"/> результат асинхронной операции.</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            await Next(environment);

            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                var routeTemplate = GetMetricsCurrentRouteName(environment);

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString());

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