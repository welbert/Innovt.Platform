﻿using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Innovt.Core.Utilities
{
    public static class Extensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static string ToStringOrDefault(this object obj)
        {
            return obj == null ? String.Empty : obj.ToString();
        }

        public static bool IsEmpty(this Guid id)
        {
            return id == Guid.Empty;
        }
        public static bool IsLessThanOrEqualToZero(this int id)
        {
            return id <= 0;
        }

        public static bool IsLessThanOrEqualToZero(this int? id)
        {
            return id.GetValueOrDefault().IsLessThanOrEqualToZero();
        }

        public static bool IsLessThanOrEqualToZero(this long id)
        {
            return id <= 0;
        }

        public static bool IsLessThanOrEqualToZero(this double id)
        {
            return id <= 0;
        }


        public static (long Latitude, long Longitude) ExtractCoordinates(this string str, char splittedBy = ';')
        {
            var splittedValues = str?.Split(splittedBy);

            if (splittedValues == null || splittedValues.Length == 0)
                return (0, 0);

            long latitude = 0;
            int op = 1;

            if (!splittedValues[0].IsNullOrEmpty())
            {
                op = splittedValues[0][0] == '-' ? -1 : 1;

                long.TryParse(splittedValues[0].OnlyNumber(), out latitude);

                latitude *= op;
            }


            long longitude = 0;

            if (splittedValues.Length > 1 && !splittedValues[1].IsNullOrEmpty())
            {
                op = splittedValues[1][0] == '-' ? -1 : 1;

                long.TryParse(splittedValues[1].OnlyNumber(), out longitude);

                longitude *= op;
            }

            return (latitude, longitude);
        }

        public static DateTimeOffset? MiliSecondsToDateTime(this double fromSeconds)
        {
            var dateTime = DateTimeOffset.MinValue;
            dateTime = dateTime.AddSeconds(fromSeconds).ToLocalTime();
            return dateTime;
        }

        public static string FormatToCurrency(this decimal value)
        {
            return $"{value:C}";
        }

        public static DateTimeOffset LastDayOfMonth(this DateTimeOffset date)
        {
            return new DateTimeOffset(
               new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)).AddDays(1)
                    .AddMilliseconds(-1), date.Offset);
        }

        public static string FormatToExtensionDateTime(this DateTimeOffset value)
        {
            string dia = String.Empty;

            string[] diaSemana = value.ToString("dddd").Split('-');

            dia = diaSemana[0].Substring(0, 1).ToUpper() + diaSemana[0].Substring(1, diaSemana[0].Length - 1);

            if (diaSemana.Length == 2)
                dia += "-" + diaSemana[1].Substring(0, 1).ToUpper() + diaSemana[1].Substring(1, diaSemana[1].Length - 1);

            string mes = value.ToString("MMMM");
            mes = mes.Substring(0, 1).ToUpper() + mes.Substring(1, mes.Length - 1);

            return dia + ", " + value.ToString("dd") + " de " + mes + " de " + value.ToString("yyyy") + ", " + value.ToString("t") + "h";
        }

        public static string FormatToSimpleDateTime(this DateTimeOffset value)
        {
            string dia = String.Empty;

            string[] diaSemana = value.ToString("dddd").Split('-');

            dia = diaSemana[0].Substring(0, 1).ToUpper() + diaSemana[0].Substring(1, diaSemana[0].Length - 1);


            return dia + " - " + value.Day + "/" + value.Month;
        }

        public static string FormatToPeriod(this DateTime startDate, DateTime endDate, CultureInfo cultureInfo = null)
        {
            return FormatToPeriod(new DateTimeOffset(startDate), new DateTimeOffset(endDate), cultureInfo);
        }

        public static DateTime FromUnixTimestamp(this double unixTimestamp)
        {
            var baseBase = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(unixTimestamp).ToLocalTime();

            return baseBase;
        }

        public static double ToUnixTimestamp(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static double ToUnixTimestamp(this DateTimeOffset dateTime)
        {
            return dateTime.UtcDateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string FormatToPeriod(this DateTimeOffset startDate, DateTimeOffset endDate, CultureInfo cultureInfo = null)
        {
            var currentYear = DateTimeOffset.Now.Year;

            if (cultureInfo == null)
                cultureInfo = new CultureInfo("pt-BR");

            //same day
            if (endDate.Subtract(startDate) == TimeSpan.Zero)
            {
                return startDate.ToString("d MMMM", cultureInfo);
            }
            else
            {
                //day equal but time diferent
                if (startDate.Day == endDate.Day && startDate.Month == endDate.Month && startDate.Year == endDate.Year)
                {
                    return $"{startDate.ToString("d MMMM", cultureInfo)} {startDate.ToString("HH:mm", cultureInfo)} - {endDate.ToString("HH:mm", cultureInfo)}";
                }
                else
                {
                    var res = String.Empty;
                    //same month
                    if (startDate.Month == endDate.Month)
                    {
                        if (startDate.Year == endDate.Year)
                        {
                            res = $"{startDate.Day} - {endDate.Day} {endDate.ToString("MMMM", cultureInfo)}";

                            if (startDate.Year != currentYear)
                                return res + $" {startDate.Year}";
                        }
                        else
                        {
                            return $"{startDate.Day} {startDate.ToString("MMMM", cultureInfo)} {startDate.Year} - {endDate.Day} {endDate.ToString("MMMM", cultureInfo)} {endDate.Year}";
                        }
                    }
                    else
                    {
                        if (startDate.Year == endDate.Year)
                        {
                            res = $"{startDate.Day} {startDate.ToString("MMMM", cultureInfo)} - {endDate.Day} {endDate.ToString("MMMM", cultureInfo)}";

                            if (startDate.Year != currentYear)
                                return res + $" {startDate.Year}";
                        }
                        else
                        {
                            return $"{startDate.Day} {startDate.ToString("MMMM", cultureInfo)} {startDate.Year} - {endDate.Day} {endDate.ToString("MMMM", cultureInfo)} {endDate.Year}";
                        }
                    }
                    return res;
                }
            }

        }

        public static string ToYesNO(this bool value)
        {
            return value ? "Sim" : "Não";
        }


        public static string[] ConvertToStringArray(this int value)
        {
            var res = new string[value + 1];

            for (int i = 0; i <= value; i++)
            {
                res[i] = i.ToString();
            }

            return res;
        }

        public static DateTimeOffset StartOfWeek(this DateTimeOffset dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTimeOffset OnlyDate(this DateTimeOffset now)
        {
            return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, 0, now.Offset);
        }

        public static DateTime ToBrazilianTimeZone(this DateTime date)
        {
            var dateUtc = date.ToUniversalTime().Subtract(TimeSpan.FromHours(3));

            return dateUtc;
        }

        public static string MaskCreditCard(this string number)
        {
            if (number.IsNullOrEmpty())
                return number;
          
            if (number.Length <= 10) 
                return number;

            const int beginLength = 6;
            const int endLength = 4;
            var middleLength = number.Length - beginLength - endLength;

            var result = number.Substring(0, beginLength);

            result = result.PadRight(middleLength,'*');
            
            return result;
        }

        //From Stackoverflow
        public static T FromByteArray<T>(this byte[] data)
        {
            if (data == null)
                return default;

            var bf = new BinaryFormatter();
            using var ms = new MemoryStream(data);
            object obj = bf.Deserialize(ms);
            return (T)obj;
        }
    }
}