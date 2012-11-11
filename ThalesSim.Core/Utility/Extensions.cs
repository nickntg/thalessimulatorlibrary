/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Data;
using System.Linq;
using System.Text;
using ThalesSim.Core.Cryptography;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Cryptography.PIN;

namespace ThalesSim.Core.Utility
{
    /// <summary>
    /// This extension class contains several helper methods.
    /// </summary>
    public static class Extensions
    {
        private static readonly Random RndMachine = new Random();

        #region Hex/binary/byte

        /// <summary>
        /// Determine if a string is numeric.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if string is numeric.</returns>
        public static bool IsNumeric (this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var chars = text.ToUpper().StripKeyScheme().ToCharArray();

            return !text.Where((t, i) => !char.IsDigit(chars[i])).Any();
        }

        /// <summary>
        /// Determine if a string is hexadecimal.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if string is hexadecimal.</returns>
        public static bool IsHex(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var chars = text.ToUpper().StripKeyScheme().ToCharArray();

            return !text.Where((t, i) => !char.IsDigit(chars[i]) && (chars[i] < 'A' || chars[i] > 'F')).Any();
        }

        /// <summary>
        /// Determines if a string contains binary digits only.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if string contains binary digits only.</returns>
        public static bool IsBinary(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            text = text.Replace("1", "").Replace("0", "");
            return (text.Length == 0);
        }

        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>String with hex characters.</returns>
        public static string GetHexString(this byte[] bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.GetLength(0); i++)
            {
                sb.AppendFormat("{0:X2}", bytes[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a hexadecimal string to a binary string.
        /// </summary>
        /// <param name="text">Hexadecimal string.</param>
        /// <returns>String with binary characters.</returns>
        public static string GetBinary(this string text)
        {
            if (!text.IsHex())
            {
                throw new InvalidOperationException("Text must be hexadecimal");
            }

            var sb = new StringBuilder();
            for (var i = 0; i < text.Length; i++)
            {
                sb.Append(Convert.ToString(Convert.ToInt32(text.Substring(i, 1), 16), 2).PadLeft(4, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a binary string to a hexadecimal string.
        /// </summary>
        /// <param name="text">Binary string.</param>
        /// <returns>Hexadecimal string.</returns>
        public static string FromBinary(this string text)
        {
            if (text.Length % 4 != 0)
            {
                throw new InvalidOperationException("Text length must be divisible by 4");
            }

            if (!text.IsBinary())
            {
                throw new InvalidOperationException(string.Format("String {0} is not binary", text));
            }

            var sb = new StringBuilder();
            for (var i = 0; i < text.Length; i += 4)
            {
                sb.Append(Convert.ToByte(text.Substring(i, 4), 2).ToString("X1"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts a string to an array of bytes.
        /// </summary>
        /// <param name="text">String to convert.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <returns>Converted byte array.</returns>
        public static byte[] GetBytes(this string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Converts a string to an array of bytes using
        /// the ANSI code page.
        /// </summary>
        /// <param name="text">String to convert.</param>
        /// <returns>Converted byte array.</returns>
        public static byte[] GetBytes(this string text)
        {
            return GetBytes(text,
                            Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage));
        }

        /// <summary>
        /// Converts a byte array to a string using
        /// the ANSI code page.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Converted string.</returns>
        public static string GetString(this byte[] bytes)
        {
            return
                Encoding.GetEncoding(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage).GetString(
                    bytes);
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array.
        /// </summary>
        /// <param name="text">Hexadecimal string.</param>
        /// <returns>Converted byte array.</returns>
        public static byte[] GetHexBytes(this string text)
        {
            if (text.Length % 2 != 0)
            {
                throw new InvalidOperationException("Text length must be even");
            }

            if (!text.IsHex())
            {
                throw new InvalidOperationException("Text must be hexadecimal");
            }

            var bytes = new byte[text.Length / 2];
            var index = 0;
            for (var i = 0; i < text.Length; i += 2)
            {
                bytes[index] = Convert.ToByte(text.Substring(i, 2), 16);
                index++;
            }

            return bytes;
        }

        /// <summary>
        /// Gets a hex/char dump for a byte array.
        /// </summary>
        /// <param name="bytes">Byte array to get dump for.</param>
        /// <returns>Hex/char dump.</returns>
        public static string GetDump (this byte[] bytes)
        {
            var sb = new StringBuilder();
            var hex = bytes.GetHexString();
            var chars = bytes.GetString();

            var str = string.Empty;
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(hex.Substring(i*2, 2) + " ");
                if (bytes[i] >= 30)
                {
                    str += chars[i];
                }
                else
                {
                    str += ".";
                }

                if ((i+1) % 8 == 0)
                {
                    sb.Append("| " + str);
                    sb.AppendLine();
                    str = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(str))
            {
                sb.Append(new string(' ', (8 - (bytes.Length % 8))*3) + "| " + str + "\r\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Performs an XOR operation on two hex strings
        /// after stripping them of their key scheme character.
        /// </summary>
        /// <param name="text">First string.</param>
        /// <param name="other">Second string.</param>
        /// <returns>XOR result.</returns>
        public static string XorHex(this string text, string other)
        {
            text = text.StripKeyScheme();
            other = other.StripKeyScheme();

            if (text.Length != other.Length)
            {
                throw new InvalidOperationException(string.Format("String {0} is different length than {1}", text, other));
            }

            var bText = text.GetHexBytes();
            var bOther = other.GetHexBytes();

            for (var i = 0; i < bText.GetLength(0); i++)
            {
                bText[i] = (byte)(bText[i] ^ bOther[i]);
            }

            return bText.GetHexString();
        }

        #endregion

        #region Key scheme

        /// <summary>
        /// Gets the character corresponding to a key scheme.
        /// </summary>
        /// <param name="scheme">Key scheme to get char for.</param>
        /// <returns>Corresponding key scheme character.</returns>
        public static string GetKeySchemeChar (this KeyScheme scheme)
        {
            switch (scheme)
            {
                case KeyScheme.DoubleLengthKeyAnsi:
                    return "X";
                case KeyScheme.SingleLengthKey:
                    return "Z";
                case KeyScheme.DoubleLengthKeyVariant:
                    return "U";
                case KeyScheme.TripleLengthKeyAnsi:
                    return "Y";
                case KeyScheme.TripleLengthKeyVariant:
                    return "T";
                default:
                    throw new InvalidCastException("Invalid key scheme");
            }
        }

        /// <summary>
        /// Gets a key scheme corresponding to a character.
        /// </summary>
        /// <param name="text">Key scheme character.</param>
        /// <returns>Corresponding key scheme.</returns>
        public static KeyScheme GetKeyScheme (this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new InvalidCastException("Cannot derive key scheme from empty/null string");
            }

            text = text.ToUpper().Substring(0,1);
            switch (text)
            {
                case "X":
                    return KeyScheme.DoubleLengthKeyAnsi;
                case "Z":
                    return KeyScheme.SingleLengthKey;
                case "U":
                    return KeyScheme.DoubleLengthKeyVariant;
                case "Y":
                    return KeyScheme.TripleLengthKeyAnsi;
                case "T":
                    return KeyScheme.TripleLengthKeyVariant;
                default:
                    throw new InvalidCastException(string.Format("Invalid key scheme {0}", text));
            }
        }

        /// <summary>
        /// Determines if a string starts with a key scheme character.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <returns>True if string starts with a key scheme character.</returns>
        public static bool StartsWithKeyScheme (this string text)
        {
            text = text.ToUpper();
            return text.StartsWith(GetKeySchemeChar(KeyScheme.DoubleLengthKeyAnsi)) ||
                   text.StartsWith(GetKeySchemeChar(KeyScheme.DoubleLengthKeyVariant)) ||
                   text.StartsWith(GetKeySchemeChar(KeyScheme.SingleLengthKey)) ||
                   text.StartsWith(GetKeySchemeChar(KeyScheme.TripleLengthKeyAnsi)) ||
                   text.StartsWith(GetKeySchemeChar(KeyScheme.TripleLengthKeyVariant));
        }

        /// <summary>
        /// Removes the starting key scheme character from a string.
        /// </summary>
        /// <param name="text">String to remove key scheme from.</param>
        /// <returns>String without key scheme character.</returns>
        public static string StripKeyScheme (this string text)
        {
            return StartsWithKeyScheme(text) ? text.Substring(1) : text;
        }

        /// <summary>
        /// Determines the length of a key corresponding to 
        /// a key scheme.
        /// </summary>
        /// <param name="scheme">Key scheme.</param>
        /// <returns>Key length.</returns>
        public static int GetKeyLength (this KeyScheme scheme)
        {
            switch (scheme)
            {
                case KeyScheme.SingleLengthKey:
                    return 16;
                case KeyScheme.DoubleLengthKeyAnsi:
                case KeyScheme.DoubleLengthKeyVariant:
                    return 32;
                default:
                    return 48;
            }
        }

        #endregion

        #region Key length
        
        /// <summary>
        /// Determines a key's length.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Length of key.</returns>
        public static KeyLength GetKeyLength(this string key)
        {
            switch (key.Length)
            {
                case 16:
                    return KeyLength.SingleLength;
                case 32:
                    return KeyLength.DoubleLength;
                case 48:
                    return KeyLength.TripleLength;
                default:
                    throw new InvalidOperationException(string.Format("{0} has an invalid length", key));
            }
        }

        #endregion

        #region Parity

        /// <summary>
        /// Checks the parity of a hexadecimal string.
        /// </summary>
        /// <param name="text">String to check.</param>
        /// <param name="parity">Parity to check for.</param>
        /// <returns>True of parity is ok.</returns>
        public static bool IsParityOk (this string text, Parity parity)
        {
            if (parity == Parity.None)
            {
                return true;
            }

            text = text.StripKeyScheme();

            if (!text.IsHex())
            {
                throw new InvalidOperationException("Text must be hexadecimal");
            }

            var bytes = text.GetHexBytes();
            if (bytes.Any(b => !b.IsParityOk(parity)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the parity of a byte is ok.
        /// </summary>
        /// <param name="b">Byte to check.</param>
        /// <param name="parity">Parity to check for.</param>
        /// <returns>True if parity is ok.</returns>
        public static bool IsParityOk(this byte b, Parity parity)
        {
            if (parity == Parity.None)
            {
                return true;
            }

            var ones = Convert.ToString(b, 2).Replace("0", "");
            return (ones.Length % 2 != 0 || parity != Parity.Odd) && (ones.Length % 2 != 1 || parity != Parity.Even);
        }

        /// <summary>
        /// Enforces a parity setting on a hexadecimal string.
        /// </summary>
        /// <param name="text">String to use.</param>
        /// <param name="parity">Parity to enforce.</param>
        /// <returns>String with parity.</returns>
        public static string MakeParity (this string text, Parity parity)
        {
            if (parity == Parity.None)
            {
                return text;
            }

            var schemeChar = string.Empty;
            if (text.StartsWithKeyScheme())
            {
                schemeChar = text.Substring(0, 1);
            }

            var bytes = text.GetHexBytes();

            for (var i = 0; i < bytes.Length; i++)
            {
                if (!bytes[i].IsParityOk(parity))
                {
                    bytes[i] = (byte) (bytes[i] ^ 0x01);
                }
            }

            return schemeChar + bytes.GetHexString();
        }

        /// <summary>
        /// Generates a random hexadecimal string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="scheme">Scheme to use.</param>
        /// <param name="parity">Parity to use.</param>
        /// <returns>Generated random key.</returns>
        public static string RandomKey (this string text, KeyScheme scheme, Parity parity)
        {
            switch (scheme.GetKeyLength())
            {
                case 16:
                    return RandomKey(text, parity);
                case 32:
                    return (RandomKey(text, parity) + RandomKey(text, parity)).MakeParity(parity);
                default:
                    return (RandomKey(text, parity) + RandomKey(text, parity) + RandomKey(text, parity)).MakeParity(parity);
            }
        }

        /// <summary>
        /// Generates a random key with odd parity.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="scheme">Scheme to use.</param>
        /// <returns>Generated key.</returns>
        public static string RandomKey (this string text, KeyScheme scheme)
        {
            return RandomKey(text, scheme, Parity.Odd);
        }

        /// <summary>
        /// Generates a random single-length key
        /// with odd parity.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Generated key.</returns>
        public static string RandomKey (this string text)
        {
            return RandomKey(text, Parity.Odd);
        }

        /// <summary>
        /// Generates a random single-length key.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parity">Parity to use.</param>
        /// <returns>Generated key.</returns>
        public static string RandomKey (this string text, Parity parity)
        {
            var sb = new StringBuilder();
            for (var i = 1; i <= 16; i++)
            {
                sb.AppendFormat("{0:X1}", RndMachine.Next(0, 16));
            }

            return sb.ToString().MakeParity(parity);
        }

        #endregion

        #region LMK

        /// <summary>
        /// Determines the LMK pair and variant from
        /// a two-digit code.
        /// </summary>
        /// <param name="text">Two-digit LMK code.</param>
        /// <param name="variant">LMK variant.</param>
        /// <returns>LMK pair.</returns>
        public static LmkPair GetLmkPairFromTwoDigits (this string text, out int variant)
        {
            variant = 0;

            switch (text.ToUpper())
            {
                case "00":
                    return LmkPair.Pair04_05;
                case "01":
                    return LmkPair.Pair06_07;
                case "02":
                    return LmkPair.Pair14_15;
                case "03":
                    return LmkPair.Pair16_17;
                case "04":
                    return LmkPair.Pair18_19;
                case "05":
                    return LmkPair.Pair20_21;
                case "06":
                    return LmkPair.Pair22_23;
                case "07":
                    return LmkPair.Pair24_25;
                case "08":
                    return LmkPair.Pair26_27;
                case "09":
                    return LmkPair.Pair28_29;
                case "0A":
                    return LmkPair.Pair30_31;
                case "0B":
                    return LmkPair.Pair32_33;
                case "10":
                    variant = 1;
                    return LmkPair.Pair04_05;
                case "42":
                    variant = 4;
                    return LmkPair.Pair14_15;
                default:
                    throw new InvalidCastException(string.Format("Cannot parse {0} as an LMK pair from two digits", text));
            }
        }

        /// <summary>
        /// Determines the LMK pair from an LMK code.
        /// </summary>
        /// <param name="text">LMK key code.</param>
        /// <returns>LMK pair.</returns>
        public static LmkPair GetLmkPair (this string text)
        {
            switch (text.ToUpper())
            {
                case "00":
                    return LmkPair.Pair04_05;
                case "01":
                    return LmkPair.Pair06_07;
                case "02":
                    return LmkPair.Pair14_15;
                case "03":
                    return LmkPair.Pair16_17;
                case "04":
                    return LmkPair.Pair18_19;
                case "05":
                    return LmkPair.Pair20_21;
                case "06":
                    return LmkPair.Pair22_23;
                case "07":
                    return LmkPair.Pair24_25;
                case "08":
                    return LmkPair.Pair26_27;
                case "09":
                    return LmkPair.Pair28_29;
                case "0A":
                    return LmkPair.Pair30_31;
                case "0B":
                    return LmkPair.Pair32_33;
                case "0C":
                    return LmkPair.Pair34_35;
                case "0D":
                    return LmkPair.Pair36_37;
                case "0E":
                    return LmkPair.Pair38_39;
                default:
                    throw new InvalidCastException(string.Format("Cannot parse {0} as an LMK pair", text));
            }
        }

        /// <summary>
        /// Returns a string representing an LMK pair.
        /// </summary>
        /// <param name="pair">LMK pair.</param>
        /// <returns>LMK pair code.</returns>
        public static string GetLmkPairCode (this LmkPair pair)
        {
            switch (pair)
            {
                case LmkPair.Pair04_05:
                    return "00";
                case LmkPair.Pair06_07:
                    return "01";
                case LmkPair.Pair14_15:
                    return "02";
                case LmkPair.Pair16_17:
                    return "03";
                case LmkPair.Pair18_19:
                    return "04";
                case LmkPair.Pair20_21:
                    return "05";
                case LmkPair.Pair22_23:
                    return "06";
                case LmkPair.Pair24_25:
                    return "07";
                case LmkPair.Pair26_27:
                    return "08";
                case LmkPair.Pair28_29:
                    return "09";
                case LmkPair.Pair30_31:
                    return "0A";
                case LmkPair.Pair32_33:
                    return "0B";
                case LmkPair.Pair34_35:
                    return "0C";
                case LmkPair.Pair36_37:
                    return "0D";
                case LmkPair.Pair38_39:
                    return "0E";
                default:
                    throw new InvalidCastException(string.Format("Cannot generate LMK pair code for {0}", pair));
            }
        }

        #endregion

        #region PIN

        /// <summary>
        /// Determines if a PIN block format is supported.
        /// </summary>
        /// <param name="text">PIN block format.</param>
        /// <returns>True if PIN block format is supported.</returns>
        public static bool IsPinBlockFormatSupported (this string text)
        {
            try
            {
                var format = text.GetPinBlockFormat();
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines the PIN block format from a string.
        /// </summary>
        /// <param name="text">String with PIN block format code.</param>
        /// <returns>PIN block format.</returns>
        public static PinBlockFormat GetPinBlockFormat (this string text)
        {
            switch (text)
            {
                case "01":
                    return PinBlockFormat.AnsiX98;
                case "02":
                    return PinBlockFormat.Docutel;
                case "03":
                    return PinBlockFormat.Diebold;
                case "04":
                    return PinBlockFormat.Plus;
                case "05":
                    return PinBlockFormat.Iso94564_1;
                default:
                    throw new InvalidCastException(string.Format("PIN block format {0} not valid or unsupported", text));
            }
        }

        /// <summary>
        /// Returns a PIN block format code corresponding
        /// to a PIN block format.
        /// </summary>
        /// <param name="format">PIN block format.</param>
        /// <returns>PIN block format code.</returns>
        public static string GetPinBlockFormat (this PinBlockFormat format)
        {
            switch (format)
            {
                case PinBlockFormat.AnsiX98:
                    return "01";
                case PinBlockFormat.Docutel:
                    return "02";
                case PinBlockFormat.Diebold:
                    return "03";
                case PinBlockFormat.Plus:
                    return "04";
                case PinBlockFormat.Iso94564_1:
                    return "05";
                default:
                    throw new InvalidCastException(string.Format("PIN block format {0} not valid or unsupported", format));
            }
        }

        /// <summary>
        /// Creates a clear PIN block for a PIN.
        /// </summary>
        /// <param name="pin">PIN to create PIN block for.</param>
        /// <param name="accountOrPadding">Account or padding string, depending
        /// on the PIN block format.</param>
        /// <param name="format">PIN block format to use.</param>
        /// <returns>Clear PIN block.</returns>
        public static string GetPinBlock (this string pin, string accountOrPadding, PinBlockFormat format)
        {
            switch (format)
            {
                case PinBlockFormat.AnsiX98:
                    if (accountOrPadding.Length < 12)
                    {
                        throw new InvalidOperationException("Account length must be equal or greater than 12 to use ANSIX9.8");
                    }

                    var s1 = (pin.Length.ToString().PadLeft(2, '0') + pin).PadRight(16, 'F');
                    var s2 = accountOrPadding.PadLeft(16, '0');

                    return s1.XorHex(s2);
                case PinBlockFormat.Diebold:
                    return pin.PadRight(16, 'F');
                case PinBlockFormat.Docutel:
                    if (pin.Length > 6)
                    {
                        throw new InvalidOperationException("PIN length must be less or equal to 6 to use Docutel");
                    }

                    var s3 = pin.Length.ToString() + pin.PadLeft(6, '0');
                    return s3 + accountOrPadding.Substring(0, 16 - s3.Length);
                case PinBlockFormat.Iso94564_1:
                    var s4 = ("0" + pin.Length.ToString() + pin).PadLeft(16, 'F');
                    var s5 = "0000" + accountOrPadding.Substring(0, 12);
                    return s4.XorHex(s5);
                case PinBlockFormat.Plus:
                    throw new NotSupportedException("Unsupported PIN block format PLUS");
                default:
                    throw new NotSupportedException(string.Format("Unsupported PIN block format {0}", format));
            }
        }

        /// <summary>
        /// Gets the PIN from a clear PIN block.
        /// </summary>
        /// <param name="clearPinBlock">Clear PIN block.</param>
        /// <param name="accountOrPadding">Account or padding string.</param>
        /// <param name="format">PIN block format.</param>
        /// <returns>PIN of the PIN block.</returns>
        public static string GetPin (this string clearPinBlock, string accountOrPadding, PinBlockFormat format)
        {
            switch (format)
            {
                case PinBlockFormat.AnsiX98:
                    var s1 = accountOrPadding.PadLeft(16, '0').XorHex(clearPinBlock);
                    return s1.Substring(2, Convert.ToInt32(s1.Substring(0, 2)));
                case PinBlockFormat.Diebold:
                    return clearPinBlock.Replace("F", "");
                case PinBlockFormat.Docutel:
                    return clearPinBlock.Substring(1, Convert.ToInt32(clearPinBlock.Substring(0, 1)));
                case PinBlockFormat.Iso94564_1:
                    var s2 = "0000" + accountOrPadding.PadLeft(16, 'F').XorHex(clearPinBlock);
                    var pl = Convert.ToInt32(s2.Substring(11, 1));
                    return s2.Substring(12, pl);
                case PinBlockFormat.Plus:
                    throw new NotSupportedException("Unsupported PIN block format PLUS");
                default:
                    throw new NotSupportedException(string.Format("Unsupported PIN block format {0}", format));
            }
        }

        #endregion

        #region File/directory

        /// <summary>
        /// Appends a trailing slash to a directory if needed.
        /// </summary>
        /// <param name="text">Directory path.</param>
        /// <returns>Directory path with trailing slash.</returns>
        public static string AppendTrailingSeparator (this string text)
        {
            var sep = "\\";
            if (!text.Contains("\\"))
            {
                sep = "/";
            }

            return !text.EndsWith(sep) ? text + sep : text;
        }

        #endregion

        #region Data

        /// <summary>
        /// Determines if a column of a data row is null.
        /// </summary>
        /// <param name="dr">Data row.</param>
        /// <param name="column">Column to use.</param>
        /// <returns>True if the column is null.</returns>
        public static bool IsNotNull(this DataRow dr, string column)
        {
            try
            {
                return dr[column] != DBNull.Value;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        #endregion

        #region Decimalisation

        /// <summary>
        /// Decimalises a string using a decimalisation table.
        /// </summary>
        /// <param name="text">String to decimalise.</param>
        /// <param name="decimalisationTable">Decimalisation table.</param>
        /// <returns>Decimalised string.</returns>
        public static string Decimalise (this string text, string decimalisationTable)
        {
            const string emptyDecTable = "FFFFFFFFFFFFFFFF";
            const string defaultDecTable = "9876543210123456";

            if (decimalisationTable == emptyDecTable)
            {
                decimalisationTable = defaultDecTable;
            }

            var output = string.Empty;
            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (ch >= '0' && ch <= '9')
                {
                    output = output + ch;
                }
                else
                {
                    var repIdx = (GetBytes(new string(ch, 1))[0] - 65) + 10;
                    output = output + decimalisationTable[repIdx];
                }
            }

            return output;
        }

        #endregion

        #region Add/subtract

        public static string AddWithoutCarry (this string text, string toAdd)
        {
            if (text.Length != toAdd.Length)
            {
                throw new InvalidOperationException("Cannot add without carry two unequal strings");
            }

            if (!text.IsNumeric() || !toAdd.IsNumeric())
            {
                throw new InvalidOperationException("Not numeric string or strings");
            }

            var output = string.Empty;
            for (var i = 0; i < text.Length; i++)
            {
                output = output + (Convert.ToInt32(text.Substring(i,1)) + Convert.ToInt32(toAdd.Substring(i,1)))%10;
            }

            return output;
        }

        #endregion
    }
}
