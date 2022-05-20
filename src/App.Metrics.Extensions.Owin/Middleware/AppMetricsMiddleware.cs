// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Metrics.Extensions.Owin.Middleware
{
    using Extensions;
    using Logging;
    using Options;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Базовый класс обработчиков метрик.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public abstract class AppMetricsMiddleware<TOptions> where TOptions : OwinMetricsOptions, new()
    {
        /// <summary>
        /// Делегат для оптимизации процедуры фильтрации обрабатываемых метрик.
        /// </summary>
        private readonly Func<string, bool> _shouldRecordMetric;

        /// <summary>
        /// Тип текущего обработчика.
        /// </summary>
        private string _middlewareType;

        protected AppMetricsMiddleware(TOptions options, IMetrics metrics)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            Options = options;

            Metrics = metrics;

            IReadOnlyList<Regex> ignoredRoutes = Options.IgnoredRoutesRegexPatterns
                .Select(p => new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase))
                .ToList();

            if (ignoredRoutes.Any())
            {
                _shouldRecordMetric = path => !ignoredRoutes.Any(ignorePattern => ignorePattern.IsMatch(path.ToString().RemoveLeadingSlash()));
            }
            else
            {
                _shouldRecordMetric = path => true;
            }

            _middlewareType = GetType().Name;
            Logger = LogProvider.GetLogger(_middlewareType);
        }

        /// <summary>
        /// Получить текущий роут.
        /// </summary>
        /// <param name="environment">Контекст Owin.</param>
        /// <param name="appendHttpMethod">Включать ли в роут название HTTP-метода (GET, POST, ...)</param>
        /// <returns>Роут./</returns>
        protected static string GetMetricsCurrentRouteName(IDictionary<string, object> environment, bool appendHttpMethod = false)
        {
            var path = environment["owin.RequestPath"].ToString().ToLower();
            if (!appendHttpMethod)
            {
                return path;
            }

            var httpMethod = environment["owin.RequestMethod"].ToString().ToUpper();
            return httpMethod + " " + path;
        }

        protected virtual void MiddlewareExecuted()
        {
            Logger.Debug($"Executed Owin Metrics Middleare {_middlewareType}");
        }

        protected virtual void MiddlewareExecuting()
        {
            Logger.Debug($"Executing Owin Metrics Middleare {_middlewareType}");
        }

        /// <summary>
        /// Логгер.
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// Объект для хвычисления и хранения метрик.
        /// </summary>
        public IMetrics Metrics { get; set; }

        /// <summary>
        /// Сссылка на следующий делегат.
        /// </summary>
        public AppFunc Next { get; set; }

        /// <summary>
        /// Настройки.
        /// </summary>
        public TOptions Options { get; set; }

        /// <summary>
        /// Метод инициализации, вызывается средой испонения Owin.
        /// </summary>
        /// <param name="next">Сссылка на следующий делегат.</param>
        public void Initialize(AppFunc next)
        {
            Next = next;
        }

        /// <summary>
        /// Определить выполнять ли сбор метрики для текущего вызова.
        /// </summary>
        /// <param name="environment">Контекст Owin.</param>
        /// <returns></returns>
        protected virtual bool ShouldPerformMetric(IDictionary<string, object> environment)
        {
            var requestPath = environment["owin.RequestPath"] as string;

            if (Options.IgnoredRoutesRegexPatterns == null)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(requestPath))
            {
                return false;
            }

            return _shouldRecordMetric(requestPath);
        }

        protected Task WriteResponseAsync(IDictionary<string, object> environment, string content, string contentType,
            HttpStatusCode code = HttpStatusCode.OK, string warning = null)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            var headers = environment["owin.ResponseHeaders"] as IDictionary<string, string[]>;
            var contentBytes = Encoding.UTF8.GetBytes(content);

            headers["Content-Type"] = new[] { contentType };
            headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
            headers["Pragma"] = new[] { "no-cache" };
            headers["Expires"] = new[] { "0" };

            if (warning.IsPresent())
            {
                headers["Warning"] = new[] { $"Warning: 100 '{warning}'" };
            }

            environment["owin.ResponseStatusCode"] = (int)code;
            return response.WriteAsync(contentBytes, 0, contentBytes.Length);
        }

    }
}