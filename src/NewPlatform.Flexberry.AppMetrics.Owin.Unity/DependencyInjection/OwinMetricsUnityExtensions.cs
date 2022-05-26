using Owin;
using Unity;

namespace NewPlatform.Flexberry.AppMetrics.Owin
{
    using System;
    using App.Metrics;
    using NewPlatform.Flexberry.AppMetrics.Owin.Middleware;
    using NewPlatform.Flexberry.AppMetrics.Owin.Options;

    /// <summary>
    /// Unity расширения.
    /// </summary>
    public static class OwinMetricsUnityExtensions
    {
        /// <summary>
        /// Зарегистрировать реализацию сервисов сбора метрик.
        /// </summary>
        /// <param name="container">Unity - контейнер.</param>
        /// <param name="onMetricsBuild">Делегат конфигурации метрик.</param>
        /// <param name="options">Опции интеграции метрик OWIN.</param>
        public static void AddMetrics(this IUnityContainer container, Action<MetricsBuilder> onMetricsBuild, OwinMetricsOptions options)
        {
            var metricsBuilder = new MetricsBuilder();

            if (onMetricsBuild == null)
            {
                throw new ArgumentNullException(nameof(onMetricsBuild));
            }

            onMetricsBuild(metricsBuilder);
            var metrics = metricsBuilder.Build();
            container.RegisterInstance(options);
            container.RegisterInstance(metrics.DefaultOutputMetricsFormatter);
            container.RegisterInstance<IMetrics>(metrics);
            container.RegisterInstance<IMetricsRoot>(metrics);
        }

        /// <summary>
        /// Зарегистрировать реализацию сервисов сбора метрик.
        /// </summary>
        /// <param name="container">Unity - контейнер.</param>
        /// <param name="onMetricsBuild">Делегат конфигурации метрик.</param>
        public static void AddMetrics(this IUnityContainer container, Action<MetricsBuilder> onMetricsBuild)
        {
            container.AddMetrics(onMetricsBuild, new OwinMetricsOptions());
        }

        /// <summary>
        /// Зарегистрировать реализацию сервисов сбора метрик.
        /// </summary>
        /// <param name="container">Unity - контейнер.</param>
        /// <param name="options">Опции интеграции метрик OWIN.</param>
        public static void AddMetrics(this IUnityContainer container, OwinMetricsOptions options)
        {
            container.AddMetrics(x => { }, options);
        }

        /// <summary>
        /// Зарегистрировать реализацию сервисов сбора метрик.
        /// </summary>
        /// <param name="container">Unity - контейнер.</param>
        public static void AddMetrics(this IUnityContainer container)
        {
            container.AddMetrics(x => { });
        }

        /// <summary>
        /// Активировать обработчики метрик.
        /// </summary>
        /// <param name="app">Объект конфигурации приложения.</param>
        /// <param name="container">Unity - контейнер.</param>
        /// <returns>Объект конфигурации приложения.</returns>
        public static IAppBuilder UseMetrics(this IAppBuilder app, IUnityContainer container)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var options = container.Resolve<OwinMetricsOptions>();

            if (options == null)
            {
                throw new ArgumentNullException(nameof(OwinMetricsOptions));
            }

            var metricOptions = container.Resolve<IMetricsRoot>();

            if (metricOptions == null)
            {
                throw new ArgumentNullException(nameof(MetricsOptions));
            }

            if (metricOptions.Options.Enabled)
            {
                if (options.ErrorRequestMeterEnabled)
                {
                    app.Use(container.Resolve<ErrorRequestMeterMiddleware>());
                }

                if (options.PingEndpointEnabled)
                {
                    app.Use(container.Resolve<PingEndpointMiddleware>());
                }

                if (options.RequestTimerEnabled)
                {
                    app.Use(container.Resolve<RequestTimerMiddleware>());
                }

                if (options.PerRequestTimerEnabled)
                {
                    app.Use(container.Resolve<PerRequestTimerMiddleware>());
                }

                if (options.PostAndPutRequestSizeEnabled)
                {
                    app.Use(container.Resolve<PostAndPutRequestSizeHistogramMiddleware>());
                }

                if (options.ApdexTrackingEnabled)
                {
                    app.Use(container.Resolve<ApdexMiddleware>());
                }

                if (options.MetricsEndpointEnabled)
                {
                    app.Use(container.Resolve<MetricsEndpointMiddleware>());
                }

                if (options.ActiveRequestCounterEnabled)
                {
                    app.Use(container.Resolve<ActiveRequestCounterEndpointMiddleware>());
                }
            }

            return app;
        }
    }
}
