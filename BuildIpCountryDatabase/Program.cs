using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CsvHelper;
using CsvHelper.Configuration;
using OpenSourceTelemetrieData.Helper;
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

            var start = ip1.Convert();
            var stop = ip2.Convert();
            var country = csv.GetField<string>(3);
            var city = csv.GetField<string>(5);

            entries.Add(new LocationEntry { Start = start, Stop = stop, Country = country, City = city });
          }
          catch
          {
            // ignore
          }
        }
      }

      using (var fs = new FileStream("dbip.bin", FileMode.Create, FileAccess.Write))
      {
        var serializer = new BinaryFormatter();
        serializer.Serialize(fs, entries.OrderBy(x => x.Start).ToArray());
      }
    }
  }
}
