using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace finance_manager.Services
{
    class TimeHelper
    {
        private static readonly string logPath = "./lastLog.txt";

        public static void checkFile()
        {
            if (!System.IO.File.Exists(logPath))
            {
                System.IO.File.Create(logPath).Close();
            }
        }

        public static string[] getCurrentDate()
        {

            string[] date = { DateTime.Now.Day.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Year.ToString() };
            return date;
        }

        public static void logDate()
        {
            string[] date = getCurrentDate();
            System.IO.File.WriteAllText(logPath, string.Empty);
            System.IO.File.WriteAllText(logPath, string.Join(",", date));
        }

        public static string[] getLastDate()
        {
            if (System.IO.File.Exists(logPath))
                return new string[] { "0", "0", "0" };
            else
            {
                string date = System.IO.File.ReadAllText(logPath);
                return date.Split(',');
            }
        }

        public static bool didMonthPass()
        {
            string[] lastDate = getLastDate();
            string[] currentDate = getCurrentDate();

            int lastDay = int.Parse(lastDate[0]);
            int lastMonth = int.Parse(lastDate[1]);
            int lastYear = int.Parse(lastDate[2]);

            int currentDay = int.Parse(currentDate[0]);
            int currentMonth = int.Parse(currentDate[1]);
            int currentYear = int.Parse(currentDate[2]);

            // Calculate the next 1st of the month after the last date
            DateTime lastDateTime = new DateTime(lastYear, lastMonth, lastDay);
            DateTime nextFirstOfMonth = new DateTime(lastYear, lastMonth, 1).AddMonths(1); // Move to next month's 1st

            // Get the current date
            DateTime currentDateTime = new DateTime(currentYear, currentMonth, currentDay);

            // Check if current date is on or after the next 1st of the month
            return currentDateTime >= nextFirstOfMonth;
        }

        public static int daysPassed()
        {
            string[] lastDate = getLastDate();
            string[] currentDate = getCurrentDate();

            DateTime lastDateTime = new DateTime(
                int.Parse(lastDate[2]), // Year
                int.Parse(lastDate[1]), // Month
                int.Parse(lastDate[0])  // Day
            );

            DateTime currentDateTime = new DateTime(
                int.Parse(currentDate[2]),
                int.Parse(currentDate[1]),
                int.Parse(currentDate[0])
            );

            int daysPassed = (int)(currentDateTime - lastDateTime).TotalDays;
            return daysPassed;
        }
    }
}
