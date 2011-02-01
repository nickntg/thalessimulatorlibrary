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
    ''' Encrypts a clear component.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("EC", "Encrypts a clear component.")> _
    Public Class EncryptClearComponent_EC
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            m_stack.PushToStack(New ConsoleMessage("Enter component: ", "", New Validators.HexKeyValidator))
            m_stack.PushToStack(New ConsoleMessage("Key Scheme: ", "", New Validators.KeySchemeValidator))
            m_stack.PushToStack(New ConsoleMessage("Key Type: ", "", New ExtendedValidator(New Validators.AuthorizedStateValidator) _
                                                                                 .AddLast(New Validators.KeyTypeValidator)))
        End Sub

        ''' <summary>
        ''' Generate and return the key (clear and encrypted, along with check value).
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String

            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme

            Dim clearComponent As String = m_inStack.PopFromStack.ConsoleMessage
            Dim keyScheme As String = m_inStack.PopFromStack.ConsoleMessage
            Dim keyType As String = m_inStack.PopFromStack.ConsoleMessage
            Dim keyLen As String
            ''
            '' Bug (see item http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=154290).
            '' ValidateKeySchemeAndLength expects 1, 2, 3 instead of 0, 1, 2.
            ''
            'Select Case clearComponent.Length
            '    Case 16
            '        keyLen = "0"
            '    Case 32
            '        keyLen = "1"
            '    Case Else
            '        keyLen = "2"
            'End Select
            Select Case clearComponent.Length
                Case 16
                    keyLen = "1"
                Case 32
                    keyLen = "2"
                Case Else
                    keyLen = "3"
            End Select

            ValidateKeySchemeAndLength(keyLen, keyScheme, ks)
            ValidateKeyTypeCode(keyType, LMKKeyPair, var)

            'Ensure odd parity on the key.
            clearComponent = Utility.MakeParity(clearComponent, Utility.ParityCheck.OddParity)

            Dim cryptKey As String = Utility.EncryptUnderLMK(clearComponent, ks, LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(clearComponent), ZEROES)

            Return "Encrypted Component: " + MakeKeyPresentable(cryptKey) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkVal)
        End Function

    End Class

End Namespace