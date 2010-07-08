''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 
' Contributed by rjw - May 2010

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generates a random BDK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the BI Racal command.
    ''' </remarks>
    <ThalesCommandCode("BI", "BJ", "", "Generates a random BDK")> _
    Public Class GenerateBDK_BI
        Inherits AHostCommand

        Private _del As String
        Private _keySchemeLMK As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the BI message parsing fields.
        ''' </remarks>
        Public Sub New()
            ReadXMLDefinitions()
        End Sub
        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeLMK = kvp.ItemOptional("Key Scheme LMK")
            End If
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse
            Dim mr As New MessageResponse
            Dim lmkKs As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, lmkKs, mr) = False Then Return mr
            Else
                lmkKs = KeySchemeTable.KeyScheme.SingleDESKey
            End If

            Dim clearKey As String = Utility.CreateRandomKey(lmkKs)
            Dim cryptKeyLMK As String = Utility.EncryptUnderLMK(clearKey, lmkKs, LMKPairs.LMKPair.Pair28_29, "0")

            Log.Logger.MinorInfo("New BDK (clear): " + clearKey)
            Log.Logger.MinorInfo("New BDK (LMK): " + cryptKeyLMK)
            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptKeyLMK)
            Return mr
        End Function

    End Class

End Namespace
