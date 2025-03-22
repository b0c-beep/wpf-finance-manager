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

        public static string getCurrentDate()
        {

            string date = DateTime.Now.Month + "/" + DateTime.Now.Year;
            return date;
        }

        public static void logDate()
        {
            string date = getCurrentDate();
            System.IO.File.WriteAllText(logPath, string.Empty);
            System.IO.File.WriteAllText(logPath, date);
        }

        public static string getLastDate()
        {
            return System.IO.File.Exists(logPath) ? System.IO.File.ReadAllText(logPath).Trim() : "";
        }

        public static bool didMonthPass()
        {
            string lastDate = getLastDate();
            string currentDate = getCurrentDate();
            if (lastDate != currentDate)
            {
                return true;
            }
            return false;
        }
    }
}
