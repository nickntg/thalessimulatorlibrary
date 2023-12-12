using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ThalesSimulatorLibrary.Core.Utility;

namespace ThalesSimulatorLibrary.Core.Cryptography.LMK
{
    public class Storage
    {
        private static Dictionary<LmkPair, string>[] _lmks;
        private static string _lmkFile = string.Empty;

        public static string Lmk(LmkPair lmkPair)
        {
            return _lmks[0][lmkPair];
        }

        public static string Lmk(LmkPair lmkPair, string variant)
        {
            return variant == "0" 
                ? _lmks[0][lmkPair]
                : _lmks[0][lmkPair].Xor(LmkHexVariants.GetVariant(Convert.ToInt32(variant)).PadRight(32, '0'));
        }

        public static string Lmk(LmkPair lmkPair, int lmkIdentifier)
        {
            AutomaticallyCreateLmks(lmkIdentifier);

            return _lmks[lmkIdentifier][lmkPair];
        }

        public static string Lmk(LmkPair lmkPair, string variant, int lmkIdentifier)
        {
            AutomaticallyCreateLmks(lmkIdentifier);

            return variant == "0"
                ? _lmks[lmkIdentifier][lmkPair]
                : _lmks[lmkIdentifier][lmkPair].Xor(LmkHexVariants.GetVariant(Convert.ToInt32(variant)).PadRight(32, '0'));
        }

        public static bool CheckLmkStorage()
        {
            foreach (var lmkSet in _lmks)
            {
                if (lmkSet == null)
                {
                    continue;
                }

                for (var pair = LmkPair.Pair0001; pair <= LmkPair.Pair3839; pair++)
                {
                    if (!lmkSet[pair].HasParity(Parity.Odd))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void ReadLmks(string lmkFile)
        {
            if (string.IsNullOrEmpty(lmkFile))
            {
                throw new InvalidOperationException("LMK file is null or empty");
            }

            if (!File.Exists(lmkFile))
            {
                CreateLmks(lmkFile);
            }

            _lmkFile = lmkFile;
            _lmks = new Dictionary<LmkPair, string>[100];
            
            for (var i = 0; i < 100; i++)
            {
                ReadLmks(lmkFile, i);
            }
        }
        
        private static void AutomaticallyCreateLmks(int lmkIdentifier)
        {
            if (_lmks[lmkIdentifier] != null)
            {
                return;
            }

            var contents = new StringBuilder();
            contents.AppendLine($"; Automatically generated LMKs, identifier {lmkIdentifier}");

            _lmks[lmkIdentifier] = new Dictionary<LmkPair, string>();

            var rnd = new Random();
            for (var pair = LmkPair.Pair0001; pair <= LmkPair.Pair3839; pair++)
            {
                var key = string.Empty;
                for (var i = 1; i <= 16; i++)
                {
                    var b = (byte)rnd.Next(0, 255);
                    b = b.MakeParity(Parity.Odd);
                    key += Convert.ToString(b, 16).PadLeft(2, '0');
                }

                contents.AppendLine(key);
                _lmks[lmkIdentifier][pair] = key;
            }

            CreateLmks(_lmkFile, lmkIdentifier, contents.ToString());
        }

        private static void ReadLmks(string lmkFile, int lmkIdentifier)
        {
            if (lmkIdentifier > 0)
            {
                lmkFile = $"{lmkFile}.{lmkIdentifier}";
            }

            if (!File.Exists(lmkFile))
            {
                return;
            }

            _lmks[lmkIdentifier] = new Dictionary<LmkPair, string>();

            var lines = File.ReadAllLines(lmkFile);
            var pair = LmkPair.Pair0001;
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith(';'))
                {
                    continue;
                }

                _lmks[lmkIdentifier].Add(pair, line);
                pair++;
            }
        }

        private static void CreateLmks(string lmkFile)
        {
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

            CreateLmks(lmkFile, 0, staticLmks);
        }

        private static void CreateLmks(string lmkFile, int lmkIdentifier, string contents)
        {
            if (lmkIdentifier > 0)
            {
                lmkFile = $"{lmkFile}.{lmkIdentifier}";
            }

            File.WriteAllText(lmkFile, contents);
        }
    }
}
