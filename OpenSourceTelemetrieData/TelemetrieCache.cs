using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Abstract;

namespace OpenSourceTelemetrieData
{
  public static class TelemetrieCache
  {
    private static List<string> _cache = new List<string>();
    private static object @lock = new object();

    public static int AutoflushEventCount { get; set; } = 100;

    public static async void Add(AbstractEvent obj)
    {
      var task = new Task(() =>
      {
        try
        {
          var str = JsonConvert.SerializeObject(obj);
          int cnt;
          lock (@lock)
          {
            _cache.Add(str);
            cnt = _cache.Count;
          }

          if (cnt >= AutoflushEventCount)
            Flush();
        }
        catch
        {
          // ignore
        }
      });
      task.Start();
      await task;
    }

    public static async void Flush()
    {
      lock (@lock)
      {

      }
    }
  }
}