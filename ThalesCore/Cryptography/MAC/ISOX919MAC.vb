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

Imports ThalesSim.Core.Cryptography

Namespace Cryptography.MAC

    ''' <summary>
    ''' This class implements the ISO X9.19 MAC algorithm with zero-based padding.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ISOX919MAC

        ''' <summary>
        ''' Calculate a MAC using the X9.19 algorithm.
        ''' </summary>
        ''' <param name="dataStr">Hex data to MAC.</param>
        ''' <param name="key">MACing key.</param>
        ''' <param name="IV">Initial vector.</param>
        ''' <param name="block">Message block to MAC.</param>
        ''' <returns>MAC result.</returns>
        ''' <remarks></remarks>
        Public Shared Function MacHexData(ByVal dataStr As String, _
                                          ByVal key As HexKey, ByVal IV As String, _
                                          ByVal block As ISOX919Blocks) As String

            If dataStr.Length Mod 16 <> 0 Then
                dataStr = ThalesSim.Core.Cryptography.MAC.ISO9797Pad.PadHexString(dataStr, Cryptography.MAC.ISO9797PaddingMethods.PaddingMethod1)
            End If

            Dim result As String = dataStr

            For i As Integer = 0 To (dataStr.Length \ 16) - 1
                IV = Utility.XORHexStringsFull(IV, dataStr.Substring(i * 16, 16))
                IV = ThalesSim.Core.Cryptography.DES.DESEncrypt(key.PartA, IV)
            Next

            result = IV

            If block = ISOX919Blocks.FinalBlock OrElse block = ISOX919Blocks.OnlyBlock Then
                result = ThalesSim.Core.Cryptography.DES.DESDecrypt(key.PartB, IV)
                result = ThalesSim.Core.Cryptography.DES.DESEncrypt(key.PartA, result)
            End If
            Return result

        End Function

    End Class

End Namespace