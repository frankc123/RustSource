using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct TimeStringFormatter
{
    public const string kArgumentTime = "<ꪻ뮪>";
    private const string kArgumentTimeReplacement = "{0}";
    public const string kPeriod = ".";
    public readonly string aDay;
    public readonly string days;
    public readonly string aHour;
    public readonly string hours;
    public readonly string aMinute;
    public readonly string minutes;
    public readonly string aSecond;
    public readonly string seconds;
    public readonly string lessThanASecond;
    private TimeStringFormatter(string aDay, string days, string aHour, string hours, string aMinute, string minutes, string aSecond, string seconds, string lessThanASecond)
    {
        this.aDay = aDay;
        this.days = days;
        this.aHour = aHour;
        this.hours = hours;
        this.aMinute = aMinute;
        this.minutes = minutes;
        this.aSecond = aSecond;
        this.seconds = seconds;
        this.lessThanASecond = lessThanASecond;
    }

    private static string DoMerge(string value)
    {
        return value.Replace("{", "{{").Replace("}", "}}").Replace("<ꪻ뮪>", "{0}");
    }

    private static string Merge(string prefix)
    {
        if (prefix == null)
        {
        }
        return DoMerge(string.Empty);
    }

    private static string Merge(string prefix, string qualifier)
    {
        if (prefix == null)
        {
        }
        if (qualifier == null)
        {
        }
        return DoMerge(string.Empty + string.Empty);
    }

    private static string Merge(string prefix, string qualifier, string suffix)
    {
        if (prefix == null)
        {
        }
        if (qualifier == null)
        {
        }
        if (suffix == null)
        {
        }
        return DoMerge(string.Empty + string.Empty + string.Empty);
    }

    public static TimeStringFormatter Define(Qualifier qualifier)
    {
        return new TimeStringFormatter(Merge(qualifier.aDay), Merge(qualifier.days), Merge(qualifier.aHour), Merge(qualifier.hours), Merge(qualifier.aMinute), Merge(qualifier.minutes), Merge(qualifier.aSecond), Merge(qualifier.seconds), Merge(qualifier.lessThanASecond));
    }

    public static TimeStringFormatter Define(string prefix, Qualifier qualifier)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return Define(qualifier);
        }
        return new TimeStringFormatter(Merge(prefix, qualifier.aDay), Merge(prefix, qualifier.days), Merge(prefix, qualifier.aHour), Merge(prefix, qualifier.hours), Merge(prefix, qualifier.aMinute), Merge(prefix, qualifier.minutes), Merge(prefix, qualifier.aSecond), Merge(prefix, qualifier.seconds), Merge(prefix, qualifier.lessThanASecond));
    }

    public static TimeStringFormatter Define(Qualifier qualifier, string suffix)
    {
        if (string.IsNullOrEmpty(suffix))
        {
            return Define(qualifier);
        }
        return new TimeStringFormatter(Merge(qualifier.aDay, suffix), Merge(qualifier.days, suffix), Merge(qualifier.aHour, suffix), Merge(qualifier.hours, suffix), Merge(qualifier.aMinute, suffix), Merge(qualifier.minutes, suffix), Merge(qualifier.aSecond, suffix), Merge(qualifier.seconds, suffix), Merge(qualifier.lessThanASecond, suffix));
    }

    public static TimeStringFormatter Define(string prefix, Qualifier qualifier, string suffix)
    {
        if (string.IsNullOrEmpty(suffix))
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return Define(qualifier);
            }
            return Define(prefix, qualifier);
        }
        if (string.IsNullOrEmpty(prefix))
        {
            return Define(qualifier, suffix);
        }
        return new TimeStringFormatter(Merge(prefix, qualifier.aDay, suffix), Merge(prefix, qualifier.days, suffix), Merge(prefix, qualifier.aHour, suffix), Merge(prefix, qualifier.hours, suffix), Merge(prefix, qualifier.aMinute, suffix), Merge(prefix, qualifier.minutes, suffix), Merge(prefix, qualifier.aSecond, suffix), Merge(prefix, qualifier.seconds, suffix), Merge(prefix, qualifier.lessThanASecond, suffix));
    }

    public static TimeStringFormatter Define(TimeStringFormatter formatter, string lessThanASecond)
    {
        if (!object.ReferenceEquals(lessThanASecond, null))
        {
            formatter = new TimeStringFormatter(formatter.aDay, formatter.days, formatter.aHour, formatter.hours, formatter.aMinute, formatter.minutes, formatter.aSecond, formatter.seconds, Merge(lessThanASecond));
        }
        return formatter;
    }

    public static TimeStringFormatter Define(string prefix, Qualifier qualifier, string suffix, string lessThanASecond)
    {
        return Define(Define(prefix, qualifier, suffix), lessThanASecond);
    }

    private static double Round(double total, Rounding rounding, int decimalPlaces, double fancyUnits)
    {
        if (total <= 0.0)
        {
            return 0.0;
        }
        switch (rounding)
        {
            case Rounding.Floor:
                return Math.Floor(total);

            case Rounding.Ceiling:
                return Math.Ceiling(total);

            case Rounding.Round:
                return Math.Round(total);

            case Rounding.Decimal:
                fancyUnits = 1.0;
                decimalPlaces = 0;
                break;

            case Rounding.RoundedDecimal:
                fancyUnits = 1.0;
                break;

            case Rounding.FancyDecimal:
                decimalPlaces = 0;
                break;
        }
        if (decimalPlaces == 0)
        {
            return total;
        }
        double num = Math.Floor(total);
        return (num + (Math.Floor((double) (((total - num) * fancyUnits) * (decimalPlaces * 10.0))) / (10.0 * decimalPlaces)));
    }

    public string GetFormattingString(TimeSpan timePassed)
    {
        return this.GetFormattingString(timePassed, Rounding.Floor);
    }

    public string GetFormattingString(TimeSpan timePassed, Rounding rounding)
    {
        string lessThanASecond;
        int num;
        double num2;
        object obj2;
        double d = Round(timePassed.TotalSeconds, rounding, num = 2, num2 = 1.0);
        if (d <= 0.0)
        {
            lessThanASecond = this.lessThanASecond;
        }
        else if (d == 1.0)
        {
            lessThanASecond = this.aSecond;
        }
        else if (d < 60.0)
        {
            lessThanASecond = this.seconds;
        }
        else
        {
            d = Round(timePassed.TotalMinutes, rounding, num = 2, num2 = 0.6);
            if (d == 1.0)
            {
                lessThanASecond = this.aMinute;
            }
            else if (d < 60.0)
            {
                lessThanASecond = this.minutes;
            }
            else
            {
                d = Round(timePassed.TotalHours, rounding, num = 2, num2 = 1.0);
                if (d == 1.0)
                {
                    lessThanASecond = this.aHour;
                }
                else if (d < 24.0)
                {
                    lessThanASecond = this.hours;
                }
                else
                {
                    d = Round(timePassed.TotalDays, rounding, num = 2, num2 = 0.24);
                    if (d == 1.0)
                    {
                        lessThanASecond = this.aDay;
                    }
                    else
                    {
                        lessThanASecond = this.days;
                    }
                }
            }
        }
        if (((rounding == Rounding.RoundedDecimal) || (rounding == Rounding.FancyDecimal)) || (rounding == Rounding.RoundedFancyDecimal))
        {
            string str2;
            if ((rounding == Rounding.RoundedDecimal) || (rounding == Rounding.RoundedFancyDecimal))
            {
                if (num != 2)
                {
                    throw new NotSupportedException("We gotta add support for that");
                }
                str2 = d.ToString("0.00");
            }
            else
            {
                str2 = d.ToString();
            }
            if ((rounding == Rounding.FancyDecimal) || (rounding == Rounding.RoundedFancyDecimal))
            {
                obj2 = str2.Replace('.', ':');
            }
            else
            {
                obj2 = str2;
            }
        }
        else if (((rounding != Rounding.Decimal) && !double.IsNaN(d)) && !double.IsInfinity(d))
        {
            obj2 = (int) d;
        }
        else
        {
            obj2 = d;
        }
        return string.Format(lessThanASecond, obj2);
    }
    public static class Ago
    {
        public const string aDay = " a day ago";
        public const string aHour = " an hour ago";
        public const string aMinute = " a minute ago";
        public const string aSecond = " a second ago";
        public const string days = " <ꪻ뮪> days ago";
        public const string hours = " <ꪻ뮪> hours ago";
        public const string kSuffix = " ago";
        public const string lessThanASecond = "";
        public const string minutes = " <ꪻ뮪> minutes ago";
        public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" a day ago", " <ꪻ뮪> days ago", " an hour ago", " <ꪻ뮪> hours ago", " a minute ago", " <ꪻ뮪> minutes ago", " a second ago", " <ꪻ뮪> seconds ago", string.Empty);
        public const string seconds = " <ꪻ뮪> seconds ago";

        public static class Period
        {
            public const string aDay = " a day ago.";
            public const string aHour = " an hour ago.";
            public const string aMinute = " a minute ago.";
            public const string aSecond = " a second ago.";
            public const string days = " <ꪻ뮪> days ago.";
            public const string hours = " <ꪻ뮪> hours ago.";
            public const string kSuffix = ".";
            public const string lessThanASecond = ".";
            public const string minutes = " <ꪻ뮪> minutes ago.";
            public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" a day ago.", " <ꪻ뮪> days ago.", " an hour ago.", " <ꪻ뮪> hours ago.", " a minute ago.", " <ꪻ뮪> minutes ago.", " a second ago.", " <ꪻ뮪> seconds ago.", ".");
            public const string seconds = " <ꪻ뮪> seconds ago.";
        }
    }

    public static class For
    {
        public const string aDay = " for a day";
        public const string aHour = " for an hour";
        public const string aMinute = " for a minute";
        public const string aSecond = " for a second";
        public const string days = " for <ꪻ뮪> days";
        public const string hours = " for <ꪻ뮪> hours";
        public const string kPrefix = " for";
        public const string lessThanASecond = "";
        public const string minutes = " for <ꪻ뮪> minutes";
        public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" for a day", " for <ꪻ뮪> days", " for an hour", " for <ꪻ뮪> hours", " for a minute", " for <ꪻ뮪> minutes", " for a second", " for <ꪻ뮪> seconds", string.Empty);
        public const string seconds = " for <ꪻ뮪> seconds";

        public static class Period
        {
            public const string aDay = " for a day.";
            public const string aHour = " for an hour.";
            public const string aMinute = " for a minute.";
            public const string aSecond = " for a second.";
            public const string days = " for <ꪻ뮪> days.";
            public const string hours = " for <ꪻ뮪> hours.";
            public const string kSuffix = ".";
            public const string lessThanASecond = ".";
            public const string minutes = " for <ꪻ뮪> minutes.";
            public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" for a day.", " for <ꪻ뮪> days.", " for an hour.", " for <ꪻ뮪> hours.", " for a minute.", " for <ꪻ뮪> minutes.", " for a second.", " for <ꪻ뮪> seconds.", ".");
            public const string seconds = " for <ꪻ뮪> seconds.";
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Qualifier
    {
        public readonly string aDay;
        public readonly string days;
        public readonly string aHour;
        public readonly string hours;
        public readonly string aMinute;
        public readonly string minutes;
        public readonly string aSecond;
        public readonly string seconds;
        public readonly string lessThanASecond;
        public Qualifier(string aDay, string days, string aHour, string hours, string aMinute, string minutes, string aSecond, string seconds, string lessThanASecond)
        {
            this.aDay = aDay;
            this.days = days;
            this.aHour = aHour;
            this.hours = hours;
            this.aMinute = aMinute;
            this.minutes = minutes;
            this.aSecond = aSecond;
            this.seconds = seconds;
            this.lessThanASecond = lessThanASecond;
        }
    }

    public static class Quantity
    {
        public const string aDay = " a day";
        public const string aHour = " an hour";
        public const string aMinute = " a minute";
        public const string aSecond = " a second";
        public const string days = " <ꪻ뮪> days";
        public const string hours = " <ꪻ뮪> hours";
        public const string kPrefix = " ";
        public const string lessThanASecond = "";
        public const string minutes = " <ꪻ뮪> minutes";
        public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" a day", " <ꪻ뮪> days", " an hour", " <ꪻ뮪> hours", " a minute", " <ꪻ뮪> minutes", " a second", " <ꪻ뮪> seconds", string.Empty);
        public const string seconds = " <ꪻ뮪> seconds";

        public static class Period
        {
            public const string aDay = " a day.";
            public const string aHour = " an hour.";
            public const string aMinute = " a minute.";
            public const string aSecond = " a second.";
            public const string days = " <ꪻ뮪> days.";
            public const string hours = " <ꪻ뮪> hours.";
            public const string kSuffix = ".";
            public const string lessThanASecond = ".";
            public const string minutes = " <ꪻ뮪> minutes.";
            public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" a day.", " <ꪻ뮪> days.", " an hour.", " <ꪻ뮪> hours.", " a minute.", " <ꪻ뮪> minutes.", " a second.", " <ꪻ뮪> seconds.", ".");
            public const string seconds = " <ꪻ뮪> seconds.";
        }
    }

    public enum Rounding
    {
        Floor,
        Ceiling,
        Round,
        Decimal,
        RoundedDecimal,
        FancyDecimal,
        RoundedFancyDecimal
    }

    public static class SinceAgo
    {
        public const string aDay = " since a day ago";
        public const string aHour = " since an hour ago";
        public const string aMinute = " since a minute ago";
        public const string aSecond = " since a second ago";
        public const string days = " since <ꪻ뮪> days ago";
        public const string hours = " since <ꪻ뮪> hours ago";
        public const string kPrefix = " since";
        public const string lessThanASecond = "";
        public const string minutes = " since <ꪻ뮪> minutes ago";
        public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" since a day ago", " since <ꪻ뮪> days ago", " since an hour ago", " since <ꪻ뮪> hours ago", " since a minute ago", " since <ꪻ뮪> minutes ago", " since a second ago", " since <ꪻ뮪> seconds ago", string.Empty);
        public const string seconds = " since <ꪻ뮪> seconds ago";

        public static class Period
        {
            public const string aDay = " since a day ago.";
            public const string aHour = " since an hour ago.";
            public const string aMinute = " since a minute ago.";
            public const string aSecond = " since a second ago.";
            public const string days = " since <ꪻ뮪> days ago.";
            public const string hours = " since <ꪻ뮪> hours ago.";
            public const string kSuffix = ".";
            public const string lessThanASecond = ".";
            public const string minutes = " since <ꪻ뮪> minutes ago.";
            public static readonly TimeStringFormatter.Qualifier Qualifier = new TimeStringFormatter.Qualifier(" since a day ago.", " since <ꪻ뮪> days ago.", " since an hour ago.", " since <ꪻ뮪> hours ago.", " since a minute ago.", " since <ꪻ뮪> minutes ago.", " since a second ago.", " since <ꪻ뮪> seconds ago.", ".");
            public const string seconds = " since <ꪻ뮪> seconds ago.";
        }
    }
}

