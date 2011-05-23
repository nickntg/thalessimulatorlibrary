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

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Verifies a truncated application cryptogram.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the K2 command.
    ''' Currently, this works only for session key derivation method=1 (MasterCard).
    ''' </remarks>
    <ThalesCommandCode("K2", "K3", "", "Verifies a truncated application cryptogram (MasterCard CAP)")> _
    Public Class VerifyTruncatedApplicationCryptogram_K2
        Inherits AHostCommand

        Const KEY_DERIVATION_METHOD_A As String = "0"
        Const KEY_DERIVATION_METHOD_B As String = "1"
        Const SESSION_KEY_DERIVATION_METHOD_MASTERCARD As String = "1"

        Private _modeFlag As String
        Private _schemeID As String
        Private _cardKeyDerivationMethod As String
        Private _sessionKeyDerivationMethod As String
        Private _mkac As String
        Private _panLength As String
        Private _panpanSeqNo As String
        Private _atc As String
        Private _un As String
        Private _transactionDataLength As String
        Private _transactionData As String
        Private _truncatedAC As String
        Private _cryptogramIPB As String
        Private _IPBMAC As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the K2 message parsing fields.
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
                _modeFlag = kvp.Item("Mode Flag")
                _schemeID = kvp.Item("Scheme ID")
                _cardKeyDerivationMethod = kvp.Item("Card Key Derivation Method")
                _sessionKeyDerivationMethod = kvp.Item("Session Key Derivation Method")
                _mkac = kvp.ItemCombination("MK-AC Scheme", "MK-AC")
                _panLength = kvp.Item("PAN Length")
                _panpanSeqNo = kvp.Item("PAN And PAN Sequence No")
                _atc = kvp.Item("ATC")
                _un = kvp.Item("UN")
                _transactionDataLength = kvp.Item("Transaction Data Length")
                _transactionData = kvp.Item("Transaction Data")
                _truncatedAC = kvp.Item("Truncated AC")
                _cryptogramIPB = kvp.Item("Cryptogram IPB")
                _IPBMAC = kvp.Item("IPB MAC")
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

            Dim cryptKey As New HexKey(_mkac)
            Dim clearKey As New HexKey(Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair28_29, "1"))
            If Utility.IsParityOK(clearKey.ToString, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim PANAndPANSeq As String = _panpanSeqNo 'Utility.fromBCD(_panpanSeqNo)
            Dim DerivedKey As HexKey = Nothing
            Select Case _cardKeyDerivationMethod
                Case KEY_DERIVATION_METHOD_A
                    DerivedKey = EMV.KeyDerivation.GetDerivedMasterKey(clearKey, PANAndPANSeq, EMV.MasterKeyDerivationMethods.EMV_4_2_OptionA)
                Case KEY_DERIVATION_METHOD_B
                    DerivedKey = EMV.KeyDerivation.GetDerivedMasterKey(clearKey, PANAndPANSeq, EMV.MasterKeyDerivationMethods.EMV_4_2_OptionB)
            End Select

            Dim ATC As String = _atc 'GetDecodedBytes(Utility.GetBytesFromString(_atc))
            Dim UN As String = _un 'GetDecodedBytes(Utility.GetBytesFromString(_un))
            Dim SessionKey As HexKey = Nothing
            Select Case _sessionKeyDerivationMethod
                Case SESSION_KEY_DERIVATION_METHOD_MASTERCARD
                    SessionKey = EMV.KeyDerivation.GetSessionKey(DerivedKey, ATC, UN, EMV.SessionKeyDerivationMethods.MasterCard_Method)
            End Select

            Dim TranData As String = _transactionData  'GetDecodedBytes(Utility.GetBytesFromString(_transactionData))
            Dim DerivedAC As String = GetMac(TranData, SessionKey)

            Dim passedAC As String = _truncatedAC '""
            'Utility.ByteArrayToHexString(Utility.GetBytesFromString(_truncatedAC), passedAC)

            Log.Logger.MinorInfo("Crypt IMK: " + cryptKey.ToString)
            Log.Logger.MinorInfo("Clear IMK: " + clearKey.ToString)
            Log.Logger.MinorInfo("Derived Key: " + DerivedKey.ToString)
            Log.Logger.MinorInfo("Session Key: " + SessionKey.ToString)
            Log.Logger.MinorInfo("ATC: " + ATC)
            Log.Logger.MinorInfo("UN: " + UN)
            Log.Logger.MinorInfo("Transaction Data: " + TranData)
            Log.Logger.MinorInfo("Derived AC: " + DerivedAC)
            Log.Logger.MinorInfo("Passed AC: " + passedAC)

            If passedAC = DerivedAC Then
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes.ER_01_VERIFICATION_FAILURE)
                If IsInAuthorizedState() Then
                    mr.AddElement(DerivedAC)
                    'mr.AddElement(GetEncodedBytes(DerivedAC))
                End If
            End If

            Return mr
        End Function

        ''' <summary>
        ''' Calculates a MAC on the track data using the derived key.
        ''' </summary>
        ''' <param name="data">Track data used for calculation.</param>
        ''' <param name="KD">Derived key.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetMac(ByVal data As String, ByVal KD As Cryptography.HexKey) As String
            'If it's padded to 8-byte blocks, add another required block.
            If data.Length Mod 16 = 0 Then
                data = data + "8000000000000000"
            Else
                'Else, add 0x80 and pad to an 8-byte block with zeroes.
                data = data + "80"
                If data.Length Mod 16 <> 0 Then
                    data = data.PadRight(data.Length + (16 - data.Length Mod 16), "0"c)
                End If
            End If

            'With an initial 8-byte block of zeroes and starting from the left of the track data:
            '  1. XOR the initial block with the next 8-byte block of the track data.
            '  2. Encrypt the result with the left side of the key.
            '  3. Use the result of the above and go to 1 until all track data are processed.
            '  4. Decrypt the result of the above with the right side of the key.
            '  5. Encrypt the result of the above with the left side of the key.
            '  6. The 2 least significant bytes of the result are the MAC.
            Dim KL As New Cryptography.HexKey(KD.ToString.Substring(0, 16))
            Dim startIndex As Integer = 0
            Dim intermediateryData As String = "0000000000000000"
            While startIndex <= data.Length - 1
                Dim algoData As String = data.Substring(startIndex, 16)
                startIndex += 16
                intermediateryData = Utility.XORHexStringsFull(intermediateryData, algoData)
                intermediateryData = Cryptography.TripleDES.TripleDESEncrypt(KL, intermediateryData)
            End While
            Dim KR As New Cryptography.HexKey(KD.ToString.Substring(16))
            intermediateryData = Cryptography.TripleDES.TripleDESDecrypt(KR, intermediateryData)
            intermediateryData = Cryptography.TripleDES.TripleDESEncrypt(KL, intermediateryData)
            Return intermediateryData.Substring(0, 16)
        End Function

        ''' <summary>
        ''' Bytes-to-ASCII.
        ''' </summary>
        ''' <param name="b"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetDecodedBytes(ByVal b() As Byte) As String
            Dim s As String = ""
            For i As Integer = 0 To b.GetUpperBound(0)
                s = s + Convert.ToString(b(i)).PadLeft(2, "0"c)
            Next
            Return s
        End Function

        ''' <summary>
        ''' Hex-To-Bytes.
        ''' </summary>
        ''' <param name="trackData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetEncodedBytes(ByVal trackData As String) As String
            Dim b((trackData.Length \ 2) - 1) As Byte
            Utility.HexStringToByteArray(trackData, b)
            Return Utility.GetStringFromBytes(b)
        End Function

    End Class

End Namespace
