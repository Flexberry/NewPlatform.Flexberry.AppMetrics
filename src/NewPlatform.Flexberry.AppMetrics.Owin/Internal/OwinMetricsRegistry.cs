// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Internal
{
    using System;
    using App.Metrics;
    using App.Metrics.Apdex;
    using App.Metrics.Counter;
    using App.Metrics.Gauge;
    using App.Metrics.Histogram;
    using App.Metrics.Meter;
    using App.Metrics.Timer;

    /// <summary>
    /// Реестр метрик.
    /// </summary>
    internal static class OwinMetricsRegistry
    {
        /// <summary>
        /// Реестр метрик Http-запросов.
        /// </summary>
        public static class HttpRequests
        {
            /// <summary>
            /// Имя ключа для обозначения контекста.
            /// </summary>
            private const string ContextName = "Application.HttpRequests";

            /// <summary>
            /// Счетчики метрик APDEX.
            /// </summary>
            public static class ApdexScores
            {
                /// <summary>
                /// Название метрики Apdex.
                /// </summary>
                public const string ApdexMetricName = "Apdex";

                /// <summary>
                /// Параметры счетчика метрик APDEX.
                /// </summary>
                public static Func<double, ApdexOptions> Apdex = apdexTSeconds => new ApdexOptions
                {
                    Context = ContextName,
                    Name = ApdexMetricName,
                    ApdexTSeconds = apdexTSeconds,
                };
            }

            /// <summary>
            /// Датчики метрик.
            /// </summary>
            public static class Gauges
            {
                /// <summary>
                /// Параметры датчика ошибочных запросов.
                /// </summary>
                public static GaugeOptions PercentageErrorRequests = new GaugeOptions
                {
                    Context = ContextName,
                    Name = "Percentage Error Requests",
                    MeasurementUnit = Unit.Custom("Error Requests"),
                };
            }

            /// <summary>
            /// Счетчики метрик.
            /// </summary>
            public static class Counters
            {
                /// <summary>
                /// Параметры счетчика активных запросов.
                /// </summary>
                public static CounterOptions ActiveRequests = new CounterOptions
                {
                    Context = ContextName,
                    Name = "Active Requests",
                    MeasurementUnit = Unit.Custom("Active Requests"),
                };
            }

            /// <summary>
            /// Диаграммы метрик.
            /// </summary>
            public static class Histograms
            {
                /// <summary>
                /// Параметры диаграммы размера запросов POST и PUT.
                /// </summary>
                public static HistogramOptions PostAndPutRequestSize = new HistogramOptions
                {
                    Context = ContextName,
                    Name = "Http Request Post & Put Size",
                    MeasurementUnit = Unit.Bytes,
                };
            }

            /// <summary>
            /// Счетчики частоты и количества событий.
            /// </summary>
            public static class Meters
            {
                /// <summary>
                /// Параметры счетчика ошибочных запросов endpoint.
                /// </summary>
                public static Func<string, MeterOptions> EndpointHttpErrorRequests = routeTemplate => new MeterOptions
                {
                    Context = ContextName,
                    Name = $"{routeTemplate} Http Error Requests",
                    MeasurementUnit = Unit.Requests,
                };

                /// <summary>
                /// Параметры счетчика ошибочных запросов.
                /// </summary>
                public static MeterOptions HttpErrorRequests = new MeterOptions
                {
                    Context = ContextName,
                    Name = "Http Error Requests",
                    MeasurementUnit = Unit.Requests,
                };
            }

            /// <summary>
            /// Таймеры метрик.
            /// </summary>
            public static class Timers
            {
                /// <summary>
                /// Таймер конечных точек на запрос.
                /// </summary>
                public static Func<string, TimerOptions> EndpointPerRequestTimer = routeTemplate => new TimerOptions
                {
                    Context = ContextName,
                    Name = routeTemplate,
                    MeasurementUnit = Unit.Requests,
                };

                /// <summary>
                /// Таймер запросов.
                /// </summary>
                public static TimerOptions WebRequestTimer = new TimerOptions
                {
                    Context = ContextName,
                    Name = "Http Requests",
                    MeasurementUnit = Unit.Requests,
                };
            }
        }
    }
}