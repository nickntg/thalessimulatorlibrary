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

Namespace Cryptography.MAC

    Public Class ISO9797Pad

        ''' <summary>
        ''' Pads a string to an 8-byte boundary.
        ''' </summary>
        ''' <param name="dataStr">Data string to pad.</param>
        ''' <param name="paddingMethod">The padding method to use.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function PadHexString(ByVal dataStr As String, ByVal paddingMethod As ISO9797PaddingMethods) As String
            If paddingMethod = ISO9797PaddingMethods.NoPadding Then
                Return dataStr
            End If

            Dim firstPadString As String = "80"
            If paddingMethod = ISO9797PaddingMethods.PaddingMethod1 Then
                firstPadString = "00"
            End If

            If (dataStr.Length \ 2) Mod 8 = 0 Then
                Return dataStr + firstPadString + "00000000000000"
            Else
                dataStr = dataStr + firstPadString
                While (dataStr.Length \ 2) Mod 8 <> 0
                    dataStr = dataStr + "00"
                End While
                Return dataStr
            End If
        End Function

    End Class

End Namespace