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
using ThalesSim.Core.Commands;
using ThalesSim.Core.Commands.Host;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;
using log4net;

namespace ThalesSim.Core.Processor
{
    /// <summary>
    /// The host command processor coordinates execution of Thales
    /// host commands. It accepts a byte array with the client request,
    /// finds the appropriate host command to execute and returns the
    /// results.
    /// </summary>
    public class HostCommandProcessor
    {
        private readonly ILog _log = LogManager.GetLogger(typeof (HostCommandProcessor));

        /// <summary>
        /// Process a message.
        /// </summary>
        /// <param name="message">Byte array with request message.</param>
        /// <param name="resp">Host command response.</param>
        /// <param name="respAfterIo">Host command response after doing printer I/O.</param>
        public void ProcessMessage (byte[] message, out StreamResponse resp, out StreamResponse respAfterIo)
        {
            DumpMessage(message, "Incoming message");

            var msg = new StreamMessage(message);

            resp = null;
            respAfterIo = null;

            try
            {
                if (msg.CharsLeft < Properties.Settings.Default.HeaderLength + 2)
                {
                    _log.ErrorFormat("Too small message, must be at least {0} bytes long",
                                     Properties.Settings.Default.HeaderLength + 2);
                    return;
                }

                var msgHeader = msg.Substring(Properties.Settings.Default.HeaderLength);
                var code = msg.Substring(2);
                var msgTrailer = Properties.Settings.Default.ExpectTrailers ? msg.GetTrailer() : string.Empty;

                _log.DebugFormat("Header {0}, command code {1}", msgHeader, code);

                var command = (HostCommand)CommandExplorer.GetCommand(CommandType.Host, code);
                if (command == null)
                {
                    _log.ErrorFormat("No implementor for {0}.", code);
                    return;
                }

                if (Properties.Settings.Default.CheckLMKParity && !LmkStorage.CheckLmkStorage())
                {
                    _log.Error("LMK storage check failed");
                    resp = new StreamResponse();
                    resp.Append(ErrorCodes.ER_13_MASTER_KEY_PARITY_ERROR);
                    resp.AppendFront(command.ResponseCode);
                    resp.AppendFront(msgHeader);
                    resp.Append(msgTrailer);
                    return;
                }

                _log.Debug("Instantiating command...");
                var o = (AHostCommand) Activator.CreateInstance(command.DeclaringType);

                _log.Debug("Calling AcceptMessage()...");
                o.AcceptMessage(msg);

                _log.Debug(o.DumpFields());

                if (o.XmlParseResult != ErrorCodes.ER_00_NO_ERROR)
                {
                    _log.DebugFormat("Error code {0} return while parsing message", o.XmlParseResult);
                    resp = new StreamResponse();
                    resp.Append(o.XmlParseResult);
                }
                else
                {
                    _log.Debug("Calling ConstructResponse()...");
                    resp = o.ConstructResponse();

                    _log.Debug("Calling ConstructResponseAfterIo()...");
                    respAfterIo = o.ConstructResponseAfterIo();
                }

                resp.AppendFront(command.ResponseCode);
                resp.AppendFront(msgHeader);
                resp.Append(msgTrailer);

                DumpMessage(resp.GetBytes(), "Response message");

                if (respAfterIo != null && !string.IsNullOrEmpty(command.ResponseCodeAfterIo))
                {
                    respAfterIo.AppendFront(command.ResponseCodeAfterIo);
                    respAfterIo.AppendFront(msgHeader);
                    respAfterIo.Append(msgTrailer);
                    DumpMessage(respAfterIo.GetBytes(), "Response message after I/O");
                }

                if (!string.IsNullOrEmpty(o.PrinterData))
                {
                    _log.InfoFormat("Printer data\r\n{0}", o.PrinterData);
                }
            }
            catch (Exception ex)
            {
                _log.Debug("Exception while processing message", ex);
                resp = null;
                respAfterIo = null;
            }
        }

        /// <summary>
        /// Dump a message to the logger.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="text"></param>
        private void DumpMessage (byte[] bytes, string text)
        {
            _log.DebugFormat(text + "\r\n" + bytes.GetDump());
        }
    }
}
