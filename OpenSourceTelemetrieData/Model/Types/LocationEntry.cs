using System;

namespace OpenSourceTelemetrieData.Model.Types
{
  [Serializable]
  public struct LocationEntry
  {
    public uint Start { get; set; }
    public uint Stop { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
  }
}
