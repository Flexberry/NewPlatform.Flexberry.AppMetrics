﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using App.Metrics;
    using App.Metrics.Timer;
    using Internal;
    using Options;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Обработчик вычисления статистики по времени выполнения всех запросов.
    /// </summary>
    public class RequestTimerMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string TimerItemsKey = "__NewPlatform.Flexberry.AppMetrics.RequestTimer__";
        private readonly ITimer _requestTimer;

        public RequestTimerMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            _requestTimer = Metrics.Provider.Timer.Instance(OwinMetricsRegistry.HttpRequests.Timers.WebRequestTimer);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                environment[TimerItemsKey] = _requestTimer.NewContext();

                await Next(environment);

                var timer = environment[TimerItemsKey];
                using (timer as IDisposable)
                {
                }
                environment.Remove(TimerItemsKey);

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}