// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Middleware
{
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