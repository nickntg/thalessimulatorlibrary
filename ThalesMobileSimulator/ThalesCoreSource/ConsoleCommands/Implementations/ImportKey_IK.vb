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
    ''' Imports a key.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    <ThalesConsoleCommandCode("IK", "Imports a key.")> _
    Public Class ImportKey_IK
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            m_stack.PushToStack(New ConsoleMessage("Enter key: ", "", New Validators.FlexibleHexKeyValidator))
            m_stack.PushToStack(New ConsoleMessage("Enter encrypted ZMK: ", "", New Validators.FlexibleHexKeyValidator))
            m_stack.PushToStack(New ConsoleMessage("Key Scheme: ", "", New Validators.KeySchemeValidator))
            m_stack.PushToStack(New ConsoleMessage("Key Type: ", "", New ExtendedValidator(New Validators.AuthorizedStateValidator) _
                                                                                 .AddLast(New Validators.KeyTypeValidator)))
        End Sub

        ''' <summary>
        ''' Imports a key encrypted under a ZMK and returns it encrypted under the appropriate LMK.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Making the assumption that a key transmitted under ZMK encryption
        ''' will always use ANSI format (therefore U is not allowed).</remarks>
        Public Overrides Function ProcessMessage() As String
            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme, kl As HexKey.KeyLength, zmkKS As KeySchemeTable.KeyScheme, zmkKL As HexKey.KeyLength

            Dim cryptKey As String = m_inStack.PopFromStack.ConsoleMessage
            Dim cryptZMK As String = m_inStack.PopFromStack.ConsoleMessage
            Dim keyScheme As String = m_inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = m_inStack.PopFromStack.ConsoleMessage

            ExtractKeySchemeAndLength(cryptZMK, zmkKL, zmkKS)
            ExtractKeySchemeAndLength(cryptKey, kl, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            If ks = KeySchemeTable.KeyScheme.DoubleLengthKeyVariant OrElse ks = KeySchemeTable.KeyScheme.TripleLengthKeyVariant Then
                Return "INVALID KEY SCHEME FOR ENCRYPTED KEY - MUST BE ANSI"
            End If

            Dim clearZMK As String = Utility.DecryptZMKEncryptedUnderLMK(New HexKey(cryptZMK).ToString, zmkKS, 0)
            Dim clearKey As String = TripleDES.TripleDESDecrypt(New HexKey(clearZMK), New HexKey(cryptKey).ToString)
            Dim cryptUnderLMK As String = Utility.EncryptUnderLMK(clearKey, KeySchemeTable.GetKeySchemeFromValue(keyScheme), LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            'Just the key.
            Return "Key under LMK: " + MakeKeyPresentable(cryptUnderLMK) + vbCrLf + _
                   "Key Check Value: " + MakeCheckValuePresentable(chkVal)
        End Function

    End Class

End Namespace