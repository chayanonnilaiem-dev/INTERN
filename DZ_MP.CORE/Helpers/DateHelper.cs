namespace DZ_MP.CORE.Helpers;

public static class DateHelper
{
    private const int BuddhistOffset = 543;

    /// <summary>
    /// แปลง DateTime เป็น string รูปแบบ dd/MM/yyyy พ.ศ. เช่น 01/01/2568
    /// </summary>
    public static string? ToThaiDate(DateTime? date)
    {
        if (date is null) return null;
        var d = date.Value;
        return $"{d.Day:D2}/{d.Month:D2}/{d.Year + BuddhistOffset}";
    }

    /// <summary>
    /// แปลง DateTime เป็น string รูปแบบ dd/MM/yyyy HH:mm:ss พ.ศ. เช่น 01/01/2567 13:30:22
    /// </summary>
    public static string? ToThaiDateTime(DateTime? date)
    {
        if (date is null) return null;
        var d = date.Value;
        return $"{d.Day:D2}/{d.Month:D2}/{d.Year + BuddhistOffset} {d:HH:mm:ss}";
    }
}
