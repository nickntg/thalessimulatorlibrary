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

using ThalesSim.Core.Properties;

namespace ThalesSim.Core.Resources
{
    /// <summary>
    /// Helper class to be used with configuration members.
    /// </summary>
    public class ConfigHelpers
    {
        private static bool _authorizedState = Settings.Default.StartInAuthorizedState;

        /// <summary>
        /// Check if Legacy Mode is enabled.
        /// </summary>
        /// <returns></returns>
        public static bool IsInLegacyMode()
        {
            return Settings.Default.LegacyMode;
        }

        /// <summary>
        /// Check if we're in the authorized state.
        /// </summary>
        /// <returns>True if we're in the authorized state.</returns>
        public static bool IsInAuthorizedState()
        {
            return _authorizedState;
        }

        /// <summary>
        /// Set the authorized state mode.
        /// </summary>
        /// <param name="flag">True to go into the authorized state.</param>
        public static void SetAuthorizedState(bool flag)
        {
            _authorizedState = flag;
        }

        /// <summary>
        /// Determines if the simulator currently uses single-length ZMKs.
        /// </summary>
        /// <returns>True if single-length ZMKs are used.</returns>
        public static bool IsSingleLengthZmk()
        {
            return !Settings.Default.DoubleLengthZMKs;
        }

        /// <summary>
        /// Sets single-length ZMKs.
        /// </summary>
        public static void SetSingleLengthZmk()
        {
            Settings.Default.DoubleLengthZMKs = false;
        }

        /// <summary>
        /// Sets double-length ZMKs.
        /// </summary>
        public static void SetDoubleLengthZmk()
        {
            Settings.Default.DoubleLengthZMKs = true;            
        }
    }
}
