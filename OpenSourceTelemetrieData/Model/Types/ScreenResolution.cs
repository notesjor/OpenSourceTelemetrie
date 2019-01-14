using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class ScreenResolution
  {

    [JsonProperty("Height")]
    public int Height { get; set; }

    [JsonProperty("HighDPI")]
    public bool HighDPI { get; set; }

    [JsonProperty("Width")]
    public int Width { get; set; }
  }
}
