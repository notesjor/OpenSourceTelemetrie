using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class ParsedStack
  {

    [JsonProperty("assembly")]
    public string Assembly { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("method")]
    public string Method { get; set; }
  }
}