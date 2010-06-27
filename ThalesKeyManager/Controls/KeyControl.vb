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
''' This class implements a control that displays all information
''' of a key.
''' </summary>
''' <remarks></remarks>
Public Class KeyControl

    Public Event KeyTypeChanged(ByVal sender As Object, ByVal keyName As String, ByVal keyType As KeyType)

    Protected m_KI As KeyInfo
    Protected suspendEvents As Boolean = False

    Public Property Key() As KeyInfo
        Get
            Return m_KI
        End Get
        Set(ByVal value As KeyInfo)
            suspendEvents = True
            If m_KI Is Nothing AndAlso cboKeyType.Items.Count = 0 Then
                Dim strs() As String = [Enum].GetNames(GetType(KeyType))
                For Each s As String In strs
                    cboKeyType.Items.Add(s)
                Next
            End If
            m_KI = value
            If m_KI IsNot Nothing Then
                txtKeyName.Text = m_KI.KeyName
                UpdateDisplay()
            Else
                ClearDisplay()
            End If
            suspendEvents = False
        End Set
    End Property

    Private Sub UpdateDisplay()
        hClearValue.LoadKey(m_KI.Key.GetClearValue)
        hANSIValue.LoadKey(m_KI.Key.GetANSIValue)
        If m_KI.Key.GetClearValue.Length <> 16 Then hVariantValue.LoadKey(m_KI.Key.GetVariantValue)
        hPlainValue.LoadKey(m_KI.Key.GetEncryptedValue)
        txtCV.Text = m_KI.Key.GetCheckValue
        cboKeyType.SelectedItem = m_KI.Key.KeyType.ToString
    End Sub

    Private Sub ClearDisplay()
        hClearValue.ClearKey()
        hANSIValue.ClearKey()
        hVariantValue.ClearKey()
        hPlainValue.ClearKey()
        txtCV.Text = ""
        cboKeyType.SelectedItem = -1
    End Sub

    Private Sub cboKeyType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboKeyType.SelectedIndexChanged
        m_KI.Key.KeyType = CType([Enum].Parse(GetType(KeyType), cboKeyType.Text), KeyType)
        UpdateDisplay()

        If suspendEvents = False Then RaiseEvent KeyTypeChanged(Me, txtKeyName.Text, m_KI.Key.KeyType)
    End Sub

    Private Sub cmdCopyClear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyClear.Click
        Clipboard.SetText(m_KI.Key.GetClearValue)
    End Sub

    Private Sub cmdCopyPlain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyPlain.Click
        Clipboard.SetText(m_KI.Key.GetEncryptedValue)
    End Sub

    Private Sub cmdCopyANSI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyANSI.Click
        Clipboard.SetText(m_KI.Key.GetANSIValue)
    End Sub

    Private Sub cmdCopyVariant_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyVariant.Click
        Clipboard.SetText(m_KI.Key.GetVariantValue)
    End Sub
End Class
