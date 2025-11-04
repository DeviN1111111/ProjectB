// static class GetDayDate
// {
//     public static List<string[]> getDayDate()
//     {
//         DateTime today = DateTime.Today;
//         List<string[]> dayDate = new List<string[]>();
//         for (int i = 0; i <= 7; i++)
//         {
//             DateTime date = today.AddDays(i);
//             string formatedDate = date.ToString("D");
//             string[] splitDate = formatedDate.Split(',');
//             string day = splitDate[0].Trim();
//             string Date = splitDate[1].Trim();
//             dayDate.Add([day, Date]);
//         }
//         return dayDate;
//     }
// }

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
