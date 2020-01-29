using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenSourceTelemetrieData.Model;
using OpenSourceTelemetrieData.Model.Types;
using Exception = System.Exception;

namespace OpenSourceTelemetrieClient
{
  public class TelemetrieClient : IDisposable
  {
    private readonly string _anonId;
    private readonly Device _device;
    private readonly Queue<Exceptions> _exceptions = new Queue<Exceptions>();
    private readonly Queue<Metrics> _metrics = new Queue<Metrics>();
    private readonly Queue<PageView> _pageViews = new Queue<PageView>();
    private readonly object _queueLock = new object();
    private readonly string _session;
    private readonly string _url;
    private Location _location;
    private Task _flushTask;

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
    ///   Automatic Flush after these amount (exceptions + metrics + pageViews)
    /// </summary>
    public int AutoFlushValue { get; set; } = 50;

    public void Dispose()
    {
      Flush();
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
        lock (_queueLock)
        {
          _exceptions.Enqueue(GenerateException(exception));
        }

        CheckFlush();
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends an exception to the telemetrie server async
    /// </summary>
    /// <param name="exception"></param>
    public async Task SendTelemetrieAsync(Exception exception)
    {
      try
      {
        await GenerateException(exception).Send(_url, "exception/");
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
        lock (_queueLock)
        {
          _pageViews.Enqueue(new PageView
          {
            AnonId = _anonId,
            Device = _device,
            EventId = Guid.NewGuid().ToString("N"),
            EventTime = DateTime.Now,
            Location = _location,
            SessionId = _session,
            Name = pageView
          });
        }

        CheckFlush();
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends a pageView async
    /// </summary>
    /// <param name="pageView">PageView</param>
    public async Task SendTelemetrieAsync(string pageView)
    {
      try
      {
        await new PageView
        {
          AnonId = _anonId,
          Device = _device,
          EventId = Guid.NewGuid().ToString("N"),
          EventTime = DateTime.Now,
          Location = _location,
          SessionId = _session,
          Name = pageView
        }.Send(_url, "pageview/");
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
        lock (_queueLock)
        {
          _metrics.Enqueue(new Metrics
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
          });
        }

        CheckFlush();
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Sends a single metric async
    /// </summary>
    /// <param name="event">Event name</param>
    /// <param name="time">ellapsed time</param>
    public async Task SendTelemetrieAsync(string @event, double time)
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
          Values = new[]
          {
            new Metric
            {
              Key = @event,
              Value = time
            }
          }
        }.Send(_url, "metric/");
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
        lock (_queueLock)
        {
          _metrics.Enqueue(new Metrics
          {
            AnonId = _anonId,
            Device = _device,
            EventId = Guid.NewGuid().ToString("N"),
            EventTime = DateTime.Now,
            Location = _location,
            SessionId = _session,
            Values = GenerateMetrics(multiMetrics)
          });
        }

        CheckFlush();
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

    private void CheckFlush()
    {
      if (_exceptions.Count + _metrics.Count + _pageViews.Count > AutoFlushValue)
        Flush();
    }

    /// <summary>
    ///   Sends all waiting telemetric data to the server
    /// </summary>
    /// <returns>Task - done?</returns>
    private void Flush()
    {
      var tasks = new List<Task>();

      lock (_queueLock)
      {
        while (_metrics.Count > 0)
          tasks.Add(_metrics.Dequeue().Send(_url, "metric/"));
        while (_pageViews.Count > 0)
          tasks.Add(_pageViews.Dequeue().Send(_url, "pageview/"));
        while (_exceptions.Count > 0)
          tasks.Add(_exceptions.Dequeue().Send(_url, "exception/"));
      }

      if (_flushTask != null && _flushTask.Status == TaskStatus.Running)
        _flushTask.Wait();

      _flushTask = Task.WhenAll(tasks);
    }
  }
}