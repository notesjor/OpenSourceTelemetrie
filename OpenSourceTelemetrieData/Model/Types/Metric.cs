using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class Metric
  {

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("value")]
    public double Value { get; set; }
  }
}