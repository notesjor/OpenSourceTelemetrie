using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSourceTelemetrieTest
{
  class Program
  {
    static void Main(string[] args)
    {
      OpenSourceTelemetrieClient.TelemetrieClient.Init("xy", "127.0.0.1", 8512, "Germany", "Siegen");
      OpenSourceTelemetrieClient.TelemetrieClient.SendPublicCrashReport(new Exception());
      OpenSourceTelemetrieClient.TelemetrieClient.SendTelemetrie("page");
      OpenSourceTelemetrieClient.TelemetrieClient.SendTelemetrie("page", 5.9);
    }
  }
}
