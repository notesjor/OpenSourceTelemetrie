using System.Collections.Generic;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Abstract;
using OpenSourceTelemetrieData.Model.Types;

namespace OpenSourceTelemetrieData.Model
{
  public class Metrics : AbstractEvent
  {
    [JsonProperty("values")]
    public IList<Metric> Values { get; set; }
  }
}