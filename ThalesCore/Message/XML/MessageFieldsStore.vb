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
    ''' This class holds all message field definitions that were read.
    ''' This is done to save file I/O and not go to the disk every time
    ''' a message needs to be parsed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MessageFieldsStore

        Private Shared m_store As New SortedDictionary(Of String, MessageFields)

        ''' <summary>
        ''' Clear the saved message fields definitions.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub Clear()
            m_store.Clear()
        End Sub

        ''' <summary>
        ''' Add message fields definition.
        ''' </summary>
        ''' <param name="key">Key to use.</param>
        ''' <param name="fields">Message field definitions.</param>
        ''' <remarks></remarks>
        Public Shared Sub Add(ByVal key As String, ByVal fields As MessageFields)
            m_store.Add(key, fields)
        End Sub

        ''' <summary>
        ''' Return message field definitions if they exist.
        ''' </summary>
        ''' <param name="key">Key to search for.</param>
        ''' <returns></returns>
        ''' <remarks>Null is return if the message field definitions are not present.</remarks>
        Public Shared Function Item(ByVal key As String) As MessageFields
            If m_store.ContainsKey(key) Then
                'Return a clone. The reason for this is that classes that
                'parse messages may perform changes to the object and we want
                'to keep the original copy.
                Return m_store(key).Clone
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace