using System.Collections.Generic;
using ThalesSimulatorLibrary.Core.Exceptions;

namespace ThalesSimulatorLibrary.Core.Cryptography.Key
{
    public static class Extensions
    {
        private const string DoubleLengthAnsi = "X";
        private const string DoubleLengthVariant = "U";
        private const string SingleLengthAnsi = "Z";
        private const string TripleLengthAnsi = "Y";
        private const string TripleLengthVariant = "T";
        private static readonly List<string> KeySchemeTags = new() { DoubleLengthAnsi, DoubleLengthVariant, SingleLengthAnsi, TripleLengthAnsi, TripleLengthVariant };


        public static string GetKeySchemeTag(this KeyScheme keyScheme)
        {
            return keyScheme switch
            {
                KeyScheme.DoubleLengthAnsi => DoubleLengthAnsi,
                KeyScheme.DoubleLengthVariant => DoubleLengthVariant,
                KeyScheme.SingleLengthAnsi => SingleLengthAnsi,
                KeyScheme.TripleLengthAnsi => TripleLengthAnsi,
                KeyScheme.TripleLengthVariant => TripleLengthVariant,
                _ => throw new InvalidKeySchemeException($"Invalid key scheme {keyScheme}")
            };
        }

        public static KeyScheme GetKeyScheme(this string keyTag)
        {
            if (string.IsNullOrEmpty(keyTag))
            {
                return KeyScheme.SingleLengthAnsi;
            }

            return keyTag.ToUpper() switch
            {
                DoubleLengthAnsi => KeyScheme.DoubleLengthAnsi,
                DoubleLengthVariant => KeyScheme.DoubleLengthVariant,
                SingleLengthAnsi => KeyScheme.SingleLengthAnsi,
                TripleLengthAnsi => KeyScheme.TripleLengthAnsi,
                TripleLengthVariant => KeyScheme.TripleLengthVariant,
                _ => throw new InvalidKeyTagException($"Invalid key tag {keyTag}")
            };
        }

        public static string StripKeySchemeTag(this string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return key;
            }

            if (KeySchemeTags.Contains(key[..1]))
            {
                return key[1..];
            }

            return key;
        }
    }
}