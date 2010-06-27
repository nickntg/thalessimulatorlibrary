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

''' <summary>
''' This class implements a control that can pretty-display a hexadecimal key.
''' </summary>
''' <remarks></remarks>
Public Class HexDisplayControl

    Public Sub LoadKey(ByVal keyValue As String)
        If keyValue = "" Then
            txtFirst.Text = ""
            txtSecond.Text = ""
            txtThird.Text = ""
        Else
            LoadKey(New ThalesSim.Core.Cryptography.HexKey(keyValue))
        End If
    End Sub

    Public Sub LoadKey(ByVal key As ThalesSim.Core.Cryptography.HexKey)
        Select Case key.KeyLen
            Case ThalesSim.Core.Cryptography.HexKey.KeyLength.SingleLength
                txtFirst.Text = key.PartA
                txtSecond.Text = ""
                txtThird.Text = ""
            Case ThalesSim.Core.Cryptography.HexKey.KeyLength.DoubleLength
                txtFirst.Text = key.PartA
                txtSecond.Text = key.PartB
                txtThird.Text = ""
            Case ThalesSim.Core.Cryptography.HexKey.KeyLength.TripleLength
                txtFirst.Text = key.PartA
                txtSecond.Text = key.PartB
                txtThird.Text = key.PartC
        End Select
    End Sub

    Public Sub ClearKey()
        txtFirst.Text = ""
        txtSecond.Text = ""
        txtThird.Text = ""
    End Sub

End Class
