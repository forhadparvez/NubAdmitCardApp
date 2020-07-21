using System;
using System.Globalization;

namespace App.Core.Application
{
    public class DateTimeFormatter
    {
        public static DateTime StringToDate(string text)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                return DateTime.ParseExact(text, "dd-MM-yyyy", provider);
            }
            catch (Exception)
            {
                throw new ApplicationException("Date Not Valid");
            }
        }

        public static string DateToString(DateTime? date)
        {
            try
            {

                if (date != null)
                {
                    var d = Convert.ToDateTime(date);
                    return d.ToString("dd-MM-yyyy");
                }

                return "";
            }
            catch (Exception)
            {
                throw new ApplicationException("Date Not Valid");
            }
        }

        public static string DateTimeToString(DateTime? date)
        {
            try
            {

                if (date != null)
                {
                    var d = Convert.ToDateTime(date);
                    return d.ToString("dd-MM-yyyy HH:mm");
                }

                return "";
            }
            catch (Exception)
            {
                throw new ApplicationException("Date Not Valid");
            }
        }

        public static string StringDateToStringFormatter(string stringDate, string formatter)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var date = DateTime.ParseExact(stringDate, "dd-MM-yyyy", provider);
                return date.ToString(formatter);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static TimeSpan StringToTime(string text)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                return DateTime.ParseExact(text, "h:mm tt", provider).TimeOfDay;
            }
            catch (Exception)
            {
                throw new ApplicationException("Time Not Valid");
            }
        }

        public static string TimeToString(TimeSpan time)
        {
            try
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                var date = DateTime.ParseExact("01-01-2000", "dd-MM-yyyy", provider);
                date += time;
                return date.ToString("h:mm tt");
            }
            catch (Exception)
            {
                throw new ApplicationException("Time Not Valid");
            }
        }
    }
}