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
Imports ThalesSim.Core.Resources
Imports ThalesSim.Core.Cryptography

Namespace ConsoleCommands

    ''' <summary>
    ''' Forms a key from components (clear or encrypted).
    ''' </summary>
    ''' <remarks>Obviously, input from smart cards is not supported.
    ''' 
    ''' The current message stack arrangement does not permit the extended checking of user data.
    ''' For example, components may be 8 or 16 hexadecimal characters if clear half or clear third key
    ''' component types are selected. But, if clear XOR components are selected then they can be
    ''' 16, 32 or 48 characters.
    ''' </remarks>
    <ThalesConsoleCommandCode("FK", "Forms a key from components.")> _
    Public Class FormKeyFromComponents_FK
        Inherits AConsoleCommand

        Private CLEAR_XOR_KEYS As String = "X"
        Private HALF_THIRD_KEYS As String = "H"
        Private ENCRYPTED_KEYS As String = "E"

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            _stack.PushToStack(New ConsoleMessage("Enter", "", False, True, New Validators.FlexibleHexKeyValidator))
            _stack.PushToStack(New ConsoleMessage("Enter number of components (2-9): ", "", True, New Validators.NumberOfComponentsValidator))
            _stack.PushToStack(New ConsoleMessage("Component type [X,H,E,S]: ", "", New Validators.ComponentTypeValidator))
            _stack.PushToStack(New ConsoleMessage("Key Scheme: ", "", New Validators.KeySchemeValidator))
            _stack.PushToStack(New ConsoleMessage("Key Type: ", "", New Validators.KeyTypeValidator))
            _stack.PushToStack(New ConsoleMessage("Key length [1,2,3]: ", "", New ExtendedValidator(New Validators.AuthorizedStateValidator) _
                                                                                           .AddLast(New Validators.KeyLengthValidator)))
        End Sub

        ''' <summary>
        ''' Generate and return the key (clear and encrypted, along with check value).
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme

            'Note that the components are stored in reverse order.
            Dim components(8) As String, idx As Integer = 0
            While True
                Dim msg As ConsoleMessage = _inStack.PopFromStack
                If msg.IsNumberOfComponents = False Then
                    components(idx) = msg.ConsoleMessage
                    idx += 1
                Else
                    Exit While
                End If
            End While

            ReDim Preserve components(idx - 1)

            Dim compType As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyScheme As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyLen As String = _inStack.PopFromStack.ConsoleMessage

            ValidateKeySchemeAndLength(keyLen, keyScheme, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            '' From now on, it's very messy.

            'Make sure all keys are of the same length.
            If AllSameLength(components) = False Then
                Throw New Exception("DATA INVALID; ALL KEYS MUST BE OF THE SAME LENGTH")
            End If

            'Validate XOR
            If compType = CLEAR_XOR_KEYS Then
                Select Case ks
                    Case KeySchemeTable.KeyScheme.SingleDESKey
                        If components(0).Length <> 16 Then
                            Throw New Exception("DATA INVALID; KEYS MUST BE 16 HEX CHARACTERS")
                        End If
                    Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                        If components(0).Length <> 32 Then
                            Throw New Exception("DATA INVALID; KEYS MUST BE 32 HEX CHARACTERS")
                        End If
                    Case KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                        If components(0).Length <> 48 Then
                            Throw New Exception("DATA INVALID; KEYS MUST BE 48 HEX CHARACTERS")
                        End If
                End Select
            End If

            'Validate half or third-length keys.
            If compType = HALF_THIRD_KEYS Then
                Select Case ks
                    Case KeySchemeTable.KeyScheme.SingleDESKey
                        If components(0).Length <> 8 Then
                            Throw New Exception("DATA INVALID; SINGLE-LENGTH HALF-KEYS MUST BE 8 HEX CHARACTERS")
                        End If
                        If idx <> 2 Then
                            Throw New Exception("DATA INVALID; THERE MUST BE 2 HALF-KEYS")
                        End If
                    Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                        If components(0).Length <> 16 Then
                            Throw New Exception("DATA INVALID; DOUBLE-LENGTH HALF-KEYS MUST BE 16 HEX CHARACTERS")
                        End If
                        If idx <> 2 Then
                            Throw New Exception("DATA INVALID; THERE MUST BE 2 HALF-KEYS")
                        End If
                    Case KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                        If components(0).Length <> 16 Then
                            Throw New Exception("DATA INVALID; TRIPLE-LENGTH THIRD-KEYS MUST BE 16 HEX CHARACTERS")
                        End If
                        If idx <> 3 Then
                            Throw New Exception("DATA INVALID; THERE MUST BE 3 THIRD-KEYS")
                        End If
                End Select
            End If

            'Validate encrypted keys
            If compType = ENCRYPTED_KEYS Then
                Select Case ks
                    Case KeySchemeTable.KeyScheme.SingleDESKey
                        If components(0).Length <> 16 Then
                            Throw New Exception("DATA INVALID; SINGLE-LENGTH ENCRYPTED COMPONENTS MUST BE 16 HEX CHARACTERS")
                        End If
                    Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                        If components(0).Length <> 33 Then
                            Throw New Exception("DATA INVALID; DOUBLE-LENGTH ENCRYPTED COMPONENTS MUST BE KEY SCHEME AND 32 HEX CHARACTERS")
                        End If
                        If AllSameStartChar(components) = False Then
                            Throw New Exception("DATA INVALID; DOUBLE-LENGTH ENCRYPTED COMPONENTS MUST ALL USE SAME KEY SCHEME")
                        End If
                    Case KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                        If components(0).Length <> 49 Then
                            Throw New Exception("DATA INVALID; TRIPLE-LENGTH ENCRYPTED COMPONENTS MUST BE KEY SCHEME AND 48 HEX CHARACTERS")
                        End If
                        If AllSameStartChar(components) = False Then
                            Throw New Exception("DATA INVALID; TRIPLE-LENGTH ENCRYPTED COMPONENTS MUST ALL USE SAME KEY SCHEME")
                        End If
                End Select
            End If

            'Find out what the clear key is.
            Dim finalKey As HexKey = Nothing
            Select Case compType
                Case HALF_THIRD_KEYS
                    Select Case ks
                        Case KeySchemeTable.KeyScheme.SingleDESKey
                            finalKey = New HexKey(components(1) + components(0))
                        Case Else
                            Dim keyStr As String = ""
                            For i As Integer = components.GetUpperBound(0) To 0 Step -1
                                keyStr = keyStr + components(i)
                            Next
                            finalKey = New HexKey(keyStr)
                    End Select
                Case CLEAR_XOR_KEYS
                    finalKey = New HexKey(XORAllKeys(components))
                Case ENCRYPTED_KEYS
                    'Assuming encrypted keys are of the same type as final key...
                    Dim clearKeys() As String
                    ReDim clearKeys(idx - 1)

                    For i As Integer = 0 To clearKeys.GetUpperBound(0)
                        clearKeys(i) = Utility.DecryptUnderLMK(components(i), ks, LMKKeyPair, var)
                    Next

                    finalKey = New HexKey(XORAllKeys(components))
            End Select

            'Force odd parity, encrypt under LMK and get check value.
            finalKey = New HexKey(Utility.MakeParity(finalKey.ToString, Utility.ParityCheck.OddParity))
            Dim cryptKey As String = Utility.EncryptUnderLMK(finalKey.ToString, ks, LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(finalKey, ZEROES)

            Return "Encrypted key: " + MakeKeyPresentable(cryptKey) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkVal)
        End Function

        ''' <summary>
        ''' Determines whether all keys in an array are of the same length.
        ''' </summary>
        ''' <param name="keys">Array with hexadecimal keys.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AllSameLength(ByVal keys() As String) As Boolean
            Dim len As Integer = keys(0).Length
            For i As Integer = 1 To keys.GetUpperBound(0)
                If keys(i).Length <> len Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' Determines whether all keys in an array begin with the same prefix.
        ''' </summary>
        ''' <param name="keys">Array with hexadecimal keys.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AllSameStartChar(ByVal keys() As String) As Boolean
            Dim s As String = keys(0).Substring(0, 1)
            For i As Integer = 1 To keys.GetUpperBound(0)
                If keys(i).Substring(0, 1) <> s Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' XORs all keys in an array.
        ''' </summary>
        ''' <param name="keys">Array with hexadecimal keys.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function XORAllKeys(ByVal keys() As String) As String
            Dim xorred As String = keys(0)
            For i As Integer = 1 To keys.GetUpperBound(0)
                xorred = Utility.XORHexStringsFull(xorred, keys(i))
            Next
            Return xorred
        End Function

    End Class

End Namespace