using System.Collections.Generic;
using Newtonsoft.Json;
using OpenSourceTelemetrieData.Model.Abstract;
using OpenSourceTelemetrieData.Model.Types;

namespace OpenSourceTelemetrieData.Model
{
  public class Exception : AbstractEvent
  {
    [JsonProperty("basicException")]
    public IList<BasicException> BasicException { get; set; }
  }
}