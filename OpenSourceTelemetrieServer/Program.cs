using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using OpenSourceTelemetrieData.Model.Types;
using Tfres;

namespace OpenSourceTelemetrieServer
{
  class Program
  {
    private static readonly int _max = 5 * 1024 * 1024;

    static void Main(string[] args)
    {
      var ip = GetIp(args);
      var port = GetPort(args);

      Console.Write($"OpenSourceTelemetrieServer http://{ip}:{port}...");

      if (Directory.Exists("data"))
        Directory.CreateDirectory("data");

      var server = new Server(ip, port, DefaultRoute);
      server.AddEndpoint(HttpVerb.POST, "/public/", SendAppcrash);
      server.AddEndpoint(HttpVerb.POST, "/telemetrie/", SendTelemetrie);

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
      => new HttpResponse(arg, false, 503, null, "text/plain", null);

    private static HttpResponse SendAppcrash(HttpRequest arg)
    {
      return StoreData(arg);
    }

    private static HttpResponse SendTelemetrie(HttpRequest arg)
    {
      if (arg.PostDataAsByteArray.Length > _max)
        return new HttpResponse(arg, false, 503, null, "text/plain", null);

      return StoreData(arg);
    }

    private static HttpResponse StoreData(HttpRequest arg)
    {
      File.WriteAllText($"data/{Guid.NewGuid():N}.json", arg.PostDataAsString.Replace("[SERVER_TIMESTAMP]", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")), Encoding.UTF8);
      return new HttpResponse(arg, true, 200, null, "text/plain", null);
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
