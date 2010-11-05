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
Imports ThalesSim.Core.PIN.PINBlockFormat
Imports ThalesSim.Core.Cryptography.DUKPT

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Translates a PIN block from ZPK to ZPK encryption (3DES).
    ''' </summary>
    ''' <remarks>
    ''' This class implements the G0 Racal command.
    ''' </remarks>
    <ThalesCommandCode("G0", "G1", "", "Translates a PIN block from DUKPT to ZPK encryption (3DES)")> _
    Public Class TranslatePINFromDUKPTToZPK3DES_G0
        Inherits AHostCommand

        Private _acct As String
        Private _sourceKey As String
        Private _targetKey As String
        Private _ksnDescriptor As String
        Private _ksn As String
        Private _pb As String
        Private _targetPBFormat As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CI message parsing fields.
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
                _sourceKey = kvp.ItemCombination("BDK Scheme", "BDK")
                _targetKey = kvp.ItemCombination("ZPK Scheme", "ZPK")
                _ksnDescriptor = kvp.Item("KSN Descriptor")
                _ksn = kvp.Item("Key Serial Number")
                _pb = kvp.Item("Encrypted Block")
                _acct = kvp.Item("Account Number")
                _targetPBFormat = kvp.Item("Destination PIN Block Format Code")
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

            Dim cryptBDK As New HexKey(_sourceKey)
            Dim clearBDK As String = Utility.DecryptUnderLMK(cryptBDK.ToString, cryptBDK.Scheme, LMKPairs.LMKPair.Pair28_29, "0")
            If Utility.IsParityOK(clearBDK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim cryptZPK As New HexKey(_targetKey)
            Dim clearZPK As String = Utility.DecryptUnderLMK(cryptZPK.ToString, cryptZPK.Scheme, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim targetPBFormat As PIN_Block_Format = ToPINBlockFormat(_targetPBFormat)
            If targetPBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes.ER_23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If
            Dim ksn As New KeySerialNumber(_ksn, _ksnDescriptor)

            Dim dukptKey As String = DerivedKey.calculateDerivedKey(ksn, clearBDK)

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(dukptKey), _pb)
            Dim clearPIN As String = ToPIN(clearPB, _acct, PIN_Block_Format.AnsiX98)

            Dim newPB As String = ToPINBlock(clearPIN, _acct, targetPBFormat)
            Dim cryptPB As String = TripleDES.TripleDESEncrypt(New HexKey(clearZPK), newPB)

            Log.Logger.MinorInfo("Clear source BDK: " + clearBDK)
            Log.Logger.MinorInfo("Clear target ZPK: " + clearZPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("New clear PIN Block: " + newPB)
            Log.Logger.MinorInfo("New crypt PIN Block: " + cryptPB)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(clearPIN.Length.ToString().PadLeft(2, "0"c))
            mr.AddElement(cryptPB)
            mr.AddElement(_targetPBFormat)

            Return mr

        End Function

    End Class

End Namespace
