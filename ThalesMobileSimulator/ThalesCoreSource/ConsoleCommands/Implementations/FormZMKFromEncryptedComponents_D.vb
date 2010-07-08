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
    ''' Forms a ZMK from encrypted components.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("D", "Forms a ZMK from encrypted components.")> _
    Public Class FormZMKFromEncryptedComponents_D
        Inherits AConsoleCommand

        ''' <summary>
        ''' Stack initialization.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
            m_stack.PushToStack(New ConsoleMessage("Enter encrypted", "", False, True, New Validators.HexKeyValidator))
            m_stack.PushToStack(New ConsoleMessage("Enter number of components (2-9): ", "", True, New ExtendedValidator(New Validators.NumberOfComponentsValidator) _
                                                                                                               .AddLast(New Validators.AuthorizedStateValidator)))
        End Sub

        ''' <summary>
        ''' Generate and return the key (clear and encrypted, along with check value).
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            'Note that the components are stored in reverse order.
            Dim components(8) As String, idx As Integer = 0
            While True
                Dim msg As ConsoleMessage = m_inStack.PopFromStack
                If msg.IsNumberOfComponents = False Then
                    components(idx) = msg.ConsoleMessage
                    idx += 1
                Else
                    Exit While
                End If
            End While

            ReDim Preserve components(idx - 1)

            'Make sure all keys are of the same length.
            If AllSameLength(components) = False Then
                Throw New Exception("DATA INVALID; ALL KEYS MUST BE OF THE SAME LENGTH")
            End If

            'Make sure they're not triple-length keys.
            If components(0).Length = 48 Then
                Return "TRIPLE LENGTH COMPONENT NOT SUPPORTED"
            End If

            Dim clearKeys() As String
            ReDim clearKeys(idx - 1)
            Dim ks As KeySchemeTable.KeyScheme = New HexKey(components(0)).Scheme

            For i As Integer = 0 To clearKeys.GetUpperBound(0)
                clearKeys(i) = Utility.DecryptZMKEncryptedUnderLMK(components(i), ks, 0)
            Next

            Dim finalKey As HexKey = New HexKey(XORAllKeys(clearKeys))

            'Force odd parity, encrypt under LMK and get check value.
            finalKey = New HexKey(Utility.MakeParity(finalKey.ToString, Utility.ParityCheck.OddParity))
            Dim cryptKey As String = Utility.EncryptUnderLMK(finalKey.ToString, ks, LMKPairs.LMKPair.Pair04_05, "0")
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