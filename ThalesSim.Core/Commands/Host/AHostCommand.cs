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
using log4net;

namespace ThalesSim.Core.Commands.Host
{
    public class AHostCommand
    {
        public string PrinterData { get; set; }

        public Fields MessageFields { get; set; }

        public string XmlParseResult { get; set; }

        public MessageKeyValuePairs KeyValues { get; set; }

        public virtual void AcceptMessage (StreamMessage message)
        {
            XmlParseResult = Parser.Parse(message, MessageFields, KeyValues);
        }

        protected ILog Log;

        public AHostCommand()
        {
            Log = LogManager.GetLogger(GetType());
            KeyValues = new MessageKeyValuePairs();
        }

        public virtual StreamResponse ConstructResponse()
        {
            return null;
        }

        public virtual StreamResponse ConstructResponseAfterIo()
        {
            return null;
        }

        public string DumpFields()
        {
            return KeyValues.ToString();
        }

        protected void ReadXmlDefinitions()
        {
            ReadXmlDefinitions(false, GetType().Name + ".xml");
        }

        protected void ReadXmlDefinitions (bool forceRead)
        {
            ReadXmlDefinitions(forceRead, GetType().Name + ".xml");
        }

        protected void ReadXmlDefinitions (string fileName)
        {
            ReadXmlDefinitions(false, GetType().Name + ".xml");
        }

        protected void ReadXmlDefinitions (bool forecRead, string fileName)
        {
            if (forecRead)
            {
                FieldsStore.Remove(GetType().Name);
            }

            MessageFields = FieldsStore.Item(GetType().Name);
            if (MessageFields == null)
            {
                MessageFields = Fields.ReadXmlDefinition(fileName);
                FieldsStore.Add(GetType().Name, MessageFields);
            }
        }
    }
}
