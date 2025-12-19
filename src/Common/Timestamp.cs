// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
#nullable enable

namespace KodeAid
{
    [StructLayout(LayoutKind.Auto)]
    [Serializable]
#if NETCOREAPP
    public readonly struct Timestamp : IComparable, ISpanFormattable, IComparable<Timestamp>, IEquatable<Timestamp>, ISerializable, IDeserializationCallback
#else
    public readonly struct Timestamp : IComparable, IComparable<Timestamp>, IEquatable<Timestamp>, ISerializable, IDeserializationCallback
#endif
    {
        // Static Fields
        public static readonly Timestamp MinValue = DateTimeOffset.MinValue;
        public static readonly Timestamp MaxValue = DateTimeOffset.MaxValue;

#if NETCOREAPP
        public static readonly Timestamp UnixEpoch = DateTimeOffset.UnixEpoch;
#endif
        private readonly DateTimeOffset _timestamp;

        public Timestamp(DateTime dateTime)
        {
            _timestamp = new(dateTime);
        }

        public Timestamp(DateTimeOffset timestamp)
        {
            _timestamp = timestamp;
        }

        public Timestamp(DateTime dateTime, TimeSpan offset)
        {
            _timestamp = new(dateTime, offset);
        }

        public Timestamp(long ticks, TimeSpan offset)
        {
            _timestamp = new(ticks, offset);
        }

        public Timestamp(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
        {
            _timestamp = new(year, month, day, hour, minute, second, offset);
        }

        public Timestamp(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset)
        {
            _timestamp = new(year, month, day, hour, minute, second, millisecond, offset);
        }

        public Timestamp(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
        {
            _timestamp = new(year, month, day, hour, minute, second, millisecond, calendar, offset);
        }

        public static Timestamp Now => DateTimeOffset.Now;

        public static Timestamp UtcNow => DateTimeOffset.UtcNow;

        public DateTime Date => _timestamp.Date;
        public int Day => _timestamp.Day;
        public DayOfWeek DayOfWeek => _timestamp.DayOfWeek;
        public int DayOfYear => _timestamp.DayOfYear;
        public int Hour => _timestamp.Hour;
        public int Millisecond => _timestamp.Millisecond;
        public int Minute => _timestamp.Minute;
        public int Month => _timestamp.Month;
        public TimeSpan Offset => _timestamp.Offset;
        public int Second => _timestamp.Second;
        public long Ticks => _timestamp.Ticks;
        public long UtcTicks => _timestamp.UtcTicks;
        public TimeSpan TimeOfDay => _timestamp.TimeOfDay;
        public int Year => _timestamp.Year;

        public Timestamp Add(TimeSpan timeSpan) => _timestamp.Add(timeSpan);

        public Timestamp AddDays(double days) => _timestamp.AddDays(days);

        public Timestamp AddHours(double hours) => _timestamp.AddHours(hours);

        public Timestamp AddMilliseconds(double milliseconds) => _timestamp.AddMilliseconds(milliseconds);

        public Timestamp AddMinutes(double minutes) => _timestamp.AddMinutes(minutes);

        public Timestamp AddMonths(int months) => _timestamp.AddMonths(months);

        public Timestamp AddSeconds(double seconds) => _timestamp.AddSeconds(seconds);

        public Timestamp AddTicks(long ticks) => _timestamp.AddTicks(ticks);

        public Timestamp AddYears(int years) => _timestamp.AddYears(years);

        public static int Compare(Timestamp first, Timestamp second) => first.CompareTo(second);

        int IComparable.CompareTo(object? obj) => ((IComparable)_timestamp).CompareTo(obj);

        public int CompareTo(Timestamp other) => _timestamp.CompareTo(other);

        public override bool Equals([NotNullWhen(true)] object? obj) => _timestamp.Equals(obj);

        public bool Equals(Timestamp other) => _timestamp.Equals(other);

        public bool EqualsExact(Timestamp other) => _timestamp.Equals(other);

        public static bool Equals(Timestamp first, Timestamp second)
            => DateTimeOffset.Equals(first, second);

        public static Timestamp FromFileTime(long fileTime)
            => DateTimeOffset.FromFileTime(fileTime);

        public static Timestamp FromUnixTimeSeconds(long seconds)
            => DateTimeOffset.FromUnixTimeSeconds(seconds);

        public static Timestamp FromUnixTimeMilliseconds(long milliseconds)
            => DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);

        void IDeserializationCallback.OnDeserialization(object? sender)
            => ((IDeserializationCallback)_timestamp).OnDeserialization(sender);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            => ((ISerializable)_timestamp).GetObjectData(info, context);

        public override int GetHashCode() => _timestamp.GetHashCode();

        public static Timestamp Parse(string input) => DateTimeOffset.Parse(input);

        public static Timestamp Parse(string input, IFormatProvider? formatProvider)
            => DateTimeOffset.Parse(input, formatProvider);

        public static Timestamp Parse(string input, IFormatProvider? formatProvider, DateTimeStyles styles)
            => DateTimeOffset.Parse(input, formatProvider, styles);

#if NETCOREAPP
        public static Timestamp Parse(ReadOnlySpan<char> input, IFormatProvider? formatProvider = null, DateTimeStyles styles = DateTimeStyles.None)
            => DateTimeOffset.Parse(input, formatProvider, styles);
#endif

        public static Timestamp ParseExact(string input, string format, IFormatProvider? formatProvider)
            => DateTimeOffset.ParseExact(input, format, formatProvider);

        public static Timestamp ParseExact(string input, string format, IFormatProvider? formatProvider, DateTimeStyles styles)
            => DateTimeOffset.ParseExact(input, format, formatProvider, styles);

#if NETCOREAPP
        public static Timestamp ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, IFormatProvider? formatProvider, DateTimeStyles styles = DateTimeStyles.None)
            => DateTimeOffset.ParseExact(input, format, formatProvider, styles);
#endif

        public static Timestamp ParseExact(string input, string[] formats, IFormatProvider? formatProvider, DateTimeStyles styles)
            => DateTimeOffset.ParseExact(input, formats, formatProvider, styles);

#if NETCOREAPP
        public static Timestamp ParseExact(ReadOnlySpan<char> input, string[] formats, IFormatProvider? formatProvider, DateTimeStyles styles = DateTimeStyles.None)
            => DateTimeOffset.ParseExact(input, formats, formatProvider, styles);
#endif

        public TimeSpan Subtract(Timestamp value) => _timestamp.Subtract(value);

        public Timestamp Subtract(TimeSpan value) => _timestamp.Subtract(value);

        public long ToFileTime() => _timestamp.ToFileTime();

        public long ToUnixTimeSeconds() => _timestamp.ToUnixTimeSeconds();

        public long ToUnixTimeMilliseconds() => _timestamp.ToUnixTimeMilliseconds();

        public Timestamp ToLocalTime() => _timestamp.ToLocalTime();

        public override string ToString() => _timestamp.ToString();

        public string ToString(string? format) => _timestamp.ToString(format);

        public string ToString(IFormatProvider? formatProvider) => _timestamp.ToString(formatProvider);

        public string ToString(string? format, IFormatProvider? formatProvider)
            => _timestamp.ToString(format, formatProvider);

#if NETCOREAPP
        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
            => _timestamp.TryFormat(destination, out charsWritten, format, provider);
#endif

        public Timestamp ToUniversalTime() => _timestamp.ToUniversalTime();

        public static bool TryParse([NotNullWhen(true)] string? input, out DateTimeOffset result)
            => DateTimeOffset.TryParse(input, out result);

#if NETCOREAPP
        public static bool TryParse(ReadOnlySpan<char> input, out DateTimeOffset result)
            => DateTimeOffset.TryParse(input, out result);
#endif

        public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParse(input, formatProvider, styles, out result);

#if NETCOREAPP
        public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParse(input, formatProvider, styles, out result);
#endif

        public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string? format, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParseExact(input, format, formatProvider, styles, out result);

#if NETCOREAPP
        public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParseExact(input, format, formatProvider, styles, out result);
#endif

        public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string?[]? formats, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParseExact(input, formats, formatProvider, styles, out result);

#if NETCOREAPP
        public static bool TryParseExact(ReadOnlySpan<char> input, [NotNullWhen(true)] string?[]? formats, IFormatProvider? formatProvider, DateTimeStyles styles, out DateTimeOffset result)
            => DateTimeOffset.TryParseExact(input, formats, formatProvider, styles, out result);
#endif

        // Operators

        public static implicit operator Timestamp(DateTimeOffset value) => new(value);

        public static implicit operator DateTimeOffset(Timestamp value) => value._timestamp;

        public static implicit operator Timestamp(DateTime dateTime) => (DateTimeOffset)dateTime;

        public static Timestamp operator +(Timestamp timestamp, TimeSpan timeSpan) =>
            new(timestamp._timestamp + timeSpan);

        public static Timestamp operator -(Timestamp timestamp, TimeSpan timeSpan) =>
            new(timestamp._timestamp - timeSpan);

        public static TimeSpan operator -(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime - right._timestamp.UtcDateTime;

        public static bool operator ==(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime == right._timestamp.UtcDateTime;

        public static bool operator !=(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime != right._timestamp.UtcDateTime;

        public static bool operator <(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime < right._timestamp.UtcDateTime;

        public static bool operator <=(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime <= right._timestamp.UtcDateTime;

        public static bool operator >(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime > right._timestamp.UtcDateTime;

        public static bool operator >=(Timestamp left, Timestamp right) =>
            left._timestamp.UtcDateTime >= right._timestamp.UtcDateTime;

        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        private sealed class NotNullWhenAttribute : Attribute
        {
            public NotNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;
            public bool ReturnValue { get; }
        }
    }
}
