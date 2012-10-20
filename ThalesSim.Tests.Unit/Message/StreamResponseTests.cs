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
using ThalesSim.Core.Message;

namespace ThalesSim.Tests.Unit.Message
{
    [TestFixture]
    public class StreamResponseTests
    {
        [Test]
        public void TestStreamResponse()
        {
            var str = new StreamResponse();

            Assert.IsNullOrEmpty(str.Message);

            str.Add("1");

            Assert.AreEqual("1", str.Message);

            str.Add("2");

            Assert.AreEqual("12", str.Message);

            str.AddStart("0");

            Assert.AreEqual("012", str.Message);
        }
    }
}
