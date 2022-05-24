// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using App.Metrics;
    using Extensions;
    using Options;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Конечная точка эхо-ответа.
    /// </summary>
    public class PingEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        public PingEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {

        }

        /// <summary>
        /// Метод вызываемый средой Owin для выполнения кода обработчика (Middleware).
        /// </summary>
        /// <param name="environment">Словарь содержащий контекст вызова.</param>
        /// <returns><see cref="Task"/> результат асинхронной операции.</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestPath = environment["owin.RequestPath"] as string;

            if (Options.PingEndpointEnabled && Options.PingEndpoint.IsPresent() && Options.PingEndpoint == requestPath)
            {
                MiddlewareExecuting();

                await WriteResponseAsync(environment, "pong", "text/plain");

                MiddlewareExecuted();

                return;
            }

            await Next(environment);
        }
    }
}