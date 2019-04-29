using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace OpenSourceTelemetrieESearchUpload
{
  class Program
  {
    static void Main(string[] args)
    {
      if (!Directory.Exists("data"))
      {
        Console.WriteLine("Das Verzeichnis data exsistiert nicht. Es wurde angelegt. Bitte hinterlegen Sie die JSON-Dateien und starten Sie die Anwendung neu.");
        Directory.CreateDirectory("data");
        Console.ReadLine();
        return;
      }

      var client = Initialize();
      var subDirs = Directory.GetDirectories("data");
      foreach (var subDir in subDirs)
      {
        Console.WriteLine($"{subDir}...");
        var name = subDir.Replace("data\\", "");
        var files = Directory.GetFiles(subDir, "*.json");
        foreach (var file in files)
        {
          Console.Write($"{file}...");
          try
          {
            var resp = client.LowLevel.Index<Response>($"ost-{name}", name, File.ReadAllText(file, Encoding.UTF8));
            Console.WriteLine(resp.Success ? "ok!" : "error!!!");
          }
          catch
          {
            Console.WriteLine("error!!!");
          }
        }
      }
    }

    public class Response : ElasticsearchResponse<string> { }

    public class ResponseBool : ElasticsearchResponse<bool> { }

    public static ElasticClient Initialize()
    {
      var settings = new ConnectionSettings(new StaticConnectionPool(new[] { "http://localhost:9200" }.Select(x => new Uri(x))));
      settings.DefaultIndex("ost");
      settings.DisableDirectStreaming();
      return new ElasticClient(settings);
    }
  }
}
