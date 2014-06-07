namespace POSIX
{
    using System;

    public static class Time
    {
        private static readonly DateTime epoch = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan Elapsed(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            return dateTimeEnd.ToUniversalTime().Subtract(dateTimeStart.ToUniversalTime());
        }

        public static TimeSpan Elapsed(int timeStampStart, int timeStampEnd)
        {
            return TimeSpan.FromSeconds((double) (timeStampEnd - timeStampStart));
        }

        public static TimeSpan Elapsed(TimeSpan sinceEpochStart, TimeSpan sinceEpochEnd)
        {
            return sinceEpochEnd.Subtract(sinceEpochStart);
        }

        public static double ElapsedSeconds(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            return dateTimeEnd.ToUniversalTime().Subtract(dateTimeStart.ToUniversalTime()).TotalSeconds;
        }

        public static double ElapsedSeconds(int timeStampStart, int timeStampEnd)
        {
            return TimeSpan.FromSeconds((double) (timeStampEnd - timeStampStart)).TotalSeconds;
        }

        public static double ElapsedSeconds(TimeSpan sinceEpochStart, TimeSpan sinceEpochEnd)
        {
            return sinceEpochEnd.Subtract(sinceEpochStart).TotalSeconds;
        }

        public static double ElapsedSecondsSince(DateTime dateTime)
        {
            return DateTime.UtcNow.Subtract(dateTime.ToUniversalTime()).TotalSeconds;
        }

        public static double ElapsedSecondsSince(int timeStamp)
        {
            return DateTime.UtcNow.Subtract(epoch.AddSeconds((double) timeStamp)).TotalSeconds;
        }

        public static double ElapsedSecondsSince(TimeSpan sinceEpoch)
        {
            return DateTime.UtcNow.Subtract(epoch.Add(sinceEpoch)).TotalSeconds;
        }

        public static TimeSpan ElapsedSince(DateTime dateTime)
        {
            return DateTime.UtcNow.Subtract(dateTime.ToUniversalTime());
        }

        public static TimeSpan ElapsedSince(int timeStamp)
        {
            return DateTime.UtcNow.Subtract(epoch.AddSeconds((double) timeStamp));
        }

        public static TimeSpan ElapsedSince(TimeSpan sinceEpoch)
        {
            return DateTime.UtcNow.Subtract(epoch.Add(sinceEpoch));
        }

        public static int ElapsedStamp(DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            double totalSeconds = dateTimeEnd.ToUniversalTime().Subtract(dateTimeStart.ToUniversalTime()).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static int ElapsedStamp(int timeStampStart, int timeStampEnd)
        {
            double totalSeconds = TimeSpan.FromSeconds((double) (timeStampEnd - timeStampStart)).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static int ElapsedStamp(TimeSpan sinceEpochStart, TimeSpan sinceEpochEnd)
        {
            double totalSeconds = sinceEpochEnd.Subtract(sinceEpochStart).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static int ElapsedStampSince(DateTime dateTime)
        {
            double totalSeconds = DateTime.UtcNow.Subtract(dateTime.ToUniversalTime()).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static int ElapsedStampSince(int timeStamp)
        {
            double totalSeconds = DateTime.UtcNow.Subtract(epoch.AddSeconds((double) timeStamp)).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static int ElapsedStampSince(TimeSpan sinceEpoch)
        {
            double totalSeconds = DateTime.UtcNow.Subtract(epoch.Add(sinceEpoch)).TotalSeconds;
            int num2 = (int) totalSeconds;
            if (num2 > totalSeconds)
            {
                num2--;
            }
            return num2;
        }

        public static double NowSeconds
        {
            get
            {
                return DateTime.UtcNow.Subtract(epoch).TotalSeconds;
            }
        }

        public static TimeSpan NowSpan
        {
            get
            {
                return DateTime.UtcNow.Subtract(epoch);
            }
        }

        public static int NowStamp
        {
            get
            {
                double totalSeconds = DateTime.UtcNow.Subtract(epoch).TotalSeconds;
                int num2 = (int) totalSeconds;
                if (num2 > totalSeconds)
                {
                    num2--;
                }
                return num2;
            }
        }
    }
}

