using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenSourceTelemetrieData.Model;
using OpenSourceTelemetrieData.Model.Types;
using Exception = System.Exception;

namespace OpenSourceTelemetrieClient
{
  public class TelemetrieClient
  {
    private readonly string _anonId;
    private readonly Device _device;
    private readonly string _session;
    private readonly string _url;
    private Location _location;

    /// <summary>
    ///   Initialize the TelemetrieClient
    /// </summary>
    /// <param name="anonId">A random anonId - user specific</param>
    /// <param name="ip">IP where you want to send the telemetrie data</param>
    /// <param name="port">Open Network Port</param>
    /// <param name="country">Country of the user</param>
    /// <param name="city">City of the user</param>
    public TelemetrieClient(string anonId, string ip, short port, string country, string city)
    {
      try
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
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Change current location
    /// </summary>
    /// <param name="country">Country of the user</param>
    /// <param name="city">City of the user</param>
    public void ChangeLocation(string country, string city)
    {
      try
      {
        _location = new Location { Country = country, City = city };
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends an anonymized crash report
    /// </summary>
    /// <param name="exception">Exception</param>
    public async Task SendPublicCrashReport(Exception exception)
    {
      try
      {
        var data = GenerateException(exception);
        // Anonymize
        data.AnonId = null;
        data.Location = null;
        data.Device = null;
        data.SessionId = null;
        await data.Send(_url, "crashreport/");
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends an exception to the telemetrie server
    /// </summary>
    /// <param name="exception"></param>
    public void SendTelemetrie(Exception exception)
    {
      try
      {
        GenerateException(exception)?.Send(_url, "exception/");
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends a pageView
    /// </summary>
    /// <param name="pageView">PageView</param>
    public void SendTelemetrie(string pageView)
    {
      try
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
        }?.Send(_url, "pageview/");
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends a single metric
    /// </summary>
    /// <param name="event">Event name</param>
    /// <param name="time">ellapsed time</param>
    public void SendTelemetrie(string @event, double time)
    {
      try
      {
        new Metrics
        {
          AnonId = _anonId,
          Device = _device,
          EventId = Guid.NewGuid().ToString("N"),
          EventTime = DateTime.Now,
          Location = _location,
          SessionId = _session,
          Values = new[]
          {
            new Metric
            {
              Key = @event,
              Value = time
            }
          }
        }?.Send(_url, "metric/");
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends multi metrics at once
    /// </summary>
    /// <param name="multiMetrics">Multi metrics</param>
    public void SendTelemetrie(Dictionary<string, double> multiMetrics)
    {
      try
      {
        new Metrics
        {
          AnonId = _anonId,
          Device = _device,
          EventId = Guid.NewGuid().ToString("N"),
          EventTime = DateTime.Now,
          Location = _location,
          SessionId = _session,
          Values = GenerateMetrics(multiMetrics)
        }?.Send(_url, "metric/");
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends multi metrics at once async
    /// </summary>
    /// <param name="multiMetrics">Multi metrics</param>
    public async Task SendTelemetrieAsync(Dictionary<string, double> multiMetrics)
    {
      try
      {
        await new Metrics
        {
          AnonId = _anonId,
          Device = _device,
          EventId = Guid.NewGuid().ToString("N"),
          EventTime = DateTime.Now,
          Location = _location,
          SessionId = _session,
          Values = GenerateMetrics(multiMetrics)
        }.Send(_url, "metric/");
      }
      catch
      {
        // ignore
      }
    }

    private IList<Metric> GenerateMetrics(Dictionary<string, double> multiMetrics)
    {
      try
      {
        return multiMetrics.Select(m => new Metric { Key = m.Key, Value = m.Value }).ToList();
      }
      catch
      {
        return null;
      }
    }

    private Exceptions GenerateException(Exception exception)
    {
      try
      {
        return new Exceptions
        {
          AnonId = _anonId,
          Location = _location,
          Device = _device,
          EventId = Guid.NewGuid().ToString("N"),
          EventTime = DateTime.Now,
          Name = exception?.GetType().FullName,
          SessionId = _session,
          Children = GenerateBasicException(exception)
        };
      }
      catch
      {
        return null;
      }
    }

    private IList<OpenSourceTelemetrieData.Model.Types.Exception> GenerateBasicException(Exception exception)
    {
      try
      {
        if (exception == null)
          return null;

        var res = new List<OpenSourceTelemetrieData.Model.Types.Exception>
        {
          new OpenSourceTelemetrieData.Model.Types.Exception
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
      catch
      {
        return null;
      }
    }
  }
}