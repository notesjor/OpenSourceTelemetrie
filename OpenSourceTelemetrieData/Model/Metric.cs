using System.Collections.Generic;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Abstract;
using OpenSourceTelemetrieData.Model.Types;

namespace OpenSourceTelemetrieData.Model
{
  public class Metric : AbstractEvent
  {
    [JsonProperty("metrics")]
    public IList<Types.Metric> Metrics { get; set; }
  }
}