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
    ''' Generate a ZMK component.
    ''' </summary>
    ''' <remarks>
    ''' Normally this would generate either a single or double-length ZMK, depending
    ''' upon the HSM configuration (accessible by the CS console command). Since we
    ''' don't have this configuration, we'll produce both a single and a double-length
    ''' ZMK and return it to the console.
    ''' </remarks>
    <ThalesConsoleCommandCode("F", "Generates a ZMK component.")> _
    Public Class GenerateZMKComponent_F
        Inherits AConsoleCommand

        ''' <summary>
        ''' No stack, since this is an immediate command.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
        End Sub

        ''' <summary>
        ''' Generate a ZMK component and return it (clear and encrypted) to the console.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            If Convert.ToBoolean(Resources.GetResource(Resources.AUTHORIZED_STATE)) = False Then
                Return "NOT AUTHORIZED"
            End If

            Dim rndKey As String = Utility.CreateRandomKey(KeySchemeTable.KeyScheme.SingleDESKey)
            rndKey = Utility.MakeParity(rndKey, Utility.ParityCheck.OddParity)
            Dim encrKey As String = Utility.EncryptUnderLMK(rndKey, KeySchemeTable.KeyScheme.SingleDESKey, LMKPairs.LMKPair.Pair04_05, "0")
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(rndKey), ZEROES)

            Dim rndKeyDbl As String = Utility.CreateRandomKey(KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi)
            rndKeyDbl = Utility.MakeParity(rndKeyDbl, Utility.ParityCheck.OddParity)
            Dim encrKeyDbl As String = Utility.EncryptUnderLMK(rndKeyDbl, KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, LMKPairs.LMKPair.Pair04_05, "0")
            Dim chkValDbl As String = TripleDES.TripleDESEncrypt(New HexKey(rndKeyDbl), ZEROES)

            Return "Clear ZMK Component: " + MakeKeyPresentable(rndKey) + vbCrLf + _
                   "Encrypted ZMK Component: " + MakeKeyPresentable(encrKey) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkVal) + vbCrLf + vbCrLf + _
                   "Clear ZMK Component: " + MakeKeyPresentable(rndKeyDbl) + vbCrLf + _
                   "Encrypted ZMK Component: " + MakeKeyPresentable(encrKeyDbl) + vbCrLf + _
                   "Key check value: " + MakeCheckValuePresentable(chkValDbl)
        End Function

    End Class

End Namespace