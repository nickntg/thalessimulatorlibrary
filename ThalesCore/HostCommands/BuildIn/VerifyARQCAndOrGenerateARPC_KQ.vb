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
'' Contributed by nahsan.

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core
Imports ThalesSim.Core.PIN.PINBlockFormat

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Verifies an ARQC and optionally generates an ARPC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the KQ Thales command.
    ''' </remarks>
    <ThalesCommandCode("KQ", "KR", "", "ARQC verification and/or ARPC generation.")> _
    Public Class VerifyARQCAndOrGenerateARPC_KQ
        Inherits AHostCommand

        Private _modeFlag As String = ""
        Private _schemeID As String = ""
        Private _mkAC As String = ""
        Private _panSeqNo As String = ""
        Private _atc As String = ""
        Private _un As String = ""
        Private _tranDataLen As String = ""
        Private _delimeter As String = ""
        Private _tranData As String = ""
        Private _arqc As String = ""
        Private _arc As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the MA message parsing fields.
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
                _modeFlag = kvp.Item("Mode")
                _schemeID = kvp.Item("Scheme ID")
                _mkAC = kvp.ItemCombination("MK-AC Scheme", "MK-AC")
                _panSeqNo = kvp.Item("PAN")
                _atc = kvp.Item("ATC")
                _un = kvp.Item("UN")
                _tranDataLen = kvp.Item("Transaction Data Length")
                _tranData = kvp.Item("Transaction Data")
                _arqc = kvp.Item("ARQCTCAAC")
                _arc = kvp.Item("APC")
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
            Dim UDK As String
            Dim KeyA As String, KeyB As String, Data As String = ""
            Dim counter, counter1, counter2, ByteNo As Integer
            Dim Result, EncOutput, XOROutput, NextData As String
            NextData = ""

            Dim cryptMDK As New HexKey(_mkAC)
            Dim clearMDK As String = Utility.DecryptUnderLMK(cryptMDK.ToString, cryptMDK.Scheme, LMKPairs.LMKPair.Pair28_29, "1")

            UDK = DerivceICCMasterKey(clearMDK, _panSeqNo)

            KeyA = UDK.Substring(0, 16)
            KeyB = UDK.Substring(16, 16)

            counter1 = _tranData.Length
            counter2 = _mkAC.Length
            counter = Convert.ToInt32(counter1 / (counter2 / 2))

            For ByteNo = 1 To counter
                If Data = "" Then
                    Data = _tranData.Substring((ByteNo - 1) * 16, 16)
                End If

                EncOutput = DES.DESEncrypt(KeyA, Data)

                If ByteNo < counter Then
                    NextData = _tranData.Substring((ByteNo) * 16, 16)
                End If

                If ByteNo <> counter Then

                    XOROutput = Utility.XORHexStrings(EncOutput, NextData)
                    Data = XOROutput
                Else
                    Data = EncOutput
                End If
            Next

            Data = DES.DESDecrypt(KeyB, Data)
            Result = DES.DESEncrypt(KeyA, Data)

            If (Result = _arqc) Then
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)

                If _modeFlag = "1" Or _modeFlag = "2" Then
                    XOROutput = ""

                    Data = _arqc

                    XOROutput = Utility.XORHexStrings(Data, "8100000000000000")
                    Data = DES.DESEncrypt(KeyA, Data)
                    Data = DES.DESDecrypt(KeyB, Data)
                    Data = DES.DESEncrypt(KeyA, Data)
                    mr.AddElement(Data)

                End If

            Else
                mr.AddElement(ErrorCodes.ER_01_VERIFICATION_FAILURE)

            End If

            Log.Logger.MinorInfo("Result: " + Result)
            Log.Logger.MinorInfo("Transaction Data: " + _tranData)
            Log.Logger.MinorInfo("ARQC: " + _arqc)

            Return mr
        End Function

        Public Function DerivceICCMasterKey(ByVal IssuerMasterKey As String, ByVal Data As String) As String
            Dim ICCMasterKey As String

            If (Data.Length > 16) Then
                Data = Data.Substring(2, 16)
            End If

            ICCMasterKey = TripleDES.TripleDESEncrypt(New HexKey(IssuerMasterKey), Data)

            Data = Utility.XORHexStrings(Data, "FFFFFFFFFFFFFFFF")

            Data = TripleDES.TripleDESEncrypt(New HexKey(IssuerMasterKey), Data)

            ICCMasterKey = ICCMasterKey + Data
            ICCMasterKey = Utility.MakeParity(ICCMasterKey, Utility.ParityCheck.OddParity)

            Return ICCMasterKey
        End Function

    End Class

End Namespace
