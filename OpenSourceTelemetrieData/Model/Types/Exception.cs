using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class Exception
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