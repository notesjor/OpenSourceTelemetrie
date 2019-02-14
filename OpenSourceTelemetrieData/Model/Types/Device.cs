using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class Device
  {
    [JsonProperty("Cores")]
    public int Cores { get; set; }

    [JsonProperty("osVersion")]
    public string OsVersion { get; set; }

    [JsonProperty("RAM")]
    public long RAM { get; set; }

    [JsonProperty("screenResolution")]
    public ScreenResolution ScreenResolution { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
  }
}