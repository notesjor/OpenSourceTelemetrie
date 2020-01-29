using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

    private static Task DefaultRoute(HttpContext arg)
    {
      return arg.Response.Send(HttpStatusCode.NotFound);
    }

    private static Task SendCrashReport(HttpContext arg)
    {
      var text = arg.Request.PostDataAsString;
      return text.Length > _max 
               ? arg.Response.Send(HttpStatusCode.InternalServerError) 
               : StoreData(text, arg, "crashreport");
    }

    private static Task SendException(HttpContext arg)
    {
      var text = arg.Request.PostDataAsString;
      return text.Length > _max
               ? arg.Response.Send(HttpStatusCode.InternalServerError)
               : StoreData(text, arg, "exception");
    }

    private static Task SendPageView(HttpContext arg)
    {
      var text = arg.Request.PostDataAsString;
      return text.Length > _max
               ? arg.Response.Send(HttpStatusCode.InternalServerError)
               : StoreData(text, arg, "pageview");
    }

    private static Task SendMetric(HttpContext arg)
    {
      var text = arg.Request.PostDataAsString;
      return text.Length > _max
               ? arg.Response.Send(HttpStatusCode.InternalServerError)
               : StoreData(text, arg, "metric");
    }

    private static Task StoreData(string text, HttpContext context, string folder)
    {
      try
      {
        File.WriteAllText($"data/{folder}/{Guid.NewGuid():N}.json", text, Encoding.UTF8);
        return context.Response.Send(200);
      }
      catch
      {
        return context.Response.Send(500);
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