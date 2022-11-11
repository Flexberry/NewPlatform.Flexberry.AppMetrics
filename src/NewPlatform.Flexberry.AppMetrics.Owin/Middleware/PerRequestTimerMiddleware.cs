// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using App.Metrics;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    /// Обработчик вычисления статистики по времени выполнения конкретных запросов.
    /// </summary>
    public class PerRequestTimerMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string TimerItemsKey = "__NewPlatform.Flexberry.AppMetrics.PerRequestStartTime__";

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="owinOptions">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public PerRequestTimerMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics)
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
            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                if (environment == null)
                {
                    throw new ArgumentNullException(nameof(environment));
                }

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString(), CultureInfo.InvariantCulture);

                environment[TimerItemsKey] = Metrics.Clock.Nanoseconds;

                await Next(environment).ConfigureAwait(true);

                if (httpResponseStatusCode != (int)HttpStatusCode.NotFound)
                {
                    var startTime = (long)environment[TimerItemsKey];
                    var elapsed = Metrics.Clock.Nanoseconds - startTime;
                    var path = GetMetricsCurrentRouteName(environment, Options.PerRequestTimerOptions.AppendHttpMethod);
                    Metrics.RecordEndpointRequestTime(path, elapsed);
                }

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment).ConfigureAwait(true);
            }
        }

        /// <inheritdoc/>
        protected override bool ShouldPerformMetric(IDictionary<string, object> environment)
        {
            if (!base.ShouldPerformMetric(environment))
            {
                return false;
            }

            var includeList = Options.PerRequestTimerOptions.IncludeRouteList;
            if (includeList.Any())
            {
                var path = GetMetricsCurrentRouteName(environment);
                return includeList.Contains(path);
            }

            return true;
        }
    }
}