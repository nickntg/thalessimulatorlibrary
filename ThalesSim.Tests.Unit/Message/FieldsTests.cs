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

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using ServiceStack.Text;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace ThalesSim.Tests.Unit.Message
{
    [TestFixture]
    public class FieldsTests
    {
        [Test]
        public void TestAllConstructions()
        {
            ConfigHelpers.SetAuthorizedState(true);
            ConfigHelpers.SetDoubleLengthZmk();

            string str;
            using (var sr = new StreamReader("..\\..\\data\\tests1.json"))
            {
                str = sr.ReadToEnd();
            }
            var lst =
                (SortedList<string, string>)
                JsonSerializer.DeserializeFromString(str, typeof (SortedList<string, string>));

            foreach (var file in lst.Keys)
            {
                var obj = Fields.ReadXmlDefinition(new FileInfo(file).Name);
                Assert.AreEqual(lst[file], SerializeToXml(obj));
            }
        }

        private string SerializeToXml (Fields obj)
        {
            const string fix1 = @"<Fields xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">";
            const string fix2 = @"<Fields xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">";

            var ser = new XmlSerializer(typeof(Fields));
            using (var ms = new MemoryStream())
            {
                ser.Serialize(ms, obj);
                return ms.ToArray().GetString().Replace(fix2, fix1);
            }
        }
    }
}
