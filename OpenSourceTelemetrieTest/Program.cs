﻿using System;
using System.Collections.Generic;
using OpenSourceTelemetrieClient;

namespace OpenSourceTelemetrieTest
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var client = new TelemetrieClient("xy", "127.0.0.1", 8512, "Germany", "Siegen");
      client.SendPublicCrashReport(new Exception());

      var count = 0;
      for (var i = 0; i < 20; i++)
      {
        client.SendTelemetrie(new Exception());
        client.SendTelemetrie("page");
        client.SendTelemetrie("page", 5.9);
        client.SendTelemetrie(new Dictionary<string, double> {{"a", 8.1}, {"b", 5.9}});
        count += 4;
        if (count > 50)
        {
          Console.WriteLine("FLUSH");
          count = 0;
        }
      }
    }
  }
}