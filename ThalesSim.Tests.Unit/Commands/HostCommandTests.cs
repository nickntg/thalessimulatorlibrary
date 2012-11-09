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
            ConfigHelpers.SetAuthorizedState(true);
        }

        [Test]
        public void SetHsmDelayTest()
        {
            Assert.AreEqual("00", TestMessage("000", new SetHsmDelay_LG()));
        }

        [Test]
        public void CancelAuthStateTest()
        {
            ConfigHelpers.SetAuthorizedState(true);
            Assert.AreEqual("00", TestMessage("", new CancelAuthState_RA()));
            Assert.IsFalse(ConfigHelpers.IsInAuthorizedState());
            ConfigHelpers.SetSingleLengthZmk();
        }

        [Test]
        public void EchoTest()
        {
            Assert.AreEqual("80", TestMessage("000A0123456", new EchoTest_B2()));
            Assert.AreEqual("15", TestMessage("000A0123456789ABCDEF", new EchoTest_B2()));
            Assert.AreEqual("000123456789", TestMessage("000A0123456789", new EchoTest_B2()));
        }

        [Test]
        public void ImportKeyTest()
        {
            ConfigHelpers.SetSingleLengthZmk();
            Assert.AreEqual("000406FBB23A5214DF0035BB", TestMessage("0024ED06495741C280C35ED0C0EA7F7D0FAZ", new ImportKey_A6()));
            Assert.AreEqual("000406FBB23A5214DF0035BB", TestMessage("0024ED06495741C280CB9219C90F03A9627Z5", new ImportKey_A6()));
            ConfigHelpers.SetDoubleLengthZmk();
            // Contributed by wpak, fixes issue described at http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=217215.
            Assert.AreEqual("00U0E07CDC0161A0DE3B5AA44DF227EC9DEABDEBC", TestMessage("001U71979DEB8587E2734F1E99D5DCAEF9ACU482C4E722BB0CF1845E1E5BD16310119U", new ImportKey_A6()));
            Assert.AreEqual("00U1EF828AA8F6B80EB83E19FBC373F3A856F1E3F", TestMessage("001U71979DEB8587E2734F1E99D5DCAEF9ACXC8E3118AFA853807EB7E92294663A5BAU", new ImportKey_A6()));
            Assert.AreEqual("00U1EF828AA8F6B80EB83E19FBC373F3A856F1E3F", TestMessage("001U71979DEB8587E2734F1E99D5DCAEF9ACX8E80C547F2A1324B84763B0EE32B73ADU1", new ImportKey_A6()));
            Assert.AreEqual("00BAB32D775A38E4AB73936E", TestMessage("001U1457FF6ADF6250C66C368416B4C9D3832B930A07119F93A8Z", new ImportKey_A6()));
        }

        [Test]
        public void ExportKeyTest()
        {
            ConfigHelpers.SetAuthorizedState(true);
            Assert.AreEqual("0035ED0C0EA7F7D0FA0035BB", TestMessage("0024ED06495741C280C0406FBB23A5214DFZ", new ExportKey_A8()));
            ConfigHelpers.SetDoubleLengthZmk();
            Assert.AreEqual("0016224FDAA779AFB31FFD3C", TestMessage("002U1457FF6ADF6250C66C368416B4C9D3837BB126F2BE631486Z", new ExportKey_A8()));
            Assert.AreEqual("00U2C62A23D001B97412950CD8BE66C7639070753", TestMessage("002U1457FF6ADF6250C66C368416B4C9D383U8463435FC4B4DAA0C49025272C29B12CU", new ExportKey_A8()));
        }

        [Test]
        public void FormKeyFromEncryptedComponentsTest()
        {
            ConfigHelpers.SetAuthorizedState(true);
            Assert.AreEqual("00FE018240022A76DCA192FE",
                            TestMessage("3002Z3B723AF4CF00A7A6954111D254A90D17EAAF49979FA95742",
                                        new FormKeyFromEncryptedComponents_A4()));
            Assert.AreEqual("00XC0BC1DFFC449A402DAB71250CA5869CC8CE396",
                            TestMessage("3000XX2EC8A0412B5D0E86E3C1E5ABFA19B3F5XFF43378ED5D85B1BC465BF000335FBF1XA235EDF4C58A2CB0C84641D07319CF21",
                                        new FormKeyFromEncryptedComponents_A4()));
            ConfigHelpers.SetAuthorizedState(false);
            Assert.IsTrue(CommandExplorer.GetCommand(CommandType.Host, "A4").RequiresAuthorizedState);
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
