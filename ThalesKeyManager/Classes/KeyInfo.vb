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
''' This class stores information about a key.
''' </summary>
''' <remarks></remarks>
Public Class KeyInfo

    Private m_keyName As String

    ''' <summary>
    ''' Get/set the key name.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property KeyName() As String
        Get
            Return m_keyName
        End Get
        Set(ByVal value As String)
            m_keyName = value
        End Set
    End Property

    Private m_key As CryptoKey

    ''' <summary>
    ''' Get/set the key value.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Key() As CryptoKey
        Get
            Return m_key
        End Get
        Set(ByVal value As CryptoKey)
            m_key = value
        End Set
    End Property

    ''' <summary>
    ''' Creates an instance of this class.
    ''' </summary>
    ''' <param name="keyName">Key name.</param>
    ''' <param name="clearKey">Clear key value.</param>
    ''' <param name="keyType">Key type.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal keyName As String, ByVal clearKey As String, ByVal keyType As KeyType)
        m_keyName = keyName
        m_key = New CryptoKey(clearKey)
        m_key.KeyType = keyType
    End Sub

    ''' <summary>
    ''' Creates an instance of this class.
    ''' </summary>
    ''' <param name="keyName">Key name.</param>
    ''' <param name="key">Instance of CryptoKey.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal keyName As String, ByVal key As CryptoKey)
        m_keyName = keyName
        m_key = key
    End Sub

    ''' <summary>
    ''' Creates an instance of this class by parsing a line read from a file.
    ''' </summary>
    ''' <param name="fileLine"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal fileLine As String)
        Dim sSplit() As String = fileLine.Split(","c)
        m_keyName = sSplit(0)
        m_key = New CryptoKey(sSplit(1))
        m_key.KeyType = CType([Enum].Parse(GetType(KeyType), sSplit(2)), KeyType)
    End Sub

    ''' <summary>
    ''' Returns a line that represents this instance to be written to a file.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFileLine() As String
        Return m_keyName + "," + m_key.GetClearValue + "," + m_key.KeyType.ToString
    End Function

End Class
