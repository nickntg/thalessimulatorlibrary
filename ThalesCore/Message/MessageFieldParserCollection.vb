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
    ''' Class that holds a collection of <see cref="MessageFieldParser"/> objects.
    ''' </summary>
    ''' <remarks>
    ''' This class is designed to help a caller parse a complete message to discrete fields.
    ''' </remarks>
    Public Class MessageFieldParserCollection

        Private _AL As New List(Of MessageFieldParser)

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Adds a <see cref="MessageFieldParser"/> to the collection.
        ''' </summary>
        ''' <remarks>
        ''' Adds a <see cref="MessageFieldParser"/> to the collection.
        ''' </remarks>
        Public Sub AddMessageFieldParser(ByVal MFP As MessageFieldParser)
            _AL.Add(MFP)
        End Sub

        ''' <summary>
        ''' Returns the number of <see cref="MessageFieldParser"/> objects in the collection.
        ''' </summary>
        ''' <remarks>
        ''' Returns the number of <see cref="MessageFieldParser"/> objects in the collection.
        ''' </remarks>
        Public Function MessageFieldCount() As Integer
            Return _AL.Count
        End Function

        ''' <summary>
        ''' Returns a <see cref="MessageFieldParser"/> object based on its index.
        ''' </summary>
        ''' <remarks>
        ''' Returns a <see cref="MessageFieldParser"/> object based on its index.
        ''' </remarks>
        Public Function GetMessageFieldByIndex(ByVal index As Integer) As MessageFieldParser
            Return _AL(index)
        End Function

        ''' <summary>
        ''' Returns a <see cref="MessageFieldParser"/> object based on a field name.
        ''' </summary>
        ''' <remarks>
        ''' Returns a <see cref="MessageFieldParser"/> object based on a field name.
        ''' </remarks>
        Public Function GetMessageFieldByName(ByVal fieldName As String) As MessageFieldParser
            For i As Integer = 0 To _AL.Count - 1
                If _AL(i).FieldName = fieldName Then
                    Return _AL(i)
                End If
            Next
            Return Nothing
        End Function

        ''' <summary>
        ''' Parses a message.
        ''' </summary>
        ''' <remarks>
        ''' This method uses all <see cref="MessageFieldParser"/> objects present in the
        ''' collection to parse a message. The <see cref="MessageFieldParser"/> objects
        ''' are used in the order they were added in the collection.
        ''' </remarks>
        Public Sub ParseMessage(ByVal msg As Message)

            For i As Integer = 0 To _AL.Count - 1
                If _AL.Item(i).DependentField = "" Then
                    _AL.Item(i).ParseField(msg)
                Else
                    For j As Integer = 0 To i - 1
                        If _AL.Item(j).FieldName = _AL.Item(i).DependentField Then
                            If _AL.Item(j).FieldValue = _AL.Item(i).DependentValue Then
                                _AL.Item(i).ParseField(msg)
                            End If
                            Exit For
                        End If
                    Next
                End If
            Next

            If msg.CharsLeft <> 0 Then
                ''TODO: Hmmmmm...
                'Log.Logger.MajorWarning("There are still unparsed message characters")
            End If

            DumpFields()
        End Sub

        Private Sub DumpFields()

            For i As Integer = 0 To _AL.Count - 1
                Select Case _AL(i).WhatFieldType
                    Case MessageFieldParser.FieldType.FixedLengthField
                        Debug.WriteLine("Field " + _AL(i).FieldName + ", value " + _AL(i).FieldValue)
                    Case MessageFieldParser.FieldType.FixedLengthWithHeader
                        Debug.WriteLine("Field " + _AL(i).FieldName + ", value=" + _AL(i).FieldValue + ", header=" + _AL(i).HeaderValue)
                    Case MessageFieldParser.FieldType.VariableLengthWithHeader
                        Debug.WriteLine("Field " + _AL(i).FieldName + ", value=" + _AL(i).FieldValue + ", header=" + _AL(i).HeaderValue)
                        Debug.WriteLine("Determiner used " + _AL(i).DeterminerName)
                End Select
            Next
        End Sub

    End Class

End Namespace