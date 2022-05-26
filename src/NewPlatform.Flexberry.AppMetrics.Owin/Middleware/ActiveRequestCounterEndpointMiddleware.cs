// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using App.Metrics;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    /// Обработчик счетчика одновременно выполняемых запросов.
    /// </summary>
    public class ActiveRequestCounterEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="owinOptions">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public ActiveRequestCounterEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics)
            : base(owinOptions, metrics)
        {
            if (owinOptions == null)
            {
                throw new ArgumentNullException(nameof(owinOptions));
            }
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
                Metrics.IncrementActiveRequests();
                await Next(environment).ConfigureAwait(true);
                Metrics.DecrementActiveRequests();
                MiddlewareExecuted();
            }
            else
            {
                await Next(environment).ConfigureAwait(true);
            }
        }
    }
}