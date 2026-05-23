using System;
using System.Text;

namespace NumberSystemCalculator
{
    /// <summary>
    /// Static utility class that performs conversions between number systems
    /// (binary, octal, decimal, hexadecimal) and basic arithmetic helpers.
    /// All conversion algorithms are implemented manually to demonstrate the
    /// underlying mathematics; <see cref="System.Convert"/> is intentionally
    /// avoided in the core routines.
    /// </summary>
    public static class NumberConverter
    {
        /// <summary>Valid digits used for any supported base up to 16.</summary>
        private const string Digits = "0123456789ABCDEF";

        /// <summary>
        /// Converts a string representation of a number in the specified base
        /// into a signed 64-bit integer. The input may start with an optional
        /// '-' sign (for base 10 typically, but accepted for all bases).
        /// </summary>
        /// <param name="value">The textual number, e.g. "1A3F".</param>
        /// <param name="fromBase">Source base (2, 8, 10, or 16).</param>
        /// <returns>The decimal (base-10) <see cref="long"/> value.</returns>
        /// <exception cref="ArgumentException">If digits are invalid for the base.</exception>
        /// <exception cref="OverflowException">If the value does not fit in long.</exception>
        public static long ToDecimal(string value, int fromBase)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            ValidateBase(fromBase);
            value = value.Trim().ToUpperInvariant();

            bool negative = false;
            int index = 0;
            if (value[0] == '-')
            {
                negative = true;
                index = 1;
                if (value.Length == 1)
                    throw new ArgumentException("Минус без числа.");
            }

            // Horner's scheme: result = result * base + digit
            long result = 0;
            for (; index < value.Length; index++)
            {
                int digitValue = Digits.IndexOf(value[index]);
                if (digitValue < 0 || digitValue >= fromBase)
                    throw new ArgumentException(
                        $"Символ '{value[index]}' недопустим для основания {fromBase}.");

                checked
                {
                    result = result * fromBase + digitValue;
                }
            }

            return negative ? -result : result;
        }

        /// <summary>
        /// Converts a signed decimal <see cref="long"/> value into its textual
        /// representation in the specified target base.
        /// </summary>
        /// <param name="value">Decimal (base-10) value.</param>
        /// <param name="toBase">Target base (2, 8, 10, or 16).</param>
        /// <returns>The textual representation, e.g. "1A3F".</returns>
        public static string FromDecimal(long value, int toBase)
        {
            ValidateBase(toBase);

            if (value == 0)
                return "0";

            bool negative = value < 0;
            // Use unsigned magnitude to safely handle long.MinValue.
            ulong magnitude = negative ? (ulong)(-(value + 1)) + 1UL : (ulong)value;

            // Repeated division algorithm: collect remainders from least to
            // most significant, then reverse.
            var sb = new StringBuilder();
            while (magnitude > 0)
            {
                int remainder = (int)(magnitude % (ulong)toBase);
                sb.Append(Digits[remainder]);
                magnitude /= (ulong)toBase;
            }

            if (negative)
                sb.Append('-');

            // Reverse the accumulated characters.
            char[] chars = sb.ToString().ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// Convenience method: converts a textual number from one base directly
        /// to another by routing through base-10.
        /// </summary>
        public static string Convert(string value, int fromBase, int toBase)
        {
            long decValue = ToDecimal(value, fromBase);
            return FromDecimal(decValue, toBase);
        }

        /// <summary>
        /// Determines whether the given character is a valid digit for the
        /// specified base.
        /// </summary>
        public static bool IsValidDigit(char ch, int baseValue)
        {
            ValidateBase(baseValue);
            int idx = Digits.IndexOf(char.ToUpperInvariant(ch));
            return idx >= 0 && idx < baseValue;
        }

        /// <summary>
        /// Alternative conversion method implemented via <see cref="System.Convert"/>
        /// for cross-checking the manual algorithm during development. Not used
        /// by the calculator UI but kept for educational comparison.
        /// </summary>
        public static long ToDecimalBuiltIn(string value, int fromBase)
        {
            return System.Convert.ToInt64(value, fromBase);
        }

        private static void ValidateBase(int baseValue)
        {
            if (baseValue != 2 && baseValue != 8 && baseValue != 10 && baseValue != 16)
                throw new ArgumentException($"Основание {baseValue} не поддерживается.");
        }
    }
}
