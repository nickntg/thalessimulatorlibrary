using System;
using Ardalis.GuardClauses;
using ThalesSimulatorLibrary.Core.Utility;

namespace ThalesSimulatorLibrary.Core.Cryptography.PIN
{
    public static class Extensions
    {
        public static string GetPin(this string pinBlock, PinBlockFormat format, string accountOrPadding = null)
        {
            GuardAgainstEmptyAndExpression(pinBlock, nameof(pinBlock), "Invalid PIN block length", s => s.Length == 16);

            switch (format)
            {
                case PinBlockFormat.AnsiX98:
                    return pinBlock.GetPinAnsiX98(accountOrPadding);
                default:
                    throw new ArgumentException($"PIN block format {format} not supported");
            }
        }

        public static string GetPinBlock(this string pin, PinBlockFormat format, string accountOrPadding = null)
        {
            Guard.Against.NullOrEmpty(pin, nameof(pin), "PIN must have a value");

            switch (format)
            {
                case PinBlockFormat.AnsiX98:
                    return pin.GetPinBlockAnsiX98(accountOrPadding);
                default:
                    throw new ArgumentException($"PIN block format {format} not supported");
            }
        }

        private static string GetPinAnsiX98(this string pinBlock, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var paddedAccount = accountOrPadding.PadLeft(16, '0');
            var xor = paddedAccount.Xor(pinBlock);
            return xor.Substring(2, Convert.ToInt32(xor[..2]));
        }

        private static string GetPinBlockAnsiX98(this string pin, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var s1 = $"{pin.Length.ToString().PadLeft(2, '0')}{pin}".PadRight(16, 'F');
            var s2 = accountOrPadding.PadLeft(16, '0');

            return s1.Xor(s2);
        }

        private static void GuardAgainstEmptyAndExpression(string input, string name, string message,
            Func<string, bool> predicate)
        {
            Guard.Against.NullOrEmpty(input, name, message);
            Guard.Against.InvalidInput(input, name, predicate, message);
        }
    }
}
