// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using App.Metrics;
    using App.Metrics.Formatters;
    using NewPlatform.Flexberry.AppMetrics.Owin.Extensions;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    /// Конечная точка для вывода текущего снимка метрик.
    /// </summary>
    public class MetricsEndpointMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string MetricsMimeType = "application/vnd.NewPlatform.Flexberry.AppMetrics.v1.metrics+json";
        private readonly IMetricsOutputFormatter _serializer;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="owinOptions">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        /// <param name="serializer">Сериализатор метрик.</param>
        public MetricsEndpointMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics, IMetricsOutputFormatter serializer)
            : base(owinOptions, metrics)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            _serializer = serializer;
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

            if (Options.MetricsEndpointEnabled && Options.MetricsEndpoint.IsPresent() && Options.MetricsEndpoint == requestPath)
            {
                MiddlewareExecuting();

                var result = string.Empty;
                using (var stream = new MemoryStream())
                {
                    var metricsData = Metrics.Snapshot.Get();
                    await _serializer.WriteAsync(stream, metricsData).ConfigureAwait(true);

                    result = Encoding.UTF8.GetString(stream.ToArray());
                }

                await WriteResponseAsync(environment, result, MetricsMimeType).ConfigureAwait(true);

                MiddlewareExecuted();

                return;
            }

            await Next(environment).ConfigureAwait(true);
        }
    }
}