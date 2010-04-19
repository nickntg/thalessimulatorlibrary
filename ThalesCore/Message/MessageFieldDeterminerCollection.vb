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

Namespace Message

    ''' <summary>
    ''' Class designed to hold several <see cref="MessageFieldDeterminer"/> objects.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class are typically loaded with several field determiners in
    ''' order to check the nature of a field present in a message.
    ''' <P>
    ''' <example>This example illustrates a common use of the MessageFieldDeterminerCollection class.
    ''' <code>
    ''' Const PVK_PAIR As String = "PVK_PAIR"
    ''' Const PVK_PAIR_DETERMINER_PLAIN As String = "PLAIN_KEY"
    ''' Const PVK_PAIR_DETERMINER_WITHHEADER As String = "WITH_HEADER"
    ''' Const PIN_BLOCK As String = "PIN_BLOCK"
    ''' Const ACCOUNT_NUMBER As String = "ACCOUNT_NUMBER"
    ''' Const PVKI As String = "PVKI"
    ''' 
    ''' Dim o As New Message.MessageFieldParserCollection
    ''' 
    ''' 'The PVK pair may be present as a 32-hex key
    ''' 'or it may preceeded by X.
    ''' Dim MFDC As New Message.MessageFieldDeterminerCollection
    ''' MFDC.AddFieldDeterminer(New Message.MessageFieldDeterminer(PVK_PAIR_DETERMINER_WITHHEADER, "X", 32))
    ''' MFDC.AddFieldDeterminer(New Message.MessageFieldDeterminer(PVK_PAIR_DETERMINER_PLAIN, "", 32))
    ''' 
    ''' o.AddMessageFieldParser(New Message.MessageFieldParser(PVK_PAIR, MFDC))
    ''' o.AddMessageFieldParser(New Message.MessageFieldParser(PIN_BLOCK, 16))
    ''' o.AddMessageFieldParser(New Message.MessageFieldParser(ACCOUNT_NUMBER, 12))
    ''' o.AddMessageFieldParser(New Message.MessageFieldParser(PVKI, 1))
    ''' 
    ''' o.ParseMessage(msg)
    ''' </code>
    ''' </example>
    ''' </P>
    ''' </remarks>
    Public Class MessageFieldDeterminerCollection

        Private _AL As New List(Of MessageFieldDeterminer)

        ''' <summary>
        ''' Returns a <see cref="MessageFieldDeterminer"/> object by its index.
        ''' </summary>
        ''' <remarks>
        ''' Returns a field determiner by its index.
        ''' </remarks>
        Public Function GetFieldDeterminer(ByVal index As Integer) As MessageFieldDeterminer
            Return _AL(index)
        End Function

        ''' <summary>
        ''' Returns the number of <see cref="MessageFieldDeterminer"/> objects present.
        ''' </summary>
        ''' <remarks>
        ''' Returns the number of <see cref="MessageFieldDeterminer"/> objects present.
        ''' </remarks>
        Public Function GetFieldCount() As Integer
            Return _AL.Count()
        End Function

        ''' <summary>
        ''' Adds a new <see cref="MessageFieldDeterminer"/>.
        ''' </summary>
        ''' <remarks>
        ''' Adds a new <see cref="MessageFieldDeterminer"/> object.
        ''' </remarks>
        Public Sub AddFieldDeterminer(ByVal FD As MessageFieldDeterminer)
            _AL.Add(FD)
        End Sub

        ''' <summary>
        ''' Finds a determiner that matches the message contents.
        ''' </summary>
        ''' <remarks>
        ''' This method will search through the <see cref="MessageFieldDeterminer"/> objects in order
        ''' to find one that matches. The <see cref="MessageFieldDeterminer"/> objects are examined
        ''' in the order they were added to the collection.
        ''' </remarks>
        Public Function FindDeterminerThatMatches(ByVal data As String) As MessageFieldDeterminer
            For i As Integer = 0 To _AL.Count - 1
                If _AL(i).FieldMatches(data) Then
                    Return _AL(i)
                End If
            Next
            Return Nothing
        End Function

    End Class

End Namespace