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
    ''' Verifies a dynamic CVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the PM command.
    ''' Currently, this works only for Scheme ID=1 and Version=2 (MastercardPayPass-PAN provided in input and 
    ''' IVCVC3 calculated from provided magnetic stripe data).
    ''' </remarks>
    <ThalesCommandCode("PM", "PN", "", "Verifies a dynamic CVV")> _
    Public Class VerifyDynamicCVV_PM
        Inherits AHostCommand

        Const SCHEME_ID As String = "SCHEME_ID"
        Const VERSION_ID As String = "VERSION_ID"
        Const MKDCVV As String = "MKDCVV"
        Const KEY_DERIVATION_METHOD As String = "KEY_DER_METHOD"
        Const PAN As String = "PAN"

        Const SCHEME_VISA As String = "0"
        Const SCHEME_MASTERCARD As String = "1"
        Const VERSION_VISA As String = "0"
        Const VERSION_MASTERCARD_PAYPASS_IVCVC3PROVIDED As String = "0"
        Const VERSION_MASTERCARD_PAYPASS_PSNANDIVCVC3PROVIDED As String = "1"
        Const VERSION_MASTERCARD_PAYPASS_PANANDTRACKPROVIDED As String = "2"

        Private _schemeID As String
        Private _versionID As String
        Private _cryptIMK As String
        Private _keyDerivationMethod As String
        Private _PANstr As String
        Private _PANSequenceNo As String
        Private _TrackLength As Integer
        Private _TrackData As String
        Private _TrackClearData As String
        Private _UN As String
        Private _ATC As String
        Private _ATCHex As String
        Private _dCVV As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the PM message parsing fields.
        ''' </remarks>
        Public Sub New()
            'We're only adding the common fields for now.
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(SCHEME_ID, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(VERSION_ID, 1))
            MFPC.AddMessageFieldParser(GeneratePVKKeyParser(MKDCVV))
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_DERIVATION_METHOD, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PAN, DELIMITER_VALUE))
            MFPC.AddMessageFieldParser(New MessageFieldParser(DELIMITER, 1))
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            MFPC.ParseMessage(msg)
            _schemeID = MFPC.GetMessageFieldByName(SCHEME_ID).FieldValue
            _versionID = MFPC.GetMessageFieldByName(VERSION_ID).FieldValue
            _cryptIMK = MFPC.GetMessageFieldByName(MKDCVV).FieldValue
            _keyDerivationMethod = MFPC.GetMessageFieldByName(KEY_DERIVATION_METHOD).FieldValue
            _PANstr = MFPC.GetMessageFieldByName(PAN).FieldValue

            'Parsing the currently implemented case manually.
            If _schemeID = SCHEME_MASTERCARD AndAlso _versionID = VERSION_MASTERCARD_PAYPASS_PANANDTRACKPROVIDED Then
                Try
                    _PANSequenceNo = msg.GetSubstring(2)
                    msg.AdvanceIndex(2)
                    _TrackLength = Convert.ToInt32(msg.GetSubstring(3))
                    msg.AdvanceIndex(3)
                    _TrackData = msg.GetSubstring(_TrackLength)
                    msg.AdvanceIndex(_TrackLength)
                    _UN = msg.GetSubstring(8)
                    msg.AdvanceIndex(8)
                    _ATC = msg.GetSubstring(5)
                    msg.AdvanceIndex(5)

                    'An X in the dynamic CVV means ignore. We'll replace it out.
                    _dCVV = msg.GetSubstring(5).Replace("X", "")

                    'We want the ATC in hexadecimal.
                    Dim ATC As Integer = Convert.ToInt32(_ATC)
                    Utility.ByteArrayToHexString(New Byte() {Convert.ToByte(ATC \ 256), Convert.ToByte(ATC Mod 256)}, _ATCHex)

                    'Track data is in binary. We want a hex string with this data.
                    _TrackClearData = ""
                    For i As Integer = 0 To _TrackData.Length - 1
                        _TrackClearData = _TrackClearData + Text.ASCIIEncoding.GetEncoding(Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage).GetBytes(_TrackData.Substring(i, 1))(0).ToString("X2")
                    Next
                Catch ex As Exception
                    'Currently assuming that an error means that a short message was send.
                    Throw New Exceptions.XShortMessage("Short message")
                End Try
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

            'Currently implemented only for Mastercard Paypass.
            If _schemeID <> SCHEME_MASTERCARD Then
                mr.AddElement(ErrorCodes._41_INTERNAL_HARDWARE_SOFTWARE_ERROR)
                Return mr
            End If

            'Currently implemented only for Mastercard Paypass with PAN and track data provided.
            If _versionID <> VERSION_MASTERCARD_PAYPASS_PANANDTRACKPROVIDED Then
                mr.AddElement(ErrorCodes._41_INTERNAL_HARDWARE_SOFTWARE_ERROR)
                Return mr
            End If

            Dim cryptIMK As New HexKey(_cryptIMK)
            Dim clearIMK As New HexKey(Utility.DecryptUnderLMK(_cryptIMK, MKDCVV, MFPC.GetMessageFieldByName(MKDCVV).DeterminerName, LMKPairs.LMKPair.Pair28_29, "7"))

            Dim calcDynamicCVV As String = CalculateDynamicCVV_MastercardPaypass(clearIMK, _PANstr, _PANSequenceNo, _TrackClearData, _UN, _ATCHex)
            If calcDynamicCVV.Length > _dCVV.Length Then
                calcDynamicCVV = calcDynamicCVV.Substring(calcDynamicCVV.Length - _dCVV.Length)
            End If
            If calcDynamicCVV = _dCVV Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
                If IsInAuthorizedState() Then
                    mr.AddElement(calcDynamicCVV)
                End If
            End If

            Return mr
        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            Return Nothing
        End Function

        ''' <summary>
        ''' Calculates a MasterCard-Paypass dynamic CVV using the track data and the PAN.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PAN">PAN.</param>
        ''' <param name="PANSequenceNo">PAN sequence number.</param>
        ''' <param name="trackData">Track data.</param>
        ''' <param name="UN">Unpredictable number.</param>
        ''' <param name="ATC">ATC (hex).</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalculateDynamicCVV_MastercardPaypass(ByVal IMK As Cryptography.HexKey, ByVal PAN As String, ByVal PANSequenceNo As String, _
                                                               ByVal trackData As String, ByVal UN As String, ByVal ATC As String) As String
            'Find the derived key.
            Dim KD As Cryptography.HexKey = GetDerivedKey(IMK, PAN, PANSequenceNo)
            'Generate the MAC to get the IVCVC3.
            Dim IV As String = GetIVMac(trackData, KD)
            'Append MAC+UN+ATC.
            Dim data As String = IV + UN + ATC
            'Encrypt above with the derived key.
            Dim result As String = Cryptography.TripleDES.TripleDESEncrypt(KD, data)
            'Two least significant bytes are the result - return in decimal.
            Dim b(7) As Byte
            Utility.HexStringToByteArray(result, b)
            Return (b(6) * 256 + b(7)).ToString.PadLeft(5, "0"c)
        End Function

        ''' <summary>
        ''' Calculates the derived key using the initial key, the PAN and the sequence number.
        ''' </summary>
        ''' <param name="IMK">Initial key.</param>
        ''' <param name="PAN">PAN.</param>
        ''' <param name="PANSequenceNo">PAN sequence number.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetDerivedKey(ByVal IMK As Cryptography.HexKey, ByVal PAN As String, ByVal PANSequenceNo As String) As Cryptography.HexKey
            'Add sequence number to PAN and pad to at least 16 digits. 
            'Then get the rightmost sixteen digits to form an 8-byte block.
            Dim Y As String = PAN + PANSequenceNo
            If Y.Length < 16 Then
                Y = Y.PadLeft(16, "0"c)
            End If
            Y = Y.Substring(Y.Length - 16)

            'Left key is the result of encrypting Y with the IMK.
            Dim ZL As String = Cryptography.TripleDES.TripleDESEncrypt(IMK, Y)
            'Right key is the result of encrypting Y XOR FFs with the IMK.
            Dim ZR As String = Cryptography.TripleDES.TripleDESEncrypt(IMK, Utility.XORHexStringsFull(Y, "FFFFFFFFFFFFFFFF"))

            'Left+Right = the derived key.
            Return New Cryptography.HexKey(Utility.MakeParity(ZL + ZR, Utility.ParityCheck.OddParity))
        End Function

        ''' <summary>
        ''' Calculates a MAC on the track data using the derived key.
        ''' </summary>
        ''' <param name="trackData">Track data used for calculation.</param>
        ''' <param name="KD">Derived key.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetIVMac(ByVal trackData As String, ByVal KD As Cryptography.HexKey) As String
            'Normalize - this is used to extract track-II data and replace with nominal characters.
            'This should not be normally needed.
            Dim data As String = trackData.Replace("=", "D").Replace("?", "F")

            'Put zeroes in standard places
            Dim DIndex As Integer = data.IndexOf("D")
            data = data.Substring(0, DIndex + 1 + 8) + "000" + data.Substring(DIndex + 12, 2) + "0000000" + data.Substring(DIndex + 21, 1)

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
            Return intermediateryData.Substring(12, 4)
        End Function

    End Class

End Namespace
