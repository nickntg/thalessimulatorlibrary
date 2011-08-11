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

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core
Imports ThalesSim.Core.PIN.PINBlockFormat

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Translates a PIN from RSA to ZPK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the custom SA thales command. The implementation is not complete
    ''' in the following ways:
    ''' 
    ''' * There is no support for retrieving the RSA key from the message. The private
    '''   key flag should, therefore, not have a value of 99.
    ''' * The private key flag is ignored and a single RSA key is used, regardless
    '''   of the index specified by the caller.
    ''' * The source PIN block is assumed to always have ANSI X9.8 format.
    ''' </remarks>
    <ThalesCommandCode("SA", "SB", "", "Translates a PIN from RSA to ZPK encryption")> _
    Public Class RSAEncryptTo3DES_SA
        Inherits AHostCommand

        Private _acct As String
        Private _targetKey As String
        Private _padModeIdentifier As String
        Private _sourcePBFormat As String
        Private _destinationPBFormat As String
        Private _rsaPBLength As String
        Private _privateKeyFlag As String
        Private _rsaPB() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the SA message parsing fields.
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
                _padModeIdentifier = kvp.Item("Pad Mode Identifier")
                _sourcePBFormat = kvp.Item("Source PIN Block Format")
                _destinationPBFormat = kvp.Item("Destination PIN Block Format")
                _acct = kvp.Item("Account Number")
                _rsaPBLength = kvp.Item("RSA Encrypted PIN Block Length")
                ReDim _rsaPB((kvp.Item("RSA Encrypted PIN Block").Length - 1) \ 2)
                Utility.HexStringToByteArray(kvp.Item("RSA Encrypted PIN Block"), _rsaPB)
                _targetKey = kvp.ItemCombination("PIN Session Key Scheme", "PIN Session Key")
                _privateKeyFlag = kvp.Item("Private Key Flag")
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

            Dim cryptZPK As New HexKey(_targetKey)
            Dim clearZPK As String = Utility.DecryptUnderLMK(cryptZPK.ToString, cryptZPK.Scheme, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim PBFormat As PIN_Block_Format = ToPINBlockFormat(_destinationPBFormat)
            If PBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes.ER_23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPB As String = String.Empty

            Try
                clearPB = RSAFunctions.GetRSADecryptedPINBlock(RSAFunctions.GetStandardRSAKey(), _rsaPB)
            Catch ex As Exception
                'This indicates an error with the data/key combination.
                mr.AddElement(ErrorCodes.ER_20_PIN_BLOCK_DOES_NOT_CONTAIN_VALID_VALUES)
                Return mr
            End Try

            Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PIN_Block_Format.AnsiX98)
            Dim translatedPB As String = PIN.PINBlockFormat.ToPINBlock(clearPIN, _acct, PBFormat)
            Dim cryptPB As String = TripleDES.TripleDESEncrypt(New HexKey(clearZPK), translatedPB)

            Log.Logger.MinorInfo("Clear ZPK: " + clearZPK)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Clear Source PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear Target PIN Block: " + translatedPB)
            Log.Logger.MinorInfo("Crypt PIN Block: " + cryptPB)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptPB)

            Return mr

        End Function

    End Class

End Namespace
