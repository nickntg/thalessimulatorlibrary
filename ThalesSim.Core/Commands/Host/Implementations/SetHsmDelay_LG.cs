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
    [ThalesHostCommand("LG", "LH", "Sets an HSM response delay")]
    public class SetHsmDelay_LG : AHostCommand
    {
        private string _delay;

        public SetHsmDelay_LG()
        {
            ReadXmlDefinitions();
        }

        public override void AcceptMessage(StreamMessage message)
        {
            base.AcceptMessage(message);

            if (XmlParseResult == ErrorCodes.ER_00_NO_ERROR)
            {
                _delay = KeyValues.Item("Delay");
            }
        }

        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            Log.InfoFormat("HSM delay {0} set and ignored", _delay);

            mr.Add(ErrorCodes.ER_00_NO_ERROR);
            return mr;
        }
    }
}
