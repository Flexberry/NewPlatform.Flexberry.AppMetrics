// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using App.Metrics;
    using Options;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Обработчик вычисления размера тела запросов PUT и POST.
    /// </summary>
    public class PostAndPutRequestSizeHistogramMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="options">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public PostAndPutRequestSizeHistogramMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
        }

        /// <summary>
        /// Метод вызываемый средой Owin для выполнения кода обработчика (Middleware).
        /// </summary>
        /// <param name="environment">Словарь содержащий контекст вызова.</param>
        /// <returns><see cref="Task"/> результат асинхронной операции.</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                var httpMethod = environment["owin.RequestMethod"].ToString().ToUpper();

                if (httpMethod == "POST" || httpMethod == "PUT")
                {
                    var headers = (IDictionary<string, string[]>)environment["owin.RequestHeaders"];
                    if (headers != null && headers.ContainsKey("Content-Length"))
                    {
                        Metrics.UpdatePostAndPutRequestSize(long.Parse(headers["Content-Length"].First()));
                    }
                }

                MiddlewareExecuted();
            }

            await Next(environment);
        }
    }
}