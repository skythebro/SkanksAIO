using System.Collections.Generic;

namespace SkanksAIO.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// The <c>Pop()</c> method removes the last element from an array and returns that element. This method changes the length of the array.
    /// </summary>
    public static T Pop<T>(this List<T> self)
    {
        var r = self[self.Count - 1];
        self.RemoveAt(self.Count - 1);
        return r;
    }

    /// <summary>
    /// The <c>Shift()</c> method removes the first element from an array and returns that element. This method changes the length of the array.
    /// </summary>
    public static T Shift<T>(this List<T> self)
    {
        if (self.Count == 0)
        {
            // Return the default value for the type T or use null as a sentinel value.
            // You can modify this behavior based on your specific needs.
            return default; // or return null;
        }

        T r = self[0];
        self.RemoveAt(0);
        return r;
    }
}
