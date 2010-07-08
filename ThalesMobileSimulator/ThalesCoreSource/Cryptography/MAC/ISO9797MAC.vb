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
    ''' This class implements the ISO 9797 MACing methods.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ISO9797MAC

        ''' <summary>
        ''' Generates a MAC on a hexadecimal string.
        ''' </summary>
        ''' <param name="dataStr">Hexadecimal string for which to generate MAC.</param>
        ''' <param name="key">DES key to use.</param>
        ''' <param name="IV">Initial vector.</param>
        ''' <param name="algorithm">ISO9797 algorithm to use.</param>
        ''' <param name="padding">ISO9797 padding method to use.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function MACHexData(ByVal dataStr As String, _
                                          ByVal key As HexKey, ByRef IV As String, _
                                          ByVal algorithm As ISO9797Algorithms, ByVal padding As ISO9797PaddingMethods) As String

            dataStr = ISO9797Pad.PadHexString(dataStr, padding)
            If algorithm = ISO9797Algorithms.MACAlgorithm3 Then
                Return Algorithm3(dataStr, key, IV)
            Else
                Throw New NotImplementedException("Algorithm 1 is not currently implemented")
            End If

        End Function

        ''' <summary>
        ''' Implements algorithm 3 as defined in the ISO 9797.
        ''' </summary>
        ''' <param name="dataStr">String to generate MAC for.</param>
        ''' <param name="key">DES key to use.</param>
        ''' <param name="IV">Initial vector.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Algorithm3(ByVal dataStr As String, ByVal key As HexKey, ByRef IV As String) As String
            For i As Integer = 0 To (dataStr.Length \ 16) - 1
                IV = Utility.XORHexStringsFull(IV, dataStr.Substring(i * 16, 16))
                IV = DES.DESEncrypt(key.PartA, IV)
            Next
            Dim result As String = DES.DESDecrypt(key.PartB, IV)
            result = DES.DESEncrypt(key.PartA, result)
            Return result
        End Function

    End Class

End Namespace