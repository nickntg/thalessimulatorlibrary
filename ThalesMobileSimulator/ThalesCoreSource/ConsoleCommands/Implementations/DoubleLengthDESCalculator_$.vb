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
    ''' Double-length DES calculator.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("$", "Encrypts data using a double-length DES key.")> _
    Public Class DoubleLengthDESCalculator_Dollar
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            m_stack.PushToStack(New ConsoleMessage("Enter data: ", "", New Validators.HexKeyValidator))
            m_stack.PushToStack(New ConsoleMessage("Enter key: ", "", New Validators.HexKeyValidator))
        End Sub

        ''' <summary>
        ''' Encrypts and returns the result
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String

            Dim data As String = m_inStack.PopFromStack.ConsoleMessage
            Dim desKey As String = m_inStack.PopFromStack.ConsoleMessage

            If desKey.Length <> 32 Then
                Return "INVALID KEY"
            End If

            If data.Length <> 16 Then
                Return "INVALID DATA"
            End If

            If Utility.IsParityOK(desKey, Utility.ParityCheck.OddParity) = False Then
                Return "KEY PARITY ERROR"
            End If

            Dim hk As New HexKey(desKey)
            Dim crypt As String = TripleDES.TripleDESEncrypt(hk, data)
            Dim decrypt As String = TripleDES.TripleDESDecrypt(hk, data)

            Return "Encrypted: " + MakeKeyPresentable(crypt) + vbCrLf + _
                   "Decrypted: " + MakeKeyPresentable(decrypt)
        End Function

    End Class

End Namespace