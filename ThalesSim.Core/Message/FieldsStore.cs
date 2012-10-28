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

namespace ThalesSim.Core.Message
{
    /// <summary>
    /// This class provides an in-memory store for all message field definitions
    /// that are read. This is done in order to reduce file I/O.
    /// </summary>
    public class FieldsStore
    {
        private static readonly SortedList<string, Fields> Store = new SortedList<string, Fields>();

        /// <summary>
        /// Clears the in-memory fields store.
        /// </summary>
        public static void Clear()
        {
            Store.Clear();
        }

        /// <summary>
        /// Adds a new fields definition to memory.
        /// </summary>
        /// <param name="key">Key to fields.</param>
        /// <param name="fields">Instance of fields.</param>
        public static void Add (string key, Fields fields)
        {
            Store.Add(key, fields);
        }

        /// <summary>
        /// Removes a stored fields definition.
        /// </summary>
        /// <param name="key">Key to fields.</param>
        public static void Remove (string key)
        {
            Store.Remove(key);
        }

        /// <summary>
        /// Checks if fields are already read.
        /// </summary>
        /// <param name="key">Key to fields.</param>
        /// <returns>True if fields are already read.</returns>
        public static bool ContainsKey (string key)
        {
            return Store.ContainsKey(key);
        }

        /// <summary>
        /// Get fields by key.
        /// </summary>
        /// <param name="key">Key to fields.</param>
        /// <returns>Instance of fields.</returns>
        public static Fields Item (string key)
        {
            try
            {
                return Store[key].Clone();
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
