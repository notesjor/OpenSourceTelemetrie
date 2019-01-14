using System;
using System.Linq;

namespace OpenSourceTelemetrieData.Helper
{
  public static class IpToUintConverter
  {
    public static uint Convert(this string text)
    {
      var split = text.Split('.');
      var array = split.Select(byte.Parse).ToArray();
      if (BitConverter.IsLittleEndian)
        Array.Reverse(array);
      return split.Length != 4 ? 0 : BitConverter.ToUInt32(array);
    }
  }
}
