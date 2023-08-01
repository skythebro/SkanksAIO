namespace SkanksAIO.Extensions;

public static class IntExtensions
{
    /// <summary>
    /// The <c>Ordinal()</c> method returns the ordinal for the number.
    /// </summary>
    public static string Ordinal(this int self)
    {
        if (self <= 0) return self.ToString();

        switch (self % 100)
        {
            case 11:
            case 12:
            case 13:
                return "th";
        }

        return (self % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }
}
