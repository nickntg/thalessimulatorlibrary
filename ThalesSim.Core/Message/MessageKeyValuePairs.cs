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
    /// <summary>
    /// This class holds key/value pairs of parsed fields.
    /// </summary>
    public class MessageKeyValuePairs
    {
        private readonly SortedList<string, string> _pairs = new SortedList<string, string>();

        /// <summary>
        /// Get the number of key/value pairs
        /// stored in this instance.
        /// </summary>
        public int Count { get { return _pairs.Count; } }

        /// <summary>
        /// Adds a new key/value pair.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public void Add (string key, string value)
        {
            _pairs.Add(key, value);
        }

        /// <summary>
        /// Determines if a key is present in this instance.
        /// </summary>
        /// <param name="key">Key to look for.</param>
        /// <returns>True if key is present.</returns>
        public bool ContainsKey (string key)
        {
            return _pairs.ContainsKey(key);
        }

        /// <summary>
        /// Returns a value stored in this instance.
        /// </summary>
        /// <param name="key">Key to value.</param>
        /// <returns>Value corresponding to key.</returns>
        public string Item (string key)
        {
            return _pairs[key];
        }

        /// <summary>
        /// Returns an optional item stored in this instance.
        /// </summary>
        /// <param name="key">Key to optional value.</param>
        /// <returns>Value corresponding to key or empty string.</returns>
        public string ItemOptional (string key)
        {
            return ContainsKey(key) ? Item(key) : string.Empty;
        }

        /// <summary>
        /// Returns a combination of items stored in this instance.
        /// </summary>
        /// <param name="key1">Key 1.</param>
        /// <param name="key2">Key 2.</param>
        /// <returns>Value1 + Value2.</returns>
        public string ItemCombination (string key1, string key2)
        {
            return ItemOptional(key1) + ItemOptional(key2);
        }

        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
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
