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

using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales RA implementation.
    /// </summary>
    [ThalesHostCommand("RA", "RB", "Cancel the authorized state")]
    public class CancelAuthState_RA : AHostCommand
    {
        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public CancelAuthState_RA()
        {
            ReadXmlDefinitions();
        }

        /// <summary>
        /// Accept message from client.
        /// </summary>
        /// <returns>Message response.</returns>
        public override Message.StreamResponse ConstructResponse()
        {
            Log.Info(ConfigHelpers.IsInAuthorizedState()
                         ? "Exiting the authorized state"
                         : "Already out of the authorized state");

            ConfigHelpers.SetAuthorizedState(false);

            var mr = new StreamResponse();
            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            return mr;
        }
    }
}
