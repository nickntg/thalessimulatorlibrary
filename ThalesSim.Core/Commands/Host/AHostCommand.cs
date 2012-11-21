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
using ThalesSim.Core.Cryptography;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;
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

        /// <summary>
        /// Parses and validates a key type code.
        /// </summary>
        /// <param name="keyTypeCode">Key type code to parse.</param>
        /// <param name="mr">Message response to add error code to, if parsing fails.</param>
        /// <param name="code">Parsed key type code.</param>
        /// <returns>True if parsing is successful.</returns>
        protected bool ValidateKeyTypeCode(string keyTypeCode, StreamResponse mr, ref KeyTypeCode code)
        {
            try
            {
                code = new KeyTypeCode(keyTypeCode);
                return true;
            }
            catch (InvalidCastException)
            {
                mr.Append(ErrorCodes.ER_04_INVALID_KEY_TYPE_CODE);
                return false;
            }
        }

        /// <summary>
        /// Parses and validates a key scheme code.
        /// </summary>
        /// <param name="keySchemeCode">Key scheme code.</param>
        /// <param name="mr">Message response to add error code to, if parsing fails.</param>
        /// <param name="scheme">Parsed key scheme.</param>
        /// <returns>True if parsing is successful.</returns>
        protected bool ValidateKeySchemeCode (string keySchemeCode, StreamResponse mr, ref KeyScheme scheme)
        {
            try
            {
                scheme = keySchemeCode.GetKeyScheme();
                return true;
            }
            catch (Exception)
            {
                mr.Append(ErrorCodes.ER_26_INVALID_KEY_SCHEME);
                return false;
            }
        }

        /// <summary>
        /// Validates an authorized state requirement.
        /// </summary>
        /// <param name="func">Function to perform.</param>
        /// <param name="pair">LMK pair.</param>
        /// <param name="variant">Variant.</param>
        /// <param name="mr">Message response to add error code to, if parsing fails.</param>
        /// <returns>True if the operation is allowed.</returns>
        protected bool ValidateAuthStateRequirement (KeyFunction func, LmkPair pair, int variant, StreamResponse mr)
        {
            var req = AuthStateRequirements.GetRequirement(func, pair, variant);
            if (req == StateRequirementType.NotAllowed)
            {
                mr.Append(ErrorCodes.ER_29_FUNCTION_NOT_PERMITTED);
                return false;
            }

            if (req == StateRequirementType.NeedsAuthorizedState && !ConfigHelpers.IsInAuthorizedState())
            {
                mr.Append(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appends to the printer data.
        /// </summary>
        /// <param name="data">String to append.</param>
        protected void AddPrinterData (string data)
        {
            PrinterData += data + "\r\n";
        }
    }
}
