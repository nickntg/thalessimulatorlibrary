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

using NUnit.Framework;
using ThalesSim.Core.Commands;
using ThalesSim.Core.Commands.Host;
using ThalesSim.Core.Commands.Host.Implementations;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Tests.Unit.Commands
{
    [TestFixture]
    public class HostCommandTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            CommandExplorer.Discover();
            LmkStorage.LmkStorageFile = "nofile.txt";
            LmkStorage.GenerateTestLmks(false);
        }

        [Test]
        public void SetHsmDelayTest()
        {
            Assert.AreEqual("00", TestMessage("000", new SetHsmDelay_LG()));
        }

        private string TestMessage (string message, AHostCommand command)
        {
            var msg = new StreamMessage(message);
            command.AcceptMessage(msg);
            if (command.XmlParseResult != ErrorCodes.ER_00_NO_ERROR)
            {
                return command.XmlParseResult;
            }

            var rsp = command.ConstructResponse();
            return rsp.GetBytes().GetString();
        }
    }
}
