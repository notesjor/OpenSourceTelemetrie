using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class Location
  {
    [JsonProperty("city")]
    public string City { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }
  }
}