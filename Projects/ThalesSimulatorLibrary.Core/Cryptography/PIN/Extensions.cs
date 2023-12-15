﻿using System;
using System.Runtime.CompilerServices;
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
                case PinBlockFormat.Docutel:
                    return pinBlock.GetPinDocutel();
                case PinBlockFormat.Diebold:
                    return pinBlock.GetPinDiebold();
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
                case PinBlockFormat.Docutel:
                    return pin.GetPinBlockDocutel(accountOrPadding);
                case PinBlockFormat.Diebold:
                    return pin.GetPinBlockDiebold();
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

        private static void GuardAgainstEmptyAndExpression(string input, string name, string message,
            Func<string, bool> predicate)
        {
            Guard.Against.NullOrEmpty(input, name, message);
            Guard.Against.InvalidInput(input, name, predicate, message);
        }
    }
}
