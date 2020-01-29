using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Types;

namespace OpenSourceTelemetrieData.Model.Abstract
{
  public class AbstractEvent
  {
    [JsonProperty("eventId")]
    public string EventId { get; set; }

    [JsonProperty("anonId")]
    public string AnonId { get; set; }

    [JsonProperty("device")]
    public Device Device { get; set; }

    [JsonProperty("eventTime")]
    public DateTime EventTime { get; set; }

    [JsonProperty("location")]
    public Location Location { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("sessionId")]
    public string SessionId { get; set; }

    public async Task Send(string url, string endpoint)
    {
      try
      {
        var httpWebRequest = (HttpWebRequest) WebRequest.Create(url + endpoint);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
          streamWriter.Write(JsonConvert.SerializeObject(this));
        }

        await httpWebRequest.GetResponseAsync();
      }
      catch
      {
        // ignore
      }
    }
  }
}