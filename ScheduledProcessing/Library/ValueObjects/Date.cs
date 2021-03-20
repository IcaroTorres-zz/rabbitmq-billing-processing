using Library.Optimizations;
using System;
using ValueOf;

namespace Library.ValueObjects
{
    public class Date : ValueOf<string, Date>
    {
        private bool _isValid;
        public bool IsValid() => _isValid;

        protected override void Validate()
        {
            _isValid = TryParse(Value, out _, out _, out _);
        }

        public static bool TryParse(ReadOnlySpan<char> value, out byte day, out byte month, out ushort year)
        {
            day = 0;
            month = 0;
            year = 0;

            return value.Length == 10 &&
                value.Slice(6, 4).TryParseUshort(out year) &&
                value.Slice(3, 2).TryParseByte(out month) &&
                value.Slice(0, 2).TryParseByte(out day);
        }

        public static bool ValidateFutureDate(ReadOnlySpan<char> duedate)
        {
            return TryParse(duedate, out byte day, out byte month, out ushort year) &&
            (
                // future year
                (year > DateTime.Today.Year) ||
                // future month
                (year == DateTime.Today.Year && month > DateTime.Today.Month) ||
                // future day
                (year == DateTime.Today.Year && month == DateTime.Today.Month && day > DateTime.Today.Day)
            );
        }

        public static bool ValidateMonth(ReadOnlySpan<char> monthYear)
        {
            if (monthYear.Length != 7) return false;
            if (!monthYear.Slice(0, 2).TryParseByte(out var parsedMonth) || parsedMonth < 1 || parsedMonth > 12) return false;
            return monthYear.Slice(3, 4).TryParseUshort(out var parsedYear) && parsedYear >= 2000;
        }
    }
}
