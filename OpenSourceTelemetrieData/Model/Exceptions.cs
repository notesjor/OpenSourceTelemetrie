using System.Collections.Generic;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Abstract;
using OpenSourceTelemetrieData.Model.Types;

namespace OpenSourceTelemetrieData.Model
{
  public class Exceptions : AbstractEvent
  {
    [JsonProperty("children")]
    public IList<Exception> Children { get; set; }
  }
}