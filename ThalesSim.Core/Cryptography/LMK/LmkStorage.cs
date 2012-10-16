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
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography.LMK
{
    public class LmkStorage
    {
        private static string _lmkStorageFile;
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
        public static string LmkOldStorageFile
        {
            get { return _lmkOldStorageFile; }
            set { _lmkOldStorageFile = value; }
        }

        public static bool UseOldLmkStorage { get; set; }

        private static SortedList<LmkPair, string> _lmKs = new SortedList<LmkPair, string>();

        private static SortedList<LmkPair, string> _oldLmKs = new SortedList<LmkPair, string>();

        public static string Lmk (LmkPair pair)
        {
            return !UseOldLmkStorage ? _lmKs[pair] : _oldLmKs[pair];
        }

        public static string LmkVariant (LmkPair pair, int variant)
        {
            var lmk = Lmk(pair);
            return variant == 0 ? lmk : lmk.XorHex(Variants.GetDoubleLengthVariant(variant).PadRight(32, '0'));
        }

        public static void ReadLmk (string storageFile)
        {
            LmkStorageFile = storageFile;
            _lmKs = ReadLmkFile(LmkStorageFile);
            _oldLmKs = ReadLmkFile(LmkOldStorageFile);
        }

        public static void GenerateTestLmks()
        {
            
        }

        private static SortedList<LmkPair, string> ReadLmkFile (string storageFile)
        {
            var lst = new SortedList<LmkPair, string>();
            var index = LmkPair.Pair00_01;
            using (var sr = new StreamReader(storageFile, System.Text.Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith(";"))
                    {
                        lst.Add(index, line);
                        index++;
                    }
                }
            }
            return lst;
        }
    }
}
