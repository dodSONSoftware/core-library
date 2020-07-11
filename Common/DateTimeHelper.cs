using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provide some basic date and time related functions.
    /// </summary>
    /// <example>
    /// The following code example will demonstrate some of the functionality of the DateTimeHelper type.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create, slightly, different time spans
    ///     var shortTs = TimeSpan.FromMilliseconds(123);
    ///     var mediumTs = shortTs.Add(TimeSpan.FromSeconds(3.21));
    ///     var longTs = TimeSpan.FromMilliseconds(1234554321);
    ///     var exLongTs = TimeSpan.FromSeconds(1234554321);
    /// 
    ///     // Format TimeSpan
    ///     Console.WriteLine($"FormatTimeSpan(...)");
    ///     Console.WriteLine($"{dodSON.Core.Common.DateTimeHelper.FormatTimeSpan(shortTs, includeMilliseconds: true)}");
    ///     Console.WriteLine($"{dodSON.Core.Common.DateTimeHelper.FormatTimeSpan(mediumTs, includeMilliseconds: true)}");
    ///     Console.WriteLine($"{dodSON.Core.Common.DateTimeHelper.FormatTimeSpan(longTs)}");
    ///     Console.WriteLine($"{dodSON.Core.Common.DateTimeHelper.FormatTimeSpan(exLongTs)}");
    ///     Console.WriteLine();
    /// 
    ///     // Format TimeSpan Verbose
    ///     Console.WriteLine($"FormatTimeSpanVerbose(...)");
    ///     Console.WriteLine(dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(shortTs));
    ///     Console.WriteLine(dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(mediumTs));
    ///     Console.WriteLine(dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(longTs));
    ///     Console.WriteLine(dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(exLongTs));
    ///     Console.WriteLine();
    /// 
    ///     // Date Time Extremes
    ///     DateTime startDate;
    ///     DateTime stopDate;
    ///     dodSON.Core.Common.DateTimeHelper.GetTodayDateTimeExtremes(out startDate, out stopDate);
    ///     Console.WriteLine(string.Format("Today's Extremes= {0} to {1}", startDate, stopDate));
    ///     dodSON.Core.Common.DateTimeHelper.GetDateTimeExtremes(new DateTime(1977, 5, 25), out startDate, out stopDate);
    ///     Console.WriteLine(string.Format("Star Wars' Debut Extremes= {0} to {1}", startDate, stopDate));
    /// 
    ///     // 
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine("press any key>");
    ///     Console.ReadKey(true);
    /// 
    /// 
    ///     // This code produces output similar to the following:
    /// 
    ///     // FormatTimeSpan(...)
    ///     // 0:00:00.123
    ///     // 0:00:03.333
    ///     // 2 weeks 6:55:54
    ///     // 39 years 1 month 3 weeks 4 days 19:45:21
    ///     // 
    ///     // FormatTimeSpanVerbose(...)
    ///     // 123 milliseconds
    ///     // 3.333 seconds
    ///     // 2 weeks 6:55:54
    ///     // 39 years 1 month 3 weeks 4 days 19:45:21
    ///     // 
    ///     // Today's Extremes= 2019-08-17 12:00:00 AM to 2019-08-17 11:59:59 PM
    ///     // Star Wars' Debut Extremes= 1977-05-25 12:00:00 AM to 1977-05-25 11:59:59 PM
    ///     // 
    ///     // ------------------------
    ///     // press any key>
    /// }
    /// </code>
    /// </example>
    public static class DateTimeHelper
    {
        #region Static Public Methods
        /// <summary>
        /// Will format a <see cref="TimeSpan"/> into a simple, human readable format.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to format.</param>
        /// <param name="includeDate">Used to include or exclude the date element.</param>
        /// <param name="showMonths">
        /// If <b>true</b> it will display months and weeks; otherwise, <b>false</b> will only show weeks.
        /// It should be noted that this method assumes there are 52 weeks a years and that 1 month is 28 days long.
        /// </param>
        /// <param name="showZeroDays">Used to include the date element if the date is 0 days.</param>
        /// <param name="includeTime">Used to include or exclude the time element.</param>
        /// <param name="dateTimeSeparator">A string to separate the date and time elements.</param>
        /// <param name="includeLeadingZeroOnHour">Determines whether to include a leading zero on the hour when it is less than 10.</param>
        /// <param name="includeSeconds">Determines whether to include the seconds.</param>
        /// <param name="includeMilliseconds">Determines whether to include any fractions of a second.</param>
        /// <param name="includeLeadingZeroOnMilliseconds">Determines whether to include a leading zeros on the milliseconds.</param>
        /// <returns>A string formatted from the given <see cref="TimeSpan"/> based on the given criteria.</returns>
        public static string FormatTimeSpan(TimeSpan value,
                                            bool includeDate = true,
                                            bool showMonths = true,
                                            bool showZeroDays = false,
                                            bool includeTime = true,
                                            string dateTimeSeparator = " ",
                                            bool includeLeadingZeroOnHour = false,
                                            bool includeSeconds = true,
                                            bool includeMilliseconds = false,
                                            bool includeLeadingZeroOnMilliseconds = true)
        {
            if (includeDate)
            {
                var days = FormatDays();
                if (includeTime)
                {
                    var separator = (days.Length > 0) ? dateTimeSeparator : "";
                    return $"{days}{separator}{FormatTime()}";
                }
                else
                {
                    return days;
                }
            }
            else
            {
                return FormatTime();
            }

            // ######## INTERNAL FUNCTIONS ########

            string FormatTime()
            {
                return string.Format("{0}:{1}{2}{3}",
                                      ((value.Hours < 1) ? ((includeLeadingZeroOnHour) ? "00" : "0") : ((includeLeadingZeroOnHour) ? value.Hours.ToString("00") : value.Hours.ToString("0"))),
                                      ((value.Minutes < 10) ? value.Minutes.ToString("00") : value.Minutes.ToString("0")),
                                      ((includeSeconds) ? (":" + value.Seconds.ToString("00")) : ""),
                                      ((includeSeconds && includeMilliseconds) ? ("." + (includeLeadingZeroOnMilliseconds ? value.Milliseconds.ToString("000") : value.Milliseconds.ToString())) : ""));
            }
            string FormatDays()
            {
                // ######## process timespan
                var years = value.Days / 365;
                var yearsPlural = (years > 1) ? "s" : "";
                string yearsStr;
                string monthsWeeksStr;
                string daysStr;
                if (showMonths)
                {
                    // include months/weeks
                    var weeks = (value.Days % 365) / 7;
                    var months = weeks / 4;
                    if (months >= 13) { months = 12; }
                    var monthsPlural = (months > 1) ? "s" : "";
                    var remainingWeeks = weeks - (months * 4);
                    var weeksPlural = (remainingWeeks > 1) ? "s" : "";
                    var remainingDays = (value.Days % 365) % 7;
                    var dayPlural = (remainingDays > 1) ? "s" : "";
                    // --------
                    yearsStr = (years == 0) ? "" : $"{years:N0} year{yearsPlural}";
                    //
                    var monthStr = (months == 0) ? "" : $"{months:N0} month{monthsPlural}";
                    var weekStr = (remainingWeeks == 0) ? "" : $"{remainingWeeks:N0} week{weeksPlural}";
                    var monthWeekPreSeparator = ((yearsStr.Length > 0) && ((monthStr.Length > 0) || (weekStr.Length > 0))) ? " " : "";
                    var monthWeekSeparator = ((monthStr.Length > 0) && (weekStr.Length > 0)) ? " " : "";
                    monthsWeeksStr = $"{monthWeekPreSeparator}{monthStr}{monthWeekSeparator}{weekStr}";
                    //
                    var daySeparator = ((yearsStr.Length > 0) || (monthsWeeksStr.Length > 0)) ? " " : "";
                    daysStr = (remainingDays == 0) ? "" : $"{daySeparator}{remainingDays:N0} day{dayPlural}";
                }
                else
                {
                    // weeks only
                    var weeks = (value.Days % 365) / 7;
                    var weeksPlural = (weeks > 1) ? "s" : "";
                    var remainingDays = (value.Days % 365) % 7;
                    var dayPlural = (remainingDays > 1) ? "s" : "";
                    // --------
                    yearsStr = (years == 0) ? "" : $"{years:N0} year{yearsPlural}";
                    //
                    var monthWeekPreSeparator = ((yearsStr.Length > 0) && (weeks > 0)) ? " " : "";
                    monthsWeeksStr = (weeks == 0) ? "" : $"{monthWeekPreSeparator}{weeks:N0} week{weeksPlural}";
                    //
                    var daySeparator = ((yearsStr.Length > 0) || (monthsWeeksStr.Length > 0)) ? " " : "";
                    daysStr = (remainingDays == 0) ? "" : $"{daySeparator}{remainingDays:N0} day{dayPlural}";
                }

                // ######## final
                var results = $"{yearsStr}{monthsWeeksStr}{daysStr}";
                return (results.Length > 0) ? results : showZeroDays ? $"0 days" : "";
            }
        }
        /// <summary>
        /// Will format a <see cref="TimeSpan"/> into a human readable format.
        /// This methods is better suited for values in the milliseconds and seconds range.
        /// After <paramref name="secondsThreshold"/> seconds, it will switch to use the <see cref="FormatTimeSpan(TimeSpan, bool, bool, bool, bool, string, bool, bool, bool, bool)"/> method.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> to format.</param>
        /// <param name="secondsThreshold">The total number of seconds required before switching to the <see cref="FormatTimeSpan(TimeSpan, bool, bool, bool, bool, string, bool, bool, bool, bool)"/> method.</param>
        /// <returns> A string formatted from a given <see cref="TimeSpan"/> based on the given criteria.</returns>
        public static string FormatTimeSpanVerbose(TimeSpan value, double secondsThreshold = 90)
        {
            if (value.TotalSeconds < 1)
            {
                return $"{value.TotalMilliseconds:000} milliseconds";
            }
            else if (value.TotalSeconds < secondsThreshold)
            {
                return $"{value.TotalSeconds:N3} seconds";
            }
            else
            {
                return FormatTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets the time extremes for the given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The target <see cref="DateTime"/> to get the time extremes for.</param>
        /// <param name="startDate">Represents midnight (00:00:00.000) for the given <see cref="DateTime"/>.</param>
        /// <param name="stopDate">Represents the last millisecond (23:59:59.999) for the given <see cref="DateTime"/>.</param>
        public static void GetDateTimeExtremes(DateTime value,
                                               out DateTime startDate,
                                               out DateTime stopDate)
        {
            startDate = value.Date;
            stopDate = startDate.AddDays(1).AddMilliseconds(-1);
        }
        /// <summary>
        /// Gets the <see cref="DateTime"/> extremes for today.
        /// </summary>
        /// <param name="startDate">Represents midnight (00:00:00.000) for the given <see cref="DateTime"/>.</param>
        /// <param name="stopDate">Represents the last millisecond (23:59:59.999) for the given <see cref="DateTime"/>.</param>
        public static void GetTodayDateTimeExtremes(out DateTime startDate,
                                                    out DateTime stopDate) => GetDateTimeExtremes(DateTime.Now, out startDate, out stopDate);
        #endregion
    }
}
