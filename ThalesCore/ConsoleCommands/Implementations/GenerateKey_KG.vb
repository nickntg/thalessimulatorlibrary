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
    ''' Generates a key.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <ThalesConsoleCommandCode("KG", "Generates a key.")> _
    Public Class GenerateKey_KG
        Inherits AConsoleCommand

        Private CLEAR_XOR_KEYS As String = "X"
        Private HALF_THIRD_KEYS As String = "H"
        Private ENCRYPTED_KEYS As String = "E"

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            _stack.PushToStack(New ConsoleMessage("Enter ZMK check value [ENTER TO SKIP CV TEST]: ", "", New Validators.CheckValueValidator(True)))
            _stack.PushToStack(New ConsoleMessage("Enter encrypted ZMK [ENTER FOR NONE]: ", "", New Validators.FlexibleHexKeyValidator(True)))
            _stack.PushToStack(New ConsoleMessage("Key Scheme (ZMK) [ENTER FOR NONE]: ", "", New Validators.KeySchemeValidator(True)))
            _stack.PushToStack(New ConsoleMessage("Key Scheme (LMK): ", "", New Validators.KeySchemeValidator))
            _stack.PushToStack(New ConsoleMessage("Key Type: ", "", New Validators.KeyTypeValidator))
            _stack.PushToStack(New ConsoleMessage("Key length [1,2,3]: ", "", New ExtendedValidator(New Validators.AuthorizedStateValidator) _
                                                                                           .AddLast(New Validators.KeyLengthValidator)))
        End Sub

        ''' <summary>
        ''' Generate a key and returns it under LMK and optionally under ZMK.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme, zmkKS As KeySchemeTable.KeyScheme, finalKS As KeySchemeTable.KeyScheme

            Dim ZMKCV As String = _inStack.PopFromStack.ConsoleMessage
            Dim cryptZMK As String = _inStack.PopFromStack.ConsoleMessage
            Dim keySchemeZMK As String = _inStack.PopFromStack.ConsoleMessage
            Dim keySchemeLMK As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyLen As String = _inStack.PopFromStack.ConsoleMessage

            ValidateKeySchemeAndLength(keyLen, keySchemeLMK, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(rndKey), ZEROES)

            'Do we also encrypt under ZMK?
            If cryptZMK <> "" Then
                If keySchemeZMK = "" Then
                    Throw New Exception("INVALID KEY SCHEME FOR ZMK")
                End If

                'Must pass same validations
                ValidateKeySchemeAndLength(keyLen, keySchemeZMK, finalKS)

                'Get key scheme of typed-in ZMK
                Try
                    zmkKS = KeySchemeTable.GetKeySchemeFromValue(cryptZMK.Substring(0, 1))
                    'Truncate leading character.
                    cryptZMK = cryptZMK.Substring(1)
                Catch ex As Exception
                    'It's single-length key.
                    zmkKS = KeySchemeTable.KeyScheme.SingleDESKey
                End Try

                Dim clearZMK As String = Utility.DecryptZMKEncryptedUnderLMK(cryptZMK, zmkKS, 0)

                If ZMKCV <> "" Then
                    'Verify ZMK check value.
                    Dim zmkProperCV As String = TripleDES.TripleDESEncrypt(New HexKey(clearZMK), ZEROES)
                    If zmkProperCV.Substring(0, 6) <> ZMKCV Then
                        Return "KEY CHECK FAILED"
                    End If
                End If

                Dim cryptForTransmission As String = Utility.EncryptUnderZMK(clearZMK, rndKey, finalKS)
                Return "Key under LMK: " + MakeKeyPresentable(cryptRndKey) + vbCrLf + _
                       "Key encrypted for transmission: " + MakeKeyPresentable(cryptForTransmission) + vbCrLf + _
                       "Key check value: " + MakeCheckValuePresentable(chkVal)
            Else
                'Just the key.
                Return "Key under LMK: " + MakeKeyPresentable(cryptRndKey) + vbCrLf + _
                       "Key check value: " + MakeCheckValuePresentable(chkVal)
            End If

        End Function

    End Class

End Namespace