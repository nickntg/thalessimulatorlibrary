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

            return format switch
            {
                PinBlockFormat.AnsiX98 => pinBlock.GetPinAnsiX98(accountOrPadding),
                PinBlockFormat.Docutel => pinBlock.GetPinDocutel(),
                PinBlockFormat.Diebold => pinBlock.GetPinDiebold(),
                PinBlockFormat.Plus => pinBlock.GetPinPlus(accountOrPadding),
                PinBlockFormat.Iso95641Format1 => pinBlock.GetPinIso95641(),
                PinBlockFormat.Emv => pinBlock.GetPinEmv(),
                PinBlockFormat.PayNowPayLater => pinBlock.GetPinPayNowPayLater(accountOrPadding),
                PinBlockFormat.Iso95641Format3 => pinBlock.GetPinIso95643(accountOrPadding),
                _ => throw new ArgumentException($"PIN block format {format} not supported")
            };
        }

        public static string GetPinBlock(this string pin, PinBlockFormat format, string accountOrPadding = null)
        {
            Guard.Against.NullOrEmpty(pin, nameof(pin), "PIN must have a value");

            return format switch
            {
                PinBlockFormat.AnsiX98 => pin.GetPinBlockAnsiX98(accountOrPadding),
                PinBlockFormat.Docutel => pin.GetPinBlockDocutel(accountOrPadding),
                PinBlockFormat.Diebold => pin.GetPinBlockDiebold(),
                PinBlockFormat.Plus => pin.GetPinBlockPlus(accountOrPadding),
                PinBlockFormat.Iso95641Format1 => pin.GetPinBlockIso95641(),
                PinBlockFormat.Emv => pin.GetPinBlockEmv(),
                PinBlockFormat.PayNowPayLater => pin.GetPinBlockPayNowPayLater(accountOrPadding),
                PinBlockFormat.Iso95641Format3 => pin.GetPinBlockIso95643(accountOrPadding),
                _ => throw new ArgumentException($"PIN block format {format} not supported")
            };
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

        private static string GetPinDiebold(this string pinBlock)
        {
            return pinBlock.TrimEnd('F');
        }

        private static string GetPinBlockDiebold(this string pin)
        {
            return pin.PadRight(16, 'F');
        }

        private static string GetPinDocutel(this string pinBlock)
        {
            return pinBlock.Substring(1, Convert.ToInt32(pinBlock[..1]));
        }

        private static string GetPinBlockDocutel(this string pin, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(pin, nameof(pin), "PIN must be up to 6 digits", s => s.Length <= 6);
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 9 digits long", s => s.Length >= 9);

            return $"{pin.Length}{pin.PadRight(6, '0')}{accountOrPadding[..9]}";
        }

        private static string GetPinPlus(this string pinBlock, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var s1 = accountOrPadding[..12].PadLeft(16, '0');
            var s2 = pinBlock.Xor(s1);

            return s2.Substring(2, Convert.ToInt32(s2.Substring(1, 1)));
        }

        private static string GetPinBlockPlus(this string pin, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var s1 = $"0{pin.Length}{pin}".PadRight(16, 'F');
            var s2 = accountOrPadding[..12].PadLeft(16, '0');

            return s1.Xor(s2);
        }

        private static string GetPinIso95641(this string pinBlock)
        {
            return pinBlock.Substring(2, HexLength(pinBlock.Substring(1, 1)));
        }

        private static string GetPinBlockIso95641(this string pin)
        {
            return $"1{pin.Length:X}{pin}".PadRight(16, '0');
        }

        private static string GetPinEmv(this string pinBlock)
        {
            return pinBlock.Substring(2, HexLength(pinBlock[1..2]));
        }

        private static string GetPinBlockEmv(this string pin)
        {
            return $"2{pin.Length:X}{pin}".PadRight(16, 'F');
        }

        private static string GetPinPayNowPayLater(this string pinBlock, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var s2 = $"0000{accountOrPadding.Substring(accountOrPadding.Length - 12)}";
            var s1 = s2.Xor(pinBlock);

            return GetPinEmv(s1);
        }

        private static string GetPinBlockPayNowPayLater(this string pin, string accountOrPadding)
        {
            GuardAgainstEmptyAndExpression(accountOrPadding, nameof(accountOrPadding), "Account/padding must be at least 12 digits long", s => s.Length >= 12);

            var s1 = GetPinBlockEmv(pin);
            var s2 = $"0000{accountOrPadding.Substring(accountOrPadding.Length - 12)}";

            return s1.Xor(s2);
        }

        private static string GetPinIso95643(this string pinBlock, string accountOrPadding)
        {
            Guard.Against.NullOrEmpty(accountOrPadding, nameof(accountOrPadding));

            var s2 = $"0000{accountOrPadding.Substring(accountOrPadding.Length - 12)}";
            var s1 = s2.Xor(pinBlock);

            return s1.Substring(2, HexLength(pinBlock[1..2]));
        }

        private static string GetPinBlockIso95643(this string pin, string accountOrPadding)
        {
            Guard.Against.NullOrEmpty(accountOrPadding, nameof(accountOrPadding));

            var s1 = $"3{pin.Length:X}{pin}".PadRight(16, 'F');
            accountOrPadding = accountOrPadding.PadLeft(12, '0');
            var s2 = $"0000{accountOrPadding.Substring(accountOrPadding.Length - 12)}";
            return s1.Xor(s2);
        }

        private static void GuardAgainstEmptyAndExpression(string input, string name, string message,
            Func<string, bool> predicate)
        {
            Guard.Against.NullOrEmpty(input, name, message);
            Guard.Against.InvalidInput(input, name, predicate, message);
        }

        private static int HexLength(string hexChar)
        {
            return "0123456789ABCDEF".IndexOf(hexChar, StringComparison.Ordinal);
        }
    }
}
