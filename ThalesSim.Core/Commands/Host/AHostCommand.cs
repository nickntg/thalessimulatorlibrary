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
    /// <summary>
    /// Blueprint for all host command implementations.
    /// </summary>
    public class AHostCommand
    {
        /// <summary>
        /// Get/set the data sent to the printer by this command.
        /// </summary>
        public string PrinterData { get; set; }

        /// <summary>
        /// Get/set the message field definitions of this command.
        /// </summary>
        public Fields MessageFields { get; set; }

        /// <summary>
        /// Get/set the result of parsing the XML definitions
        /// of this command.
        /// </summary>
        public string XmlParseResult { get; set; }

        /// <summary>
        /// Get/set the key/value pairs parsed from the message.
        /// </summary>
        public MessageKeyValuePairs KeyValues { get; set; }

        /// <summary>
        /// Logger.
        /// </summary>
        protected ILog Log;

        /// <summary>
        /// Default class constructor that initializes this instance.
        /// </summary>
        public AHostCommand()
        {
            Log = LogManager.GetLogger(GetType());
            KeyValues = new MessageKeyValuePairs();
        }

        /// <summary>
        /// Called to parse the message received by the client.
        /// </summary>
        /// <param name="message">Request message.</param>
        public virtual void AcceptMessage (StreamMessage message)
        {
            XmlParseResult = Parser.Parse(message, MessageFields, KeyValues);
        }

        /// <summary>
        /// Called to process the received request message.
        /// </summary>
        /// <returns>Response.</returns>
        public virtual StreamResponse ConstructResponse()
        {
            return null;
        }

        /// <summary>
        /// Called to process the received request message 
        /// after printer I/O is done.
        /// </summary>
        /// <returns>Response after printer I/O.</returns>
        public virtual StreamResponse ConstructResponseAfterIo()
        {
            return null;
        }

        /// <summary>
        /// Returns a string representation of the
        /// parsed key/value pairs.
        /// </summary>
        /// <returns>String representation of the 
        /// parsed message.</returns>
        public string DumpFields()
        {
            return KeyValues.ToString();
        }

        /// <summary>
        /// Read the XML definitions.
        /// </summary>
        protected void ReadXmlDefinitions()
        {
            ReadXmlDefinitions(false, GetType().Name + ".xml");
        }

        /// <summary>
        /// Read the XML definitions.
        /// </summary>
        /// <param name="forceRead">Flag to force read again from disk.</param>
        protected void ReadXmlDefinitions (bool forceRead)
        {
            ReadXmlDefinitions(forceRead, GetType().Name + ".xml");
        }

        /// <summary>
        /// Read the XML definitions using a specific
        /// XML file definition.
        /// </summary>
        /// <param name="fileName">File with XML definitions.</param>
        protected void ReadXmlDefinitions (string fileName)
        {
            ReadXmlDefinitions(false, GetType().Name + ".xml");
        }

        /// <summary>
        /// Read the XML definitions.
        /// </summary>
        /// <param name="forecRead">Flag to force read again from disk.</param>
        /// <param name="fileName">File with XML definitions.</param>
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
