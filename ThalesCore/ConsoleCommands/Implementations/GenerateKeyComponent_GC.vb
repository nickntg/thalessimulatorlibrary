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
    ''' Generate a key component command.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("GC", "Generates a key component.")> _
    Public Class GenerateKeyComponent_GC
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            _stack.PushToStack(New ConsoleMessage("Key Scheme: ", "", New Validators.KeySchemeValidator))
            _stack.PushToStack(New ConsoleMessage("Key Type: ", "", New Validators.KeyTypeValidator))
            _stack.PushToStack(New ConsoleMessage("Key length [1,2,3]: ", "", New Validators.KeyLengthValidator))
        End Sub

        ''' <summary>
        ''' Generate and return the key (clear and encrypted, along with check value).
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme

            Dim keyScheme As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = _inStack.PopFromStack.ConsoleMessage
            Dim keyLen As String = _inStack.PopFromStack.ConsoleMessage

            ValidateKeySchemeAndLength(keyLen, keyScheme, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(rndKey), ZEROES)

            Return "Clear Component: " + MakeKeyPresentable(rndKey) + vbCrLf + _
                   "Encrypted Component: " + MakeKeyPresentable(cryptRndKey) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkVal)
        End Function

    End Class

End Namespace