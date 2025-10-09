static class GetDayDate
{
    public static List<string[]> getDayDate()
    {
        DateTime today = DateTime.Today;
        List<string[]> dayDate = new List<string[]>();
        for (int i = 0; i <= 7; i++)
        {
            DateTime date = today.AddDays(i);
            string formatedDate = date.ToString("D");
            string[] splitDate = formatedDate.Split(',');
            string day = splitDate[0].Trim();
            string Date = splitDate[1].Trim();
            dayDate.Add([day, Date]);
        }
        return dayDate;
    }
}