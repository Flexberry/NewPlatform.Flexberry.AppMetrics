﻿// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Middleware
{
    using App.Metrics;
    using App.Metrics.Apdex;
    using Internal;
    using Options;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Обработчик вычисления статистики APDEX.
    /// </summary>
    public class ApdexMiddleware : AppMetricsMiddleware<OwinMetricsOptions>
    {
        private const string ApdexItemsKey = "__NewPlatform.Flexberry.AppMetrics.Apdex__";
        private readonly IApdex _apdexTracking;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="options">Класс параметров.</param>
        /// <param name="metrics">Объект с метриками.</param>
        public ApdexMiddleware(OwinMetricsOptions owinOptions, IMetrics metrics) : base(owinOptions, metrics)
        {
            _apdexTracking = Metrics.Provider.Apdex.Instance(OwinMetricsRegistry.HttpRequests.ApdexScores.Apdex(owinOptions.ApdexTSeconds));
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

                environment[ApdexItemsKey] = _apdexTracking.NewContext();

                await Next(environment);

                var apdex = environment[ApdexItemsKey];
                using (apdex as IDisposable)
                {
                }
                environment.Remove(ApdexItemsKey);

                MiddlewareExecuted();
            }
            else
            {
                await Next(environment);
            }
        }
    }
}