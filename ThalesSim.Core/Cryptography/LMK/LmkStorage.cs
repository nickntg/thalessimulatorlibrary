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

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ThalesSim.Core.Properties;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography.LMK
{
    /// <summary>
    /// This class loads and maintains the LMKs used by the simulator.
    /// </summary>
    public class LmkStorage
    {
        private static string _lmkStorageFile;

        /// <summary>
        /// Get/set the LMK file.
        /// </summary>
        public static string LmkStorageFile
        {
            get { return _lmkStorageFile; }
            set
            {
                _lmkStorageFile = value;
                _lmkOldStorageFile = value + ".old";
            }
        }

        private static string _lmkOldStorageFile;

        /// <summary>
        /// Get/set the old LMK file.
        /// </summary>
        public static string LmkOldStorageFile
        {
            get { return _lmkOldStorageFile; }
            set { _lmkOldStorageFile = value; }
        }

        private static bool _useOldLmkStorage = false;

        /// <summary>
        /// Get/set whether to use the old LMKs.
        /// </summary>
        public static bool UseOldLmkStorage
        {
            get { return _useOldLmkStorage; }
            set { _useOldLmkStorage = value; }
        }

        private static SortedList<LmkPair, string> _lmKs = new SortedList<LmkPair, string>();

        private static SortedList<LmkPair, string> _oldLmKs = new SortedList<LmkPair, string>();

        /// <summary>
        /// Returns the clear value of an LMK.
        /// </summary>
        /// <param name="pair">LMK pair.</param>
        /// <returns>LMK for pair.</returns>
        public static string Lmk (LmkPair pair)
        {
            return !UseOldLmkStorage ? _lmKs[pair] : _oldLmKs[pair];
        }

        /// <summary>
        /// Returns the clear value of an LMK variant.
        /// </summary>
        /// <param name="pair">LMK pair.</param>
        /// <param name="variant">LMK variant.</param>
        /// <returns>LMK for pair and variant.</returns>
        public static string LmkVariant (LmkPair pair, int variant)
        {
            var lmk = Lmk(pair);
            return variant == 0 ? lmk : lmk.XorHex(Variants.GetVariant(variant).PadRight(32, '0'));
        }

        /// <summary>
        /// Reads the LMKs from a file.
        /// </summary>
        /// <param name="storageFile">LMK storage file.</param>
        public static void ReadLmk (string storageFile)
        {
            LmkStorageFile = storageFile;
            _lmKs = ReadLmkFile(LmkStorageFile);
            _oldLmKs = ReadLmkFile(LmkOldStorageFile);
        }

        /// <summary>
        /// Generates test LMKs.
        /// </summary>
        /// <param name="writeLmks">True to write the LMKs to file.</param>
        public static void GenerateTestLmks(bool writeLmks = true)
        {
            if (string.IsNullOrEmpty(LmkStorageFile))
            {
                throw new InvalidOleVariantTypeException("No storage file specified");
            }

            const string staticLmks =
                @"; LMK Storage file
                01010101010101017902CD1FD36EF8BA
                20202020202020203131313131313131
                40404040404040405151515151515151
                61616161616161617070707070707070
                80808080808080809191919191919191
                A1A1A1A1A1A1A1A1B0B0B0B0B0B0B0B0
                C1C1010101010101D0D0010101010101
                E0E0010101010101F1F1010101010101
                1C587F1C13924FEF0101010101010101
                01010101010101010101010101010101
                02020202020202020404040404040404
                07070707070707071010101010101010
                13131313131313131515151515151515
                16161616161616161919191919191919
                1A1A1A1A1A1A1A1A1C1C1C1C1C1C1C1C
                23232323232323232525252525252525
                26262626262626262929292929292929
                2A2A2A2A2A2A2A2A2C2C2C2C2C2C2C2C
                2F2F2F2F2F2F2F2F3131313131313131
                01010101010101010101010101010101";
            using (var ms = new MemoryStream(staticLmks.GetBytes()))
            {
                _lmKs = ReadLmkFile(ms);
            }

            const string staticOldLmks =
                @"; LMK Storage file
                101010101010101F7902CD1FD36EF8BA
                202020202020202F3131313131313131
                404040404040404F5151515151515151
                616161616161616E7070707070707070
                808080808080808F9191919191919191
                A1A1A1A1A1A1A1AEB0B0B0B0B0B0B0B0
                C1C101010101010ED0D0010101010101
                E0E001010101010EF1F1010101010101
                1C587F1C13924FFE0101010101010101
                010101010101010E0101010101010101
                020202020202020E0404040404040404
                070707070707070E1010101010101010
                131313131313131F1515151515151515
                161616161616161F1919191919191919
                1A1A1A1A1A1A1A1F1C1C1C1C1C1C1C1C
                232323232323232F2525252525252525
                262626262626262F2929292929292929
                2A2A2A2A2A2A2A2F2C2C2C2C2C2C2C2C
                2F2F2F2F2F2F2FFE3131313131313131
                010101010101010E0101010101010101";

            using (var ms = new MemoryStream(staticOldLmks.GetBytes()))
            {
                _oldLmKs = ReadLmkFile(ms);
            }

            if (writeLmks)
            {
                WriteLmks();
            }
        }

        /// <summary>
        /// Writes the LMKs to file.
        /// </summary>
        public static void WriteLmks()
        {
            WriteLmks(LmkStorageFile, _lmKs);
            WriteLmks(LmkOldStorageFile, _oldLmKs);
        }

        /// <summary>
        /// Returns a string with the LMK values.
        /// </summary>
        /// <returns>String with LMK values.</returns>
        public static string DumpLmks()
        {
            var sb = new StringBuilder();
            for (var pair = LmkPair.Pair00_01; pair <= LmkPair.Pair34_35; pair++)
            {
                sb.AppendLine(Lmk(pair));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the LMK check value.
        /// </summary>
        /// <returns>LMK check value.</returns>
        public static string GenerateLmkCheckValue()
        {
            var text = Lmk(LmkPair.Pair00_01).XorHex(Lmk(LmkPair.Pair02_03));
            for (var pair = LmkPair.Pair04_05; pair <= LmkPair.Pair34_35; pair++)
            {
                text = text.XorHex(Lmk(pair));
            }

            return text.Substring(0, 16).XorHex(text.Substring(16));
        }

        /// <summary>
        /// Determines if the LMK storage has odd parity.
        /// </summary>
        /// <returns>True if all keys have odd parity.</returns>
        public static bool CheckLmkStorage()
        {
            for (var pair = LmkPair.Pair00_01; pair <= LmkPair.Pair34_35; pair++)
            {
                if (!Lmk(pair).IsParityOk(Parity.Odd))
                {
                    return false;
                }
            }

            return true;
        }

        private static SortedList<LmkPair, string> ReadLmkFile (string storageFile)
        {
            using (var sr = new StreamReader(storageFile, Encoding.Default))
            {
                return ReadLmkFile(sr.BaseStream);
            }
        }

        private static SortedList<LmkPair, string> ReadLmkFile (Stream stream)
        {
            var lst = new SortedList<LmkPair, string>();
            var index = LmkPair.Pair00_01;
            using (var sr = new StreamReader(stream))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                    {
                        lst.Add(index, line.Trim());
                        index++;
                    }
                }
            }
            return lst;
        }

        private static void WriteLmks(string storageFile, SortedList<LmkPair, string> lst)
        {
            using (var sw = new StreamWriter(storageFile, false, Encoding.Default))
            {
                sw.WriteLine("; LMK Storage File");
                for (var pair = LmkPair.Pair00_01; pair <= LmkPair.Pair34_35; pair++)
                {
                    sw.WriteLine(lst[pair]);
                }
            }
        }
    }
}
