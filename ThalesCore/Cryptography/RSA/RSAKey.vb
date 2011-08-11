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

Imports System.Security.Cryptography

Public NotInheritable Class RSAKey

    Private m_rsa As RSACryptoServiceProvider

    Public Property Key() As RSACryptoServiceProvider
        Get
            Return m_rsa
        End Get
        Set(ByVal value As RSACryptoServiceProvider)
            m_rsa = value
        End Set
    End Property

    Public ReadOnly Property PublicKey() As String
        Get
            Return m_rsa.ToXmlString(False)
        End Get
    End Property

    Public ReadOnly Property PrivateKey() As String
        Get
            Return m_rsa.ToXmlString(True)
        End Get
    End Property

    Public Sub New(ByVal keyLength As Integer, ByVal flags As CspProviderFlags)
        Dim parms As New CspParameters
        parms.Flags = flags
        m_rsa = New RSACryptoServiceProvider(parms)
    End Sub

    Public Sub New(ByVal xmlString As String)
        m_rsa = New RSACryptoServiceProvider
        m_rsa.FromXmlString(xmlString)
    End Sub

End Class
