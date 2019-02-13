using System;
using System.Collections.Generic;
using OpenSourceTelemetrieData.Model;
using OpenSourceTelemetrieData.Model.Types;
using Exception = System.Exception;
using Metric = OpenSourceTelemetrieData.Model.Metric;

namespace OpenSourceTelemetrieClient
{
  public static class TelemetrieClient
  {
    private static string _url;
    private static Location _location;
    private static Device _device;
    private static string _anonId;
    private static string _session;

    /// <summary>
    /// Initialize the TelemetrieClient
    /// </summary>
    /// <param name="anonId">A random anonId - user specific</param>
    /// <param name="ip">IP where you want to send the telemetrie data</param>
    /// <param name="port">Open Network Port</param>
    /// <param name="country">Country of the user</param>
    /// <param name="city">City of the user</param>
    public static void Init(string anonId, string ip, short port, string country, string city)
    {
      _session = Guid.NewGuid().ToString("N");
      _anonId = anonId;
      _url = $"http://{ip}:{port}/";
      ChangeLocation(country, city);
      _device = new Device
      {
        Cores = Environment.ProcessorCount,
        OsVersion = Environment.OSVersion.ToString(),
        RAM = -1, // ToDo: Aktuell nicht möglich mit .NET Core
        ScreenResolution = null // ToDo: Aktuell nicht möglich mit .NET Core
      };
    }

    /// <summary>
    /// Change current location
    /// </summary>
    /// <param name="country">Country of the user</param>
    /// <param name="city">City of the user</param>
    public static void ChangeLocation(string country, string city)
    {
      _location = new Location { Country = country, City = city };
    }

    /// <summary>
    /// Sends an anonymized crash report
    /// </summary>
    /// <param name="exception">Exception</param>
    public static void SendPublicCrashReport(Exception exception)
    {
      var data = GenerateException(exception);
      // Anonymize
      data.AnonId = null;
      data.Location = null;
      data.Device = null;
      data.SessionId = null;
      data.Send(_url, "public/");
    }

    /// <summary>
    /// Sends an exception to the telemetrie server
    /// </summary>
    /// <param name="exception"></param>
    public static void SendTelemetrie(Exception exception)
    {
      GenerateException(exception).Send(_url, "telemetrie/");
    }

    /// <summary>
    /// Sends a pageView
    /// </summary>
    /// <param name="pageView">PageView</param>
    public static void SendTelemetrie(string pageView)
    {
      new PageView
      {
        AnonId = _anonId,
        Device = _device,
        EventId = Guid.NewGuid().ToString("N"),
        EventTime = DateTime.Now,
        Location = _location,
        SessionId = _session,
        Name = pageView
      }.Send(_url, "telemetrie/");
    }

    /// <summary>
    /// Sends a single metric
    /// </summary>
    /// <param name="event">Event name</param>
    /// <param name="time">ellapsed time</param>
    public static void SendTelemetrie(string @event, double time)
    {
      new Metric
      {
        AnonId = _anonId,
        Device = _device,
        EventId = Guid.NewGuid().ToString("N"),
        EventTime = DateTime.Now,
        Location = _location,
        SessionId = _session,
        Metrics = new[]{new OpenSourceTelemetrieData.Model.Types.Metric
        {
          Key = @event,
          Value = time
        }}
      }.Send(_url, "telemetrie/");
    }

    /// <summary>
    /// Sends multi metrics at once
    /// </summary>
    /// <param name="multiMetrics">Multi metrics</param>
    public static void SendTelemetrie(Dictionary<string, double> multiMetrics)
    {
      new Metric
      {
        AnonId = _anonId,
        Device = _device,
        EventId = Guid.NewGuid().ToString("N"),
        EventTime = DateTime.Now,
        Location = _location,
        SessionId = _session,
        Metrics = GenerateMetrics(multiMetrics)
      }.Send(_url, "telemetrie/");
    }

    private static IList<OpenSourceTelemetrieData.Model.Types.Metric> GenerateMetrics(Dictionary<string, double> multiMetrics)
    {
      var res = new List<OpenSourceTelemetrieData.Model.Types.Metric>();
      foreach (var m in multiMetrics)
      {
        res.Add(new OpenSourceTelemetrieData.Model.Types.Metric { Key = m.Key, Value = m.Value });
      }
      return res;
    }

    private static OpenSourceTelemetrieData.Model.Exception GenerateException(Exception exception)
    {
      return new OpenSourceTelemetrieData.Model.Exception
      {
        AnonId = _anonId,
        Location = _location,
        Device = _device,
        EventId = Guid.NewGuid().ToString("N"),
        EventTime = DateTime.Now,
        Name = exception?.GetType().FullName,
        SessionId = _session,
        BasicException = GenerateBasicException(exception)
      };
    }

    private static IList<BasicException> GenerateBasicException(Exception exception)
    {
      if (exception == null)
        return null;

      var res = new List<BasicException>
      {
        new BasicException
        {
          Assembly = exception.Source,
          ExceptionType = exception.GetType()?.FullName,
          Message = exception.Message,
          StackTrace = exception.StackTrace,
          Method = exception.TargetSite?.Name
        }
      };

      if (exception.InnerException != null)
        res.AddRange(GenerateBasicException(exception.InnerException));

      return res;
    }
  }
}
