using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenSourceTelemetrieData.Model.Types
{
  public class BasicException
  {

    [JsonProperty("assembly")]
    public string Assembly { get; set; }

    [JsonProperty("exceptionGroup")]
    public string ExceptionGroup { get; set; }

    [JsonProperty("exceptionType")]
    public string ExceptionType { get; set; }

    [JsonProperty("failedUserCodeAssembly")]
    public string FailedUserCodeAssembly { get; set; }

    [JsonProperty("failedUserCodeMethod")]
    public string FailedUserCodeMethod { get; set; }

    [JsonProperty("hasFullStack")]
    public bool HasFullStack { get; set; }

    [JsonProperty("innermostExceptionMessage")]
    public string InnermostExceptionMessage { get; set; }

    [JsonProperty("innermostExceptionThrownAtAssembly")]
    public string InnermostExceptionThrownAtAssembly { get; set; }

    [JsonProperty("innermostExceptionThrownAtMethod")]
    public string InnermostExceptionThrownAtMethod { get; set; }

    [JsonProperty("innermostExceptionType")]
    public string InnermostExceptionType { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("method")]
    public string Method { get; set; }

    [JsonProperty("outerExceptionMessage")]
    public string OuterExceptionMessage { get; set; }

    [JsonProperty("outerExceptionThrownAtAssembly")]
    public string OuterExceptionThrownAtAssembly { get; set; }

    [JsonProperty("outerExceptionThrownAtMethod")]
    public string OuterExceptionThrownAtMethod { get; set; }

    [JsonProperty("outerExceptionType")]
    public string OuterExceptionType { get; set; }

    [JsonProperty("parsedStack")]
    public IList<ParsedStack> ParsedStack { get; set; }

    [JsonProperty("problemId")]
    public string ProblemId { get; set; }

    [JsonProperty("typeName")]
    public string TypeName { get; set; }
  }
}