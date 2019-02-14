using System;
using System.IO;
using System.Text;
using Tfres;

namespace OpenSourceTelemetrieServer
{
  public class Program
  {
    private static readonly int _max = 64 * 1024 * 1024;

    public static void Main(params string[] args)
    {
      var ip = GetIp(args);
      var port = GetPort(args);

      Console.Write($"OpenSourceTelemetrieServer http://{ip}:{port}...");

      if (!Directory.Exists("data"))
        Directory.CreateDirectory("data");
      if (!Directory.Exists("data/crashreport"))
        Directory.CreateDirectory("data/crashreport");
      if (!Directory.Exists("data/exception"))
        Directory.CreateDirectory("data/exception");
      if (!Directory.Exists("data/pageview"))
        Directory.CreateDirectory("data/pageview");
      if (!Directory.Exists("data/metric"))
        Directory.CreateDirectory("data/metric");

      var server = new Server(ip, port, DefaultRoute);
      server.AddEndpoint(HttpVerb.POST, "/crashreport/", SendCrashReport);
      server.AddEndpoint(HttpVerb.POST, "/exception/", SendException);
      server.AddEndpoint(HttpVerb.POST, "/pageview/", SendPageView);
      server.AddEndpoint(HttpVerb.POST, "/metric/", SendMetric);

      Console.WriteLine("ok!");
      while (true)
      {
        var command = Console.ReadLine();
        if (command == "exit" || command == "quit")
          break;
      }

      server.Dispose();
    }

    private static HttpResponse DefaultRoute(HttpRequest arg)
    {
      return new HttpResponse(arg, false, 503, null, "text/plain", null);
    }

    private static HttpResponse SendCrashReport(HttpRequest arg)
    {
      return arg.PostDataAsByteArray.Length > _max
               ? new HttpResponse(arg, false, 503, null, "text/plain", null)
               : StoreData(arg, "crashreport");
    }

    private static HttpResponse SendException(HttpRequest arg)
    {
      return arg.PostDataAsByteArray.Length > _max
               ? new HttpResponse(arg, false, 503, null, "text/plain", null)
               : StoreData(arg, "exception");
    }

    private static HttpResponse SendPageView(HttpRequest arg)
    {
      return arg.PostDataAsByteArray.Length > _max
               ? new HttpResponse(arg, false, 503, null, "text/plain", null)
               : StoreData(arg, "pageview");
    }

    private static HttpResponse SendMetric(HttpRequest arg)
    {
      return arg.PostDataAsByteArray.Length > _max
               ? new HttpResponse(arg, false, 503, null, "text/plain", null)
               : StoreData(arg, "metric");
    }

    private static HttpResponse StoreData(HttpRequest arg, string folder)
    {
      try
      {
        File.WriteAllText($"data/{folder}/{Guid.NewGuid():N}.json", arg.PostDataAsString, Encoding.UTF8);
        return new HttpResponse(arg, true, 200, null, "text/plain", null);
      }
      catch
      {
        return new HttpResponse(arg, false, 503, null, "text/plain", null);
      }
    }

    private static string GetIp(string[] args)
    {
      if (args == null || args.Length == 0 || !args[0].StartsWith("--ip:"))
        return "127.0.0.1";

      try
      {
        return args[0].Replace("--ip:", "");
      }
      catch
      {
        return "127.0.0.1";
      }
    }

    private static int GetPort(string[] args)
    {
      if (args == null || args.Length == 0 || !args[0].StartsWith("--port:"))
        return 8512;

      try
      {
        return short.Parse(args[0].Replace("--port:", ""));
      }
      catch
      {
        return 8512;
      }
    }
  }
}