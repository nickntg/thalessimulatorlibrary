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

Namespace Message.XML

    ''' <summary>
    ''' This class holds a list of key/value pairs.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MessageKeyValuePairs

        Private m_KVPairs As New SortedList(Of String, String)

        ''' <summary>
        ''' Adds a key/value pair to this instance.
        ''' </summary>
        ''' <param name="key">Key.</param>
        ''' <param name="value">Value/</param>
        ''' <remarks></remarks>
        Public Sub Add(ByVal key As String, ByVal value As String)
            m_KVPairs.Add(key, value)
        End Sub

        ''' <summary>
        ''' Determines whether a value with a specific key exists.
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ContainsKey(ByVal key As String) As Boolean
            Return m_KVPairs.ContainsKey(key)
        End Function

        ''' <summary>
        ''' Returns a value based on a key.
        ''' </summary>
        ''' <param name="key">Key.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Item(ByVal key As String) As String
            Return m_KVPairs(key)
        End Function

        ''' <summary>
        ''' Returns a value based on a key or an empty string
        ''' if the key does not exist.
        ''' </summary>
        ''' <param name="key">Key.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ItemOptional(ByVal key As String) As String
            Return GetItemOrEmptyString(key)
        End Function

        ''' <summary>
        ''' Returns a combination of values based on two keys.
        ''' </summary>
        ''' <param name="key1">Key 1.</param>
        ''' <param name="key2">Key 2.</param>
        ''' <returns></returns>
        ''' <remarks>If either key does not exist, an empty string is used
        ''' in the place of the corresponding value.</remarks>
        Public Function ItemCombination(ByVal key1 As String, ByVal key2 As String) As String
            Return GetItemOrEmptyString(key1) + GetItemOrEmptyString(key2)
        End Function

        ''' <summary>
        ''' Returns the number of items in the list.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Count() As Integer
            Return m_KVPairs.Count
        End Function

        ''' <summary>
        ''' Returns all key-value pairs of this instance.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Dim strBld As New System.Text.StringBuilder
            For Each key As String In m_KVPairs.Keys
                strBld.AppendFormat("[Key,Value]=[{0},{1}]{2}", key, m_KVPairs(key), vbCrLf)
            Next
            Return strBld.ToString
        End Function

        ''' <summary>
        ''' Searches for a value corresponding to a key and returns that or an empty
        ''' string if the key is not present.
        ''' </summary>
        ''' <param name="key">Key.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetItemOrEmptyString(ByVal key As String) As String
            If m_KVPairs.ContainsKey(key) Then
                Return m_KVPairs(key)
            Else
                Return ""
            End If
        End Function

    End Class

End Namespace