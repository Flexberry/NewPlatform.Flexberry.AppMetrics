// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using App.Metrics;
    using NewPlatform.Flexberry.AppMetrics.Owin.Extensions;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    /// Конечная точка эхо-ответа.
    /// </summary>
    public class PingEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="owinOptions">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public PingEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics)
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
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var requestPath = environment["owin.RequestPath"] as string;

            if (Options.PingEndpointEnabled && Options.PingEndpoint.IsPresent() && Options.PingEndpoint == requestPath)
            {
                MiddlewareExecuting();

                await WriteResponseAsync(environment, "pong", "text/plain").ConfigureAwait(true);

                MiddlewareExecuted();

                return;
            }

            await Next(environment).ConfigureAwait(true);
        }
    }
}