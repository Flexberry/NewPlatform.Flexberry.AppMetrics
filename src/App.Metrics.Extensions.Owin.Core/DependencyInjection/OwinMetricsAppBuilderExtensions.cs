// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics
{
    using Extensions.Owin.Options;
    using Extensions.Owin.Middleware;
    using Microsoft.Extensions.DependencyInjection;
    using Owin;
    using System;
    using System.Threading.Tasks;
    using System.Web.Hosting;

    public static class OwinMetricsAppBuilderExtensions
    {

        /// <summary>
        /// ���������������� ���������� �������� ����� ������.
        /// </summary>
        /// <param name="services">��������� ��������.</param>
        /// <param name="onMetricsBuild">������� ������������ ������.</param>
        /// <param name="options">����� ���������� ������ OWIN.</param>
        public static void AddMetrics(this IServiceCollection services, Action<MetricsBuilder> onMetricsBuild, OwinMetricsOptions options)
        {
            var metricsBuilder = new MetricsBuilder();
            onMetricsBuild(metricsBuilder);
            var metrics = metricsBuilder.Build();
            services.AddSingleton(options);
            services.AddSingleton(metrics.DefaultOutputMetricsFormatter);
            services.AddSingleton<IMetrics>(metrics);
            services.AddSingleton<IMetricsRoot>(metrics);
        }

        /// <summary>
        /// ���������������� ���������� �������� ����� ������.
        /// </summary>
        /// <param name="services">��������� ��������.</param>
        /// <param name="onMetricsBuild">������� ������������ ������.</param>
        public static void AddMetrics(this IServiceCollection services, Action<MetricsBuilder> onMetricsBuild)
        {
            services.AddMetrics(onMetricsBuild, new OwinMetricsOptions());
        }

        /// <summary>
        /// ���������������� ���������� �������� ����� ������.
        /// </summary>
        /// <param name="services">��������� ��������.</param>
        /// <param name="options">����� ���������� ������ OWIN.</param>
        public static void AddMetrics(this IServiceCollection services, OwinMetricsOptions options)
        {
            services.AddMetrics(x => { }, options);
        }

        /// <summary>
        /// ���������������� ���������� �������� ����� ������.
        /// </summary>
        /// <param name="services">��������� ��������.</param>
        public static void AddMetrics(this IServiceCollection services)
        {
            services.AddMetrics(x => { });
        }

        /// <summary>
        /// ������������ ����������� ������.
        /// </summary>
        /// <param name="app">������ ������������ ����������.</param>
        /// <param name="provider">��������� ��������.</param>
        /// <returns>������ ������������ ����������.</returns>
        public static IAppBuilder UseMetrics(this IAppBuilder app, IServiceProvider provider)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var options = provider.GetRequiredService<OwinMetricsOptions>();
            var metricOptions = provider.GetRequiredService<IMetricsRoot>();

            if (metricOptions.Options.Enabled)
            {
                if (options.ErrorRequestMeterEnabled)
                {
                    app.Use(provider.GetRequiredService<ErrorRequestMeterMiddleware>());
                }

                if (options.PingEndpointEnabled)
                {
                    app.Use(provider.GetRequiredService<PingEndpointMiddleware>());
                }

                if (options.RequestTimerEnabled)
                {
                    app.Use(provider.GetRequiredService<RequestTimerMiddleware>());
                }

                if (options.PerRequestTimerEnabled)
                {
                    app.Use(provider.GetRequiredService<PerRequestTimerMiddleware>());
                }

                if (options.PostAndPutRequestSizeEnabled)
                {
                    app.Use(provider.GetRequiredService<PostAndPutRequestSizeHistogramMiddleware>());
                }

                if (options.ApdexTrackingEnabled)
                {
                    app.Use(provider.GetRequiredService<ApdexMiddleware>());
                }

                if (options.MetricsEndpointEnabled)
                {
                    app.Use(provider.GetRequiredService<MetricsEndpointMiddleware>());
                }

                if (options.ActiveRequestCounterEnabled)
                {
                    app.Use(provider.GetRequiredService<ActiveRequestCounterEndpointMiddleware>());
                }
            }

            return app;
        }

        /// <summary>
        /// ������������ �������� ������.
        /// </summary>
        /// <param name="app">������ ������������ ����������.</param>
        /// <param name="provider">��������� ��������.</param>
        /// <returns></returns>
        public static IAppBuilder UseMetricsReporting(this IAppBuilder app, IServiceProvider provider)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                var metricsRoot = provider.GetRequiredService<IMetricsRoot>();
                await Task.WhenAll(metricsRoot.ReportRunner.RunAllAsync(cancellationToken));
            });

            return app;
        }
    }
}