using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Research.SEAL;

namespace HE2.ClientApp.Helpers
{
  public static class Utilities
  {
    public static void PrintLine([CallerLineNumber] int lineNumber = 0)
    {
      Console.Write("Line {0,3} --> ", lineNumber);
    }

    public static void PrintVector<T>(
      IEnumerable<T> vec, int printSize = 4, int prec = 3)
    {
      string numFormat = string.Format("{{0:N{0}}}", prec);
      T[] veca = vec.ToArray();
      int slotCount = veca.Length;
      Console.WriteLine();
      if (slotCount <= 2 * printSize)
      {
        Console.Write("    [");
        for (int i = 0; i < slotCount; i++)
        {
          Console.Write(" " + string.Format(numFormat, veca[i]));
          if (i != (slotCount - 1))
            Console.Write(",");
          else
            Console.Write(" ]");
        }
        Console.WriteLine();
      }
      else
      {
        Console.Write("    [");
        for (int i = 0; i < printSize; i++)
        {
          Console.Write(" "+ string.Format(numFormat, veca[i]) + ", ");
        }
        if (veca.Length > 2 * printSize)
        {
          Console.Write(" ...");
        }
        for (int i = slotCount - printSize; i < slotCount; i++)
        {
          Console.Write(", " + string.Format(numFormat, veca[i]));
        }
        Console.WriteLine(" ]");
      }
      Console.WriteLine();
    }

    public static string ULongToString(ulong value)
    {
      byte[] bytes = BitConverter.GetBytes(value);
      return BitConverter.ToString(bytes).Replace("-", "");
    }
  }
}
