public static class UIFormat
{
    public static string Minutes(int minutes)
    {
        int h = minutes / 60;
        int m = minutes % 60;

        if (h == 0) return $"{m} min";
        if (m == 0) return $"{h} h";
        return $"{h} h {m} min";
    }
}