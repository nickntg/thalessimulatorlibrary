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
    ''' Encrypts a clear ZMK component.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("Z", "Encrypts a clear ZMK component.")> _
    Public Class EncryptZMKComponent_Z
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            m_stack.PushToStack(New ConsoleMessage("Enter ZMK Component: ", "", New ExtendedValidator(New Validators.HexKeyValidator) _
                                                                                            .AddLast(New Validators.AuthorizedStateValidator)))
        End Sub

        ''' <summary>
        ''' Generate a ZMK component and return it (clear and encrypted) to the console.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String

            Dim key As String = m_inStack.PopFromStack.ConsoleMessage
            If key.Length = 48 Then
                Return "TRIPLE LENGTH COMPONENT NOT SUPPORTED"
            End If

            If Not Utility.IsParityOK(key, Utility.ParityCheck.OddParity) Then
                Return "PARITY ERROR"
            End If

            Dim hxkey As New HexKey(key)
            Dim encrKey As String = Utility.EncryptUnderLMK(hxkey.ToString, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0")
            Dim chkVal As String = TripleDES.TripleDESEncrypt(hxkey, ZEROES)

            Return "Encrypted ZMK component: " + MakeKeyPresentable(hxkey.ToString) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkVal) + vbCrLf
        End Function

    End Class

End Namespace