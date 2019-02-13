using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CsvHelper;
using CsvHelper.Configuration;
using OpenSourceTelemetrieData.Model.Db;
using OpenSourceTelemetrieData.Model.Types;

namespace BuildIpCountryDatabase
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("1. Download IP-DB from: https://db-ip.com/db/download/ip-to-city-lite");
      Console.WriteLine("2. Extract DB and rename it to: dbip.csv");
      Console.WriteLine("3. PRESS ENTER");
      Console.ReadLine();

      var entries = new List<LocationEntry>();

      using (var reader = new StreamReader("dbip.csv"))
      using (var csv = new CsvReader(reader, new Configuration { Delimiter = ",", HasHeaderRecord = false }))
      {
        while (csv.Read())
        {
          try
          {
            var ip1 = csv.GetField<string>(0);
            var ip2 = csv.GetField<string>(1);
            if (ip1.Contains(":") || ip2.Contains(":"))
              continue;

            var start = ip1.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            var stop = ip2.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if(start.Length != 4 || stop.Length != 4)
              continue;

            var country = csv.GetField<string>(3);
            var city = csv.GetField<string>(5);

            entries.Add(new LocationEntry
            {
              Country = country, 
              City = city,
              IP1S = byte.Parse(start[0]),
              IP2S = byte.Parse(start[1]),
              IP3S = byte.Parse(start[2]),
              IP4S = byte.Parse(start[3]),
              IP1E = byte.Parse(stop[0]),
              IP2E = byte.Parse(stop[1]),
              IP3E = byte.Parse(stop[2]),
              IP4E = byte.Parse(stop[3])
            });
          }
          catch
          {
            // ignore
          }
        }
      }

      var context = new IpDataContext($"Data Source=ip4.db;FailIfMissing=False;License Key={File.ReadAllText("devart.key")};");
      context.CreateDatabase(true, true);
      context.LocationEntries.InsertAllOnSubmit(entries);
      context.SubmitChanges();
    }
  }
}
