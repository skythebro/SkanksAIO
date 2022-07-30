namespace SkanksAIO.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// The <c>Pop()</c> method removes the last element from an array and returns that element. This method changes the length of the array.
    /// </summary>
    public static T Pop<T>(this System.Collections.Generic.List<T> self)
    {
        T r = self[self.Count - 1];
        self.RemoveAt(self.Count - 1);
        return r;
    }

    /// <summary>
    /// The <c>Shift()</c> method removes the first element from an array and returns that element. This method changes the length of the array.
    /// </summary>
    public static T Shift<T>(this System.Collections.Generic.List<T> self)
    {
        T r = self[0];
        self.RemoveAt(0);
        return r;
    }
}
