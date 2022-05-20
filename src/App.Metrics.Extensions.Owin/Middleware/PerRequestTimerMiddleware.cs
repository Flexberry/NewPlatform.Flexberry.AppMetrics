// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
    using Options;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Обработчик вычисления статистики по времени выполнения конкретных запросов.
    /// </summary>
    public class PerRequestTimerMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string TimerItemsKey = "__App.Metrics.PerRequestStartTime__";


        public PerRequestTimerMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {

        }

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

        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (ShouldPerformMetric(environment))
            {
                MiddlewareExecuting();

                var httpResponseStatusCode = int.Parse(environment["owin.ResponseStatusCode"].ToString());

                environment[TimerItemsKey] = Metrics.Clock.Nanoseconds;

                await Next(environment);

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
                await Next(environment);
            }
        }
    }
}