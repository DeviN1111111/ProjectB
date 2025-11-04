static class GetDayDate
{
    public static List<string[]> getDayDate()
    {
        DateTime today = DateTime.Today;
        List<string[]> dayDate = new();

        for (int i = 0; i <= 7; i++)
        {
            DateTime date = today.AddDays(i);
            string day = date.ToString("dddd");   // e.g. "Monday"
            string formattedDate = date.ToString("d MMMM yyyy"); // e.g. "4 November 2025"
            dayDate.Add(new[] { day, formattedDate });
        }

        return dayDate;
    }
}
