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
    ''' Exports a key.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <ThalesConsoleCommandCode("KE", "Exports a key.")> _
    Public Class ExportKey_KE
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            _stack.PushToStack(New ConsoleMessage("Enter encrypted key: ", "", New Validators.FlexibleHexKeyValidator))
            _stack.PushToStack(New ConsoleMessage("Enter encrypted ZMK: ", "", New Validators.FlexibleHexKeyValidator))
            _stack.PushToStack(New ConsoleMessage("Key Scheme: ", "", New Validators.KeySchemeValidator))
            _stack.PushToStack(New ConsoleMessage("Key Type: ", "", New ExtendedValidator(New Validators.AuthorizedStateValidator) _
                                                                                 .AddLast(New Validators.KeyTypeValidator)))
        End Sub

        ''' <summary>
        ''' Exports a key encrypted under LMK and returns it encrypted under the ZMK.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme, kl As HexKey.KeyLength, zmkKS As KeySchemeTable.KeyScheme, zmkKL As HexKey.KeyLength

            Dim cryptKey As String = _inStack.PopFromStack.ConsoleMessage
            Dim cryptZMK As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyScheme As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = _inStack.PopFromStack.ConsoleMessage

            ExtractKeySchemeAndLength(cryptZMK, zmkKL, zmkKS)
            ExtractKeySchemeAndLength(cryptKey, kl, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            Dim clearZMK As String = Utility.DecryptZMKEncryptedUnderLMK(New HexKey(cryptZMK).ToString, zmkKS, 0)
            Dim clearKey As String = Utility.DecryptUnderLMK(New HexKey(cryptKey).ToString, ks, LMKKeyPair, var)
            'TripleDES.TripleDESDecrypt(New HexKey(clearZMK), New HexKey(cryptKey).ToString)
            Dim cryptUnderZMK As String = Utility.EncryptUnderZMK(clearZMK, New HexKey(clearKey).ToString, KeySchemeTable.GetKeySchemeFromValue(keyScheme))
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            'Just the key.
            Return "Key encrypted under ZMK: " + MakeKeyPresentable(cryptUnderZMK) + vbCrLf + _
                   "Key Check Value: " + MakeCheckValuePresentable(chkVal)
        End Function

    End Class

End Namespace