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

Imports ThalesSim.Core
Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography

Namespace HostCommands

    ''' <summary>
    ''' This is the base class for all implementations of a Racal host command.
    ''' </summary>
    ''' <remarks>
    ''' All valid Racal host commands typically accept a request message and generate
    ''' a response message. Some commands that involve printer I/O return an additional
    ''' response message after printer I/O is concluded.
    ''' </remarks>
    Public Class AHostCommand

        '''' <summary>
        '''' Format placeholder.
        '''' </summary>
        '''' <remarks>
        '''' This variable is used to hold the request message definition. At this level,
        '''' the request message is comprised of everything except the header and the
        '''' command code.
        '''' </remarks>
        'Protected MFPC As ThalesSim.Core.Message.MessageFieldParserCollection

        ''' <summary>
        ''' Printer data.
        ''' </summary>
        ''' <remarks>
        ''' This variable is used to hold any data that the command directs to the 
        ''' attached printer.
        ''' </remarks>
        Protected m_PrinterData As String = ""

        ''' <summary>
        ''' XML message fields.
        ''' </summary>
        ''' <remarks></remarks>
        Protected m_msgFields As XML.MessageFields

        ''' <summary>
        ''' XML parsing result.
        ''' </summary>
        ''' <remarks></remarks>
        Protected m_XMLParseResult As String = ErrorCodes.ER_00_NO_ERROR

        ''' <summary>
        ''' Parsed key-value pairs.
        ''' </summary>
        ''' <remarks></remarks>
        Protected kvp As New XML.MessageKeyValuePairs

        ''' <summary>
        ''' Get/set the message fields definitions to be parsed.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property XMLMessageFields() As XML.MessageFields
            Get
                Return m_msgFields
            End Get
            Set(ByVal value As XML.MessageFields)
                m_msgFields = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the result of the XML parsing.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property XMLParseResult() As String
            Get
                Return m_XMLParseResult
            End Get
            Set(ByVal value As String)
                m_XMLParseResult = value
            End Set
        End Property

        ''' <summary>
        ''' Get the message key/value pairs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property KeyValuePairs() As XML.MessageKeyValuePairs
            Get
                Return kvp
            End Get
        End Property

        ''' <summary>
        ''' Returns data printed by this command.
        ''' </summary>
        ''' <remarks>
        ''' Returns data printed by this command. If the string is empty, no data have
        ''' been printed.
        ''' </remarks>
        Public ReadOnly Property PrinterData() As String
            Get
                Return m_PrinterData
            End Get
        End Property

        ''' <summary>
        ''' Called to process a host request message.
        ''' </summary>
        ''' <remarks>
        ''' Override this method to perform request message parsing. The caller will call
        ''' this method before a call to <see cref="HostCommands.AHostCommand.ConstructResponse"/>.
        ''' </remarks>
        Public Overridable Sub AcceptMessage(ByVal msg As Message.Message)
        End Sub

        ''' <summary>
        ''' Called to return a response message.
        ''' </summary>
        ''' <remarks>
        ''' Override this method to create a response message. At this level, the response
        ''' message does <b>not</b> include the message header or the response code.
        ''' 
        ''' This method is called after <see cref="HostCommands.AHostCommand.AcceptMessage"/>.
        ''' </remarks>
        Public Overridable Function ConstructResponse() As Message.MessageResponse
            Return Nothing
        End Function

        ''' <summary>
        ''' Called to return a response message after performing printer I/O.
        ''' </summary>
        ''' <remarks>
        ''' Override this method to create a response message. At this level, the response
        ''' message does <b>not</b> include the message header or the response code. If
        ''' the specific host command is not supposed to perform printer I/O, return Nothing.
        ''' 
        ''' This method is called after <see cref="HostCommands.AHostCommand.ConstructResponse"/>.
        ''' </remarks>
        Public Overridable Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            Return Nothing
        End Function

        ''' <summary>
        ''' Called to perform cleanup.
        ''' </summary>
        ''' <remarks>
        ''' This method is called after <see cref="HostCommands.AHostCommand.ConstructResponseAfterOperationComplete"/>.
        ''' </remarks>
        Public Overridable Sub Terminate()
            'MFPC = Nothing
        End Sub

        ''' <summary>
        ''' Returns a text dump of the parsed fields.
        ''' </summary>
        ''' <remarks>
        ''' This method returns a text dump of the message fields and their values.
        ''' </remarks>
        Public Function DumpFields() As String
            Return kvp.ToString
        End Function

        ''' <summary>
        ''' Parses and validates a key type code.
        ''' </summary>
        ''' <remarks>
        ''' This method parses a key type code. If all is well, the <b>Pair</b> and <b>Var</b> 
        ''' output variables are set and the method returns <b>True</b>. Otherwise, an appropriate
        ''' error code is added to the passed response message and the method returns <b>False</b>.
        ''' </remarks>
        Protected Function ValidateKeyTypeCode(ByVal ktc As String, _
                                               ByRef Pair As LMKPairs.LMKPair, _
                                               ByRef Var As String, _
                                               ByRef MR As MessageResponse) As Boolean

            Try
                KeyTypeTable.ParseKeyTypeCode(ktc, Pair, Var)
                Return True
            Catch ex As Exceptions.XInvalidKeyType
                MR.AddElement(ErrorCodes.ER_04_INVALID_KEY_TYPE_CODE)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Parses and validates a key scheme code.
        ''' </summary>
        ''' <remarks>
        ''' This method parses a key scheme code. If all is well, the <b>KS</b> output variable
        ''' is set and the method returns <b>True</b>. Otherwise, an appropriate error code is
        ''' added to the passed response message and the method returns <b>False</b>.
        ''' </remarks>
        Protected Function ValidateKeySchemeCode(ByVal ksc As String, _
                                                 ByRef KS As KeySchemeTable.KeyScheme, _
                                                 ByRef MR As MessageResponse) As Boolean

            Try
                KS = KeySchemeTable.GetKeySchemeFromValue(ksc)
                Return True
            Catch ex As Exceptions.XInvalidKeyScheme
                MR.AddElement(ErrorCodes.ER_26_INVALID_KEY_SCHEME)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Determines whether function requirements are met.
        ''' </summary>
        ''' <remarks>
        ''' If the function requirements are met for the specified parameters, the method returns
        ''' <b>True</b>. Otherwise, an appropriate error code is added to the passed response
        ''' message and the method returns <b>False</b>.
        ''' </remarks>
        Protected Function ValidateFunctionRequirement(ByVal func As KeyTypeTable.KeyFunction, ByVal Pair As LMKPairs.LMKPair, ByVal var As String, _
                                                       ByRef MR As MessageResponse) As Boolean

            Dim requirement As KeyTypeTable.AuthorizedStateRequirement = KeyTypeTable.GetAuthorizedStateRequirement(KeyTypeTable.KeyFunction.Generate, Pair, var)
            If requirement = KeyTypeTable.AuthorizedStateRequirement.NotAllowed Then
                MR.AddElement(ErrorCodes.ER_29_FUNCTION_NOT_PERMITTED)
                Return False
            ElseIf requirement = KeyTypeTable.AuthorizedStateRequirement.NeedsAuthorizedState AndAlso Convert.ToBoolean(Resources.GetResource(Resources.AUTHORIZED_STATE)) = False Then
                MR.AddElement(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return False
            Else
                Return True
            End If

        End Function

        ''' <summary>
        ''' Determines whether the simulator is in the authorized state.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function IsInAuthorizedState() As Boolean
            Return Convert.ToBoolean(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE))
        End Function

        ''' <summary>
        ''' Determines whether the simulator is in legacy mode.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function IsInLegacyMode() As Boolean
            Return Convert.ToBoolean(Core.Resources.GetResource(Core.Resources.LEGACY_MODE))
        End Function

        ''' <summary>
        ''' Decrypts data encrypted under a ZMK.
        ''' </summary>
        ''' <remarks>
        ''' This method may be used with Thales commands that decrypt key encrypted under a ZMK.
        ''' </remarks>
        Protected Function DecryptUnderZMK(ByVal clearZMK As String, ByVal cryptData As String, _
                                           ByVal ZMK_KeyScheme As KeySchemeTable.KeyScheme) As String

            Return DecryptUnderZMK(clearZMK, cryptData, ZMK_KeyScheme, String.Empty)

        End Function

        ''' <summary>
        ''' Decrypts data encrypted under a ZMK after applying an Atalla Variant.
        ''' </summary>
        ''' <remarks>
        ''' This method may be used with Thales commands that decrypt key encrypted under a ZMK.
        ''' </remarks>
        Protected Function DecryptUnderZMK(ByVal clearZMK As String, ByVal cryptData As String, _
                                           ByVal ZMK_KeyScheme As KeySchemeTable.KeyScheme, _
                                           ByVal AtallaVariant As String) As String
            Dim result As String = ""

            clearZMK = Utility.TransformUsingAtallaVariant(clearZMK, AtallaVariant)

            Select Case ZMK_KeyScheme
                Case KeySchemeTable.KeyScheme.SingleDESKey, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.Unspecified
                    result = TripleDES.TripleDESDecrypt(New HexKey(clearZMK), cryptData)
                Case KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                    result = TripleDES.TripleDESDecryptVariant(New HexKey(clearZMK), cryptData)
            End Select

            Select Case ZMK_KeyScheme
                Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant, KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                    result = KeySchemeTable.GetKeySchemeValue(ZMK_KeyScheme) + result
            End Select

            Return result
        End Function

        ''' <summary>
        ''' Creates a random PIN.
        ''' </summary>
        ''' <remarks>
        ''' This method creates a random decimal PIN.
        ''' </remarks>
        Protected Function GetRandomPIN(ByVal pinLength As Integer) As String
            Dim rndMachine As Random = New Random
            Dim PIN As String = ""
            For i As Integer = 1 To pinLength
                PIN += Convert.ToString(rndMachine.Next(0, 10))
            Next
            rndMachine = Nothing
            Return PIN
        End Function

        ''' <summary>
        ''' Encrypts a PIN.
        ''' </summary>
        ''' <remarks>
        ''' The current implementation only adds a 0 character to the clear PIN.
        ''' </remarks>
        Protected Function EncryptPINForHostStorage(ByVal PIN As String) As String
            Return "0" + PIN
        End Function

        ''' <summary>
        ''' Decrypts a PIN.
        ''' </summary>
        ''' <remarks>
        ''' The current implementation only removes the leading PIN character.
        ''' </remarks>
        Protected Function DecryptPINUnderHostStorage(ByVal PIN As String) As String
            Return PIN.Substring(1)
        End Function

        ''' <summary>
        ''' Encrypts a PIN using the Thales algorithm.
        ''' </summary>
        ''' <remarks>
        ''' The current implementation only adds a 0 character to the clear PIN.
        ''' </remarks>
        Protected Function EncryptPINForHostStorageThales(ByVal PIN As String) As String
            Return "0" + PIN
        End Function

        ''' <summary>
        ''' Decrypts a PIN encrypted with the Racal algorithm.
        ''' </summary>
        ''' <remarks>
        ''' The current implementation only removes the leading PIN character.
        ''' </remarks>
        Protected Function DecryptPINUnderHostStorageRacal(ByVal PIN As String) As String
            Return PIN.Substring(1)
        End Function

        ''' <summary>
        ''' Generates a VISA PVV.
        ''' </summary>
        ''' <remarks>
        ''' This method creates a 4-digit VISA PVV.
        ''' </remarks>
        Protected Function GeneratePVV(ByVal AccountNumber As String, ByVal PVKI As String, ByVal PIN As String, ByVal PVKPair As String) As String

            Dim stage1 As String = AccountNumber.Substring(1, 11) + PVKI + PIN.Substring(0, 4)
            Dim stage2 As String = TripleDES.TripleDESEncrypt(New HexKey(PVKPair), stage1)
            Dim PVV As String = "", i As Integer

            While PVV.Length < 4
                i = 0
                While PVV.Length < 4 AndAlso i < stage2.Length
                    If Char.IsDigit(stage2.Chars(i)) Then PVV += stage2.Substring(i, 1)
                    i += 1
                End While
                If PVV.Length < 4 Then
                    For j As Integer = 0 To stage2.Length - 1
                        Dim newChar As String = " "
                        If Char.IsDigit(stage2.Chars(j)) = False Then
                            newChar = (Convert.ToInt32(stage2.Substring(j, 1), 16) - 10).ToString("X")
                        End If
                        stage2 = stage2.Remove(j, 1)
                        stage2 = stage2.Insert(j, newChar)
                    Next
                    stage2 = stage2.Replace(" ", "")
                End If
            End While

            Return PVV

        End Function

        ''' <summary>
        ''' Generates a VISA CVV.
        ''' </summary>
        ''' <remarks>
        ''' This method generates a VISA CVV.
        ''' </remarks>
        Protected Function GenerateCVV(ByVal CVKPair As String, ByVal AccountNumber As String, ByVal ExpirationDate As String, ByVal SVC As String) As String

            Dim CVKA As String = Utility.RemoveKeyType(CVKPair).Substring(0, 16)
            Dim CVKB As String = Utility.RemoveKeyType(CVKPair).Substring(16)
            Dim block As String = (AccountNumber + ExpirationDate + SVC).PadRight(32, "0"c)
            Dim blockA As String = block.Substring(0, 16)
            Dim blockB As String = block.Substring(16)

            Dim result As String = TripleDES.TripleDESEncrypt(New HexKey(CVKA), blockA)
            result = Utility.XORHexStrings(result, blockB)
            result = TripleDES.TripleDESEncrypt(New HexKey(CVKA + CVKB), result)

            Dim CVV As String = "", i As Integer = 0
            While CVV.Length < 3
                If Char.IsDigit(result.Chars(i)) Then
                    CVV += result.Substring(i, 1)
                End If
                i += 1
            End While

            Return CVV

        End Function

        ''' <summary>
        ''' Generates a AMEX CSC.
        ''' </summary>
        ''' <remarks>
        ''' This method generates a AMEX CSC.
        ''' </remarks>
        Protected Function GenerateCSC(ByVal CSCKPair As String, ByVal AccountNumber As String, ByVal ExpirationDate As String, ByVal SVC As String, ByVal CSCVersion As Integer) As String

            Dim CSCKA As String = Utility.RemoveKeyType(CSCKPair).Substring(0, 16)
            Dim CSCKB As String = Utility.RemoveKeyType(CSCKPair).Substring(16)
            Dim block As String
            Dim result As String = ""

            If AccountNumber.Substring(0, 2) = "37" Then
                block = ExpirationDate + AccountNumber.Substring(2, 12)
            ElseIf AccountNumber.Substring(0, 2) = "34" Then
                block = AccountNumber.Substring(2, 12) + ExpirationDate
            Else
                Return "-1"
            End If

            If CSCVersion = 0 Then
                result = TripleDES.TripleDESEncrypt(New HexKey(CSCKA + CSCKB), block)
            ElseIf CSCVersion = 2 Then
                block = (block + SVC).PadRight(32, "0"c)
                result = TripleDES.TripleDESEncrypt(New HexKey(CSCKA + CSCKB), block).Substring(16, 16)
            Else
                Return "-1"
            End If

            Dim CSC As String = "", i As Integer = 0
            For i = 0 To result.Length - 1
                If Char.IsDigit(result.Chars(i)) Then
                    CSC += result.Chars(i)
                End If
            Next
            i = 0
            Dim conv As Integer

            While CSC.Length < 12
                If Not Char.IsDigit(result.Chars(i)) Then
                    conv = Asc(result.Chars(i)) - 64
                    If i >= 5 Then
                        conv = (conv + 5) Mod 10
                    End If
                    CSC += Chr(conv + 48)
                End If
                i += 1
            End While

            Return CSC

        End Function

        ''' <summary>
        ''' Generates a MAC.
        ''' </summary>
        ''' <remarks>
        ''' Generates a message authentication code.
        ''' </remarks>
        Protected Function GenerateMAC(ByVal b() As Byte, ByVal key As String, ByVal IV As String) As String

            Dim curIndex As Integer = 0
            Dim result As String = ""

            While curIndex <= b.GetUpperBound(0)
                MACBytes(b, curIndex, IV, key, result)
                IV = result
            End While

            Return result

        End Function

        Private Sub MACBytes(ByVal b() As Byte, ByRef curIndex As Integer, ByVal IV As String, ByVal key As String, ByRef result As String)

            Dim dataStr As String = ""

            While dataStr.Length <> 16
                If curIndex <= b.GetUpperBound(0) Then
                    dataStr = dataStr + b(curIndex).ToString("X2")
                    curIndex += 1
                Else
                    dataStr = dataStr.PadRight(16, "0"c)
                End If
            End While

            dataStr = Utility.XORHexStringsFull(dataStr, IV)
            result = TripleDES.TripleDESEncrypt(New HexKey(key), dataStr)

        End Sub

        ''' <summary>
        ''' Adds a line to the printer output.
        ''' </summary>
        ''' <remarks>
        ''' Adds a line to the printer output.
        ''' </remarks>
        Protected Sub AddPrinterData(ByVal data As String)
            m_PrinterData = PrinterData + data + vbCrLf
        End Sub

        ''' <summary>
        ''' Clears the printer output.
        ''' </summary>
        ''' <remarks>
        ''' Clears the printer output.
        ''' </remarks>
        Protected Sub ClearPrinterData()
            m_PrinterData = ""
        End Sub

        ''' <summary>
        ''' Reads the message field definitions using
        ''' the class name to look for the xml file.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub ReadXMLDefinitions()
            ReadXMLDefinitions(Me.GetType.Name + ".xml")
        End Sub

        ''' <summary>
        ''' Reads the message field definitions using
        ''' the class name to look for the xml file.
        ''' </summary>
        ''' <param name="forceRead">True to force xml fields to be re-parsed 
        ''' and ignore the cache.</param>
        ''' <remarks></remarks>
        Protected Sub ReadXMLDefinitions(ByVal forceRead As Boolean)
            ReadXMLDefinitions(forceRead, Me.GetType.Name + ".xml")
        End Sub

        ''' <summary>
        ''' Reads the message field definitions using
        ''' a specific xml file name.
        ''' </summary>
        ''' <param name="fileName">XML file with definition.</param>
        ''' <remarks></remarks>
        Protected Sub ReadXMLDefinitions(ByVal fileName As String)
            ReadXMLDefinitions(False, fileName)
        End Sub

        ''' <summary>
        ''' Reads the message field definitions using
        ''' a specific xml file name.
        ''' </summary>
        ''' <param name="forceRead">True to force xml fields to be re-parsed 
        ''' and ignore the cache.</param>
        ''' <param name="fileName">XML file with definition.</param>
        ''' <remarks></remarks>
        Protected Sub ReadXMLDefinitions(ByVal forceRead As Boolean, ByVal fileName As String)
            If forceRead Then
                XML.MessageFieldsStore.Remove(Me.GetType.Name)
            End If

            XMLMessageFields = XML.MessageFieldsStore.Item(Me.GetType.Name)
            If XMLMessageFields Is Nothing Then
                XMLMessageFields = XML.MessageFields.ReadXMLFields(fileName)
                XML.MessageFieldsStore.Add(Me.GetType.Name, XMLMessageFields)
            End If
        End Sub

    End Class

End Namespace
