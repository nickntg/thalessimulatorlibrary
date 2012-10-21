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
using System.Text;

namespace ThalesSim.Core.Message
{
    public class MessageKeyValuePairs
    {
        private readonly SortedList<string, string> _pairs = new SortedList<string, string>();

        public int Count { get { return _pairs.Count; } }

        public void Add (string key, string value)
        {
            _pairs.Add(key, value);
        }

        public bool ContainsKey (string key)
        {
            return _pairs.ContainsKey(key);
        }

        public string Item (string key)
        {
            return _pairs[key];
        }

        public string ItemOptional (string key)
        {
            return ContainsKey(key) ? Item(key) : string.Empty;
        }

        public string ItemCombination (string key1, string key2)
        {
            return ItemOptional(key1) + ItemOptional(key2);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var key in _pairs.Keys)
            {
                sb.AppendFormat("[Key,Value]=[{0},{1}]{2}", key, _pairs[key], "\r\n");
            }
            return sb.ToString();
        }
    }
}
