# OpenSourceTelemetrie

## Requierments
- Based on .NET Core 2.1 (https://dotnet.microsoft.com/download/dotnet-core/2.1)
- Runs on Windows, Linux and MacOS

### SERVER
- Run your own self hosted telemetric server - only 15MB RAM requiered
- Runs on Windows, Linux and MacOS

### CLIENT
- Reference OpenSourceTelemetrieClient.dll and OpenSourceTelemetrieData.dll in your .NET-Application
- create a new client by: '''var client = new OpenSourceTelemetrieClient.TelemetrieClient("userID", "127.0.0.1", 8512, "Germany", "Siegen");'''
- Send data by; '''client.SendTelemetrie(new Exception());''' or '''client.SendTelemetrie("calculation A+x", 5.9);'''
- To reduce the network interaction > the client collects multiple send-requests and sends them in a (auto) bulk request. You can use '''client.Flush();''' to send all cached data.
- To change the default auto-bulk behavior change '''client.AutoFlushValue''' (default is 50).

### DATA
There are the following telemetric data types:
- CrashReport - full anonymized crash reports. (no caching - send data direct to the server - use it only for complete application crashes)
- Exception - send .NET-Exceptions - including inner exceptions.
- PageView - send information about a new PageView (e.g. user opens a new form).
- Metrics - send information about an action/calculation/event and the metric (e. g. runtime).

## Roadmap
- More complex metrics
- Automate time measures
- Direct Visualisation of exceptions, pageViews and metrics