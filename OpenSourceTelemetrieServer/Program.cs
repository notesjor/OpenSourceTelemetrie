using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using OpenSourceTelemetrieData.Model.Db;
using OpenSourceTelemetrieData.Model.Types;
using Tfres;

namespace OpenSourceTelemetrieServer
{
  class Program
  {
    private static readonly int _max = 5 * 1024 * 1024;
    private static IpDataContext _context = new IpDataContext("Data Source=ip4.db;");

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
      return StoreData(arg, "", "");
    }

    private static HttpResponse SendTelemetrie(HttpRequest arg)
    {
      if (arg.PostDataAsByteArray.Length > _max)
        return new HttpResponse(arg, false, 503, null, "text/plain", null);

      ResolveCountryAndCityName(arg.SourceIp, out var country, out var city);

      return StoreData(arg, country, city);
    }

    private static void ResolveCountryAndCityName(string ip, out string country, out string city)
    {
      country = "";
      city = "";

      if (string.IsNullOrEmpty(ip) || ip.Contains(":"))
        return;

      var split = ip.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length != 4)
        return;

      var i = split.Select(byte.Parse).ToArray();
      var entry = (from x in _context.LocationEntries
                   where
                     x.IP1S >= i[0] && i[0] <= x.IP1E &&
                     x.IP2S >= i[1] && i[1] <= x.IP2E &&
                     x.IP3S >= i[2] && i[2] <= x.IP3E &&
                     x.IP4S >= i[3] && i[3] <= x.IP4E
                   select x).FirstOrDefault();
      if(entry == null)
        return;

      country = entry.Country;
      city = entry.City;
    }

    private static HttpResponse StoreData(HttpRequest arg, string country, string city)
    {
      var dt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
      var id = Guid.NewGuid();

      File.WriteAllText($"data/{id:N}.json", arg.PostDataAsString
                                                .Replace("[SERVER_TIMESTAMP]", dt)
                                                .Replace("[SERVER_CN]", country)
                                                .Replace("[SERVER_CT]", city), Encoding.UTF8);
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
