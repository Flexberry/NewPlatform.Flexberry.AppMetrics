﻿// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace NewPlatform.Flexberry.AppMetrics.Owin.Internal
{
    using App.Metrics;
    using App.Metrics.Apdex;
    using App.Metrics.Counter;
    using App.Metrics.Gauge;
    using App.Metrics.Histogram;
    using App.Metrics.Meter;
    using App.Metrics.Timer;
    using System;

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
            public static string ContextName = "Application.HttpRequests";

            /// <summary>
            /// Счетчики метрик APDEX.
            /// </summary>
            public static class ApdexScores
            {
                public static readonly string ApdexMetricName = "Apdex";

                public static Func<double, ApdexOptions> Apdex = apdexTSeconds => new ApdexOptions
                {
                    Context = ContextName,
                    Name = ApdexMetricName,
                    ApdexTSeconds = apdexTSeconds
                };
            }

            public static class Gauges
            {
                public static GaugeOptions PercentageErrorRequests = new GaugeOptions
                {
                    Context = ContextName,
                    Name = "Percentage Error Requests",
                    MeasurementUnit = Unit.Custom("Error Requests")
                };
            }

            public static class Counters
            {
                public static CounterOptions ActiveRequests = new CounterOptions
                {
                    Context = ContextName,
                    Name = "Active Requests",
                    MeasurementUnit = Unit.Custom("Active Requests")
                };
            }

            public static class Histograms
            {
                public static HistogramOptions PostAndPutRequestSize = new HistogramOptions
                {
                    Context = ContextName,
                    Name = "Http Request Post & Put Size",
                    MeasurementUnit = Unit.Bytes
                };
            }

            public static class Meters
            {
                public static Func<string, MeterOptions> EndpointHttpErrorRequests = routeTemplate => new MeterOptions
                {
                    Context = ContextName,
                    Name = $"{routeTemplate} Http Error Requests",
                    MeasurementUnit = Unit.Requests
                };

                public static MeterOptions HttpErrorRequests = new MeterOptions
                {
                    Context = ContextName,
                    Name = "Http Error Requests",
                    MeasurementUnit = Unit.Requests
                };
            }

            public static class Timers
            {
                public static Func<string, TimerOptions> EndpointPerRequestTimer = routeTemplate => new TimerOptions
                {
                    Context = ContextName,
                    Name = routeTemplate,
                    MeasurementUnit = Unit.Requests
                };

                public static TimerOptions WebRequestTimer = new TimerOptions
                {
                    Context = ContextName,
                    Name = "Http Requests",
                    MeasurementUnit = Unit.Requests
                };
            }
        }
    }
}