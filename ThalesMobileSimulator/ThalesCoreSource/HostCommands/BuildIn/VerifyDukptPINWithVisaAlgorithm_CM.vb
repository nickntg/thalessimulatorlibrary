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
    ''' Verifies a DUKPT PIN using the VISA algorithm.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CM Racal command.
    ''' </remarks>
    <ThalesCommandCode("CM", "CN", "", "Verifies a DUKPT PIN using the VISA algorithm")> _
    Public Class VerifyDukptPINWithVISAAlgorithm_CM
        Inherits AHostCommand

        Private _bdk As String
        Private _pvkPair As String
        Private _ksnDescriptor As String
        Private _ksn As String
        Private _pinBlock As String
        Private _acct As String
        Private _pvki As String
        Private _pvv As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CM message parsing fields.
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
                _bdk = kvp.ItemCombination("BDK Scheme", "BDK")
                _pvkPair = kvp.ItemCombination("PVK Scheme", "PVK")
                _ksnDescriptor = kvp.Item("KSN Descriptor")
                _ksn = kvp.Item("Key Serial Number")
                _pinBlock = kvp.Item("Encrypted Block")
                _acct = kvp.Item("Account Number")
                _pvki = kvp.Item("PVKI")
                _pvv = kvp.Item("PVV")
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

            Dim cryptBDK As New HexKey(_bdk)
            Dim clearBDK As String = Utility.DecryptUnderLMK(cryptBDK.ToString, cryptBDK.Scheme, LMKPairs.LMKPair.Pair28_29, "0")
            If Utility.IsParityOK(clearBDK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim cryptPVK As New HexKey(_pvkPair)
            Dim clearPVK As String = Utility.DecryptUnderLMK(cryptPVK.ToString, cryptPVK.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim ksn As New KeySerialNumber(_ksn, _ksnDescriptor)
            Dim dukptKey As String = DerivedKey.calculateDerivedKey(ksn, clearBDK)
            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(dukptKey), _pinBlock)
            Dim clearPIN As String = ToPIN(clearPB, _acct, PIN_Block_Format.AnsiX98)

            '            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearBDK), _pinBlock)
            '           Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PIN_Block_Format.AnsiX98)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim PVV As String = Me.GeneratePVV(_acct, _pvki, clearPIN, clearPVK)

            Log.Logger.MinorInfo("Clear PVKs: " + clearPVK)
            Log.Logger.MinorInfo("Clear BDK: " + clearBDK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            If _pvv = PVV Then
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes.ER_01_VERIFICATION_FAILURE)
            End If

            Return mr

        End Function

    End Class

End Namespace
