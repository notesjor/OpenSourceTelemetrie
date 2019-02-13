using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class BasicException
  {

    [JsonProperty("assembly")]
    public string Assembly { get; set; }

    [JsonProperty("exceptionType")]
    public string ExceptionType { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("StackTrace")]
    public string StackTrace { get; set; }
  }
}