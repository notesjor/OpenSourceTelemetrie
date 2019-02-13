using System;
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

    [JsonProperty("eventServerTime")]
    public DateTime EventServerTime { get; set; }

    [JsonProperty("location")]
    public Location Location { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }

    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("sessionId")]
    public string SessionId { get; set; }
  }
}