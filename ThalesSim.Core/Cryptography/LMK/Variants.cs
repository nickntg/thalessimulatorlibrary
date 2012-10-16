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

namespace ThalesSim.Core.Cryptography.LMK
{
    public class Variants
    {
        private static readonly string[] SingleLengthVariants = new[] { "A6", "5A", "6A", "DE", "2B", "50", "74", "9C", "FA" };
        private static readonly string[] DoubleLengthVariants = new[] { "A6", "5A" };
        private static readonly string[] TripleLengthVariants = new[] { "6A", "DE", "2B" };

        public static string GetVariant(int index)
        {
            return SingleLengthVariants[index - 1];
        }

        public static string GetDoubleLengthVariant (int index)
        {
            return DoubleLengthVariants[index - 1];
        }

        public static string GetTripleLengthVariant (int index)
        {
            return TripleLengthVariants[index - 1];
        }
    }
}
