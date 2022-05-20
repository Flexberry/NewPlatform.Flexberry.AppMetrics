// Copyright (c) Allan hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace App.Metrics.Extensions.Owin.Options
{
    using System.Collections.Generic;

    /// <summary>
    /// ��������� ������, ������������ ��� ���������� Owin.
    /// </summary>
    public class OwinMetricsOptions
    {
        /// <summary>
        /// �������� ���� ��������� APDEX.
        /// </summary>
        public bool ApdexTrackingEnabled { get; set; } = false;

        /// <summary>
        /// �������� ����� ��������� APDEX.
        /// </summary>
        public double ApdexTSeconds { get; set; } = 0.5;

        /// <summary>
        /// ������ ���������� ���������, �� ������� ������������ ���� �� �������������� ���������.
        /// </summary>
        public IList<string> IgnoredRoutesRegexPatterns { get; set; } = new List<string>();

        /// <summary>
        /// ���� �� �������� �������� �������.
        /// </summary>
        public string MetricsEndpoint { get; set; } = "/metrics";

        /// <summary>
        /// ������������ �������� ����� � ��������� �� ���� MetricsEndpoint.
        /// </summary>
        public bool MetricsEndpointEnabled { get; set; } = true;

        /// <summary>
        /// ���� �� �������� �������� ���-�����.
        /// </summary>
        public string PingEndpoint { get; set; } = "/ping";

        /// <summary>
        /// ������������ �������� ����� � ���-������� �� ���� PingEndpoint.
        /// </summary>
        public bool PingEndpointEnabled { get; set; } = false;

        /// <summary>
        /// ������������ ���������� ���������� ���������� �� ������� ���������� ���������� ��������. ��. PerRequestTimerOptions.
        /// </summary>
        public bool PerRequestTimerEnabled { get; set; } = false;

        /// <summary>
        /// ��������� ����� ��������� �� ��������� ������.
        /// </summary>
        public PerRequestTimerOptions PerRequestTimerOptions { get; set; }

        /// <summary>
        /// ������������ ���������� ���������� ���������� �� ������� ���������� ���� ��������.
        /// </summary>
        public bool RequestTimerEnabled { get; set; } = false;

        /// <summary>
        /// ������������ ���������� ���������� ������� ���� �������� PUT � POST.
        /// </summary>
        public bool PostAndPutRequestSizeEnabled { get; set; } = false;

        /// <summary>
        /// ������������ ���������� �������� ������������ ����������� ��������.
        /// </summary>
        public bool ActiveRequestCounterEnabled { get; set; } = true;

        /// <summary>
        /// ������������ ���������� ���������� ����� ������� ��������� ��������.
        /// </summary>
        public bool ErrorRequestMeterEnabled { get; set; } = false;
    }
}