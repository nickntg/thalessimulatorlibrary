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
    ''' Class that can parse a message field.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class can be used to parse a single message field. Several objects
    ''' of this class can be combined in a <see cref="MessageFieldParserCollection"/> in order
    ''' to parse a whole message to discrete elements.
    ''' </remarks>
    Public Class MessageFieldParser

        ''' <summary>
        ''' Defines a field type.
        ''' </summary>
        ''' <remarks>
        ''' Defines a field type.
        ''' </remarks>
        Public Enum FieldType
            ''' <summary>
            ''' Fixed length field.
            ''' </summary>
            ''' <remarks>
            ''' Fixed length field.
            ''' </remarks>
            FixedLengthField = 0
            ''' <summary>
            ''' Fixed length field with a header.
            ''' </summary>
            ''' <remarks>
            ''' Fixed length field with a header.
            ''' </remarks>
            FixedLengthWithHeader = 1
            ''' <summary>
            ''' Variable length field, depending upon the value of the header.
            ''' </summary>
            ''' <remarks>
            ''' Variable length field, depending upon the value of the header.
            ''' </remarks>
            VariableLengthWithHeader = 2
            ''' <summary>
            ''' Variable length field, depending upon the value of a subsequent delimiter.
            ''' </summary>
            ''' <remarks>
            ''' Variable length field, depending upon the value of a subsequent delimiter.
            ''' </remarks>
            VariableLengthUntilDelimiter = 3
        End Enum

        Private _length As Integer
        Private _headerLength As Integer
        Private _fieldType As FieldType
        Private _MFDC As MessageFieldDeterminerCollection
        Private _fieldName As String
        Private _fieldValue As String = ""
        Private _headerValue As String = ""
        Private _determinerName As String = ""
        Private _dependendField As String = ""
        Private _dependendValue As String = ""
        Private _delimiterValue As String = ""

        ''' <summary>
        ''' Returns the field's type.
        ''' </summary>
        ''' <remarks>
        ''' Returns the field's type.
        ''' </remarks>
        Public ReadOnly Property WhatFieldType() As FieldType
            Get
                Return _fieldType
            End Get
        End Property

        ''' <summary>
        ''' Returns the field's name.
        ''' </summary>
        ''' <remarks>
        ''' Returns the field's name.
        ''' </remarks>
        Public ReadOnly Property FieldName() As String
            Get
                Return _fieldName
            End Get
        End Property

        ''' <summary>
        ''' Returns the field's value.
        ''' </summary>
        ''' <remarks>
        ''' Returns the field's type. Use only after field parsing is complete.
        ''' </remarks>
        Public ReadOnly Property FieldValue() As String
            Get
                Return _fieldValue
            End Get
        End Property

        ''' <summary>
        ''' Returns the header value.
        ''' </summary>
        ''' <remarks>
        ''' Returns the header value.
        ''' </remarks>
        Public ReadOnly Property HeaderValue() As String
            Get
                Return _headerValue
            End Get
        End Property

        ''' <summary>
        ''' Returns the <see cref="MessageFieldDeterminer"/> name.
        ''' </summary>
        ''' <remarks>
        ''' Returns the <see cref="MessageFieldDeterminer"/> name. Use only after parsing is complete.
        ''' </remarks>
        Public ReadOnly Property DeterminerName() As String
            Get
                Return _determinerName
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the dependent field name.
        ''' </summary>
        ''' <remarks>
        ''' If set, parsing of this field is dependend upon the value of a previous
        ''' message field. This property returns or sets the value of this dependent field.
        ''' </remarks>
        Public Property DependentField() As String
            Get
                Return _dependendField
            End Get
            Set(ByVal Value As String)
                _dependendField = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the dependent field value.
        ''' </summary>
        ''' <remarks>
        ''' This property returns or sets the value of the dependent field (see also <see cref="MessageFieldParser.DependentField"/>).
        ''' </remarks>
        Public Property DependentValue() As String
            Get
                Return _dependendValue
            End Get
            Set(ByVal Value As String)
                _dependendValue = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the value of the delimiter to look for.
        ''' </summary>
        ''' <remarks>
        ''' This property returns or sets the value of the delimiter that follows the field.
        ''' </remarks>
        Public Property DelimiterValue() As String
            Get
                Return _delimiterValue
            End Get
            Set(ByVal Value As String)
                _delimiterValue = Value
            End Set
        End Property

        ''' <summary>
        ''' Constructor for fixed length fields.
        ''' </summary>
        ''' <remarks>
        ''' This constructor sets up the object for a fixed length field.
        ''' </remarks>
        Public Sub New(ByVal fieldName As String, ByVal length As Integer)
            _fieldName = fieldName
            _fieldType = FieldType.FixedLengthField
            _length = length
        End Sub

        ''' <summary>
        ''' Constructor for fixed length fields with a header.
        ''' </summary>
        ''' <remarks>
        ''' This constructor sets up the object for a fixed length field
        ''' with a header.
        ''' </remarks>
        Public Sub New(ByVal fieldName As String, ByVal headerLength As Integer, ByVal length As Integer)
            _fieldName = fieldName
            _fieldType = FieldType.FixedLengthWithHeader
            _headerLength = headerLength
            _length = length
        End Sub

        ''' <summary>
        ''' Constructor for variable length field.
        ''' </summary>
        ''' <remarks>
        ''' This constructor sets up the object for a variable length field.
        ''' </remarks>
        Public Sub New(ByVal fieldName As String, ByVal MFDC As MessageFieldDeterminerCollection)
            _fieldName = fieldName
            _MFDC = MFDC
            _fieldType = FieldType.VariableLengthWithHeader
        End Sub

        ''' <summary>
        ''' Constructor for variable field until a delimiter.
        ''' </summary>
        ''' <remarks>
        ''' This constructor sets up the object for a variable length field, parsed until
        ''' a delimiter is found.
        ''' </remarks>
        Public Sub New(ByVal fieldName As String, ByVal Delimiter As String)
            _fieldName = fieldName
            _fieldType = FieldType.VariableLengthUntilDelimiter
            _delimiterValue = Delimiter
        End Sub

        ''' <summary>
        ''' This method parses the field from a message's content.
        ''' </summary>
        ''' <remarks>
        ''' This method performs the actual field parsing.
        ''' </remarks>
        Public Sub ParseField(ByVal msg As Message)
            Select Case _fieldType
                Case FieldType.FixedLengthField
                    If msg.CharsLeft < _length Then
                        Throw New Exceptions.XShortMessage("Too few characters left to parse field " + _fieldName + "." + vbCrLf + _
                                                           "Length=" + _length.ToString() + ", left=" + msg.CharsLeft().ToString())
                    End If
                    _fieldValue = msg.GetSubstring(_length)
                    msg.AdvanceIndex(_length)
                Case FieldType.FixedLengthWithHeader
                    If msg.CharsLeft < _length + _headerLength Then
                        Throw New Exceptions.XShortMessage("Too few characters left to parse field " + _fieldName + "." + vbCrLf + _
                                                           "Length=" + _length.ToString() + ", header length=" + _headerLength.ToString() + ", left=" + msg.CharsLeft().ToString())
                    End If
                    _headerValue = msg.GetSubstring(_headerLength)
                    msg.AdvanceIndex(_headerLength)
                    _fieldValue = msg.GetSubstring(_length)
                    msg.AdvanceIndex(_length)
                Case FieldType.VariableLengthWithHeader
                    Dim MFD As MessageFieldDeterminer = _MFDC.FindDeterminerThatMatches(msg.GetSubstring(msg.CharsLeft))
                    If MFD Is Nothing Then
                        Throw New Exceptions.XNoDeterminerMatched("No determiner matched parsing for field " + _fieldName + "." + vbCrLf + _
                                                                  "Remaining message data: " + msg.GetSubstring(msg.CharsLeft))
                    End If
                    _determinerName = MFD.DeterminerName
                    _headerValue = msg.GetSubstring(MFD.HeaderValue.Length)
                    msg.AdvanceIndex(MFD.HeaderValue.Length)
                    If msg.CharsLeft < MFD.FieldLength Then
                        Throw New Exceptions.XShortMessage("Too few characters left to parse field " + _fieldName + "." + vbCrLf + _
                                                           "Length=" + MFD.FieldLength.ToString() + ", left=" + msg.CharsLeft().ToString())
                    End If
                    _fieldValue = msg.GetSubstring(MFD.FieldLength)
                    msg.AdvanceIndex(MFD.FieldLength)
                Case FieldType.VariableLengthUntilDelimiter
                    Dim f As String = ""
                    While msg.CharsLeft > 0
                        If msg.GetSubstring(1) = _delimiterValue Then
                            _fieldValue = f
                            Exit While
                        Else
                            f = f + msg.GetSubstring(1)
                            msg.AdvanceIndex(1)
                        End If
                    End While
                    If msg.CharsLeft = 0 OrElse msg.GetSubstring(1) <> _delimiterValue Then
                        Throw New Exceptions.XShortMessage("Delimiter not found to parse field " + _fieldName + ".")
                    End If
                    'msg.AdvanceIndex(-1)
            End Select
        End Sub

    End Class

    ''' <summary>
    ''' Class that holds a collection of <see cref="MessageFieldParser"/> objects.
    ''' </summary>
    ''' <remarks>
    ''' This class is designed to help a caller parse a complete message to discrete fields.
    ''' </remarks>
    Public Class MessageFieldParserCollection

        Private _AL As New ArrayList

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
            Return CType(_AL(index), MessageFieldParser)
        End Function

        ''' <summary>
        ''' Returns a <see cref="MessageFieldParser"/> object based on a field name.
        ''' </summary>
        ''' <remarks>
        ''' Returns a <see cref="MessageFieldParser"/> object based on a field name.
        ''' </remarks>
        Public Function GetMessageFieldByName(ByVal fieldName As String) As MessageFieldParser
            For i As Integer = 0 To _AL.Count - 1
                If CType(_AL.Item(i), MessageFieldParser).FieldName = fieldName Then
                    Return CType(_AL.Item(i), MessageFieldParser)
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
                If CType(_AL.Item(i), MessageFieldParser).DependentField = "" Then
                    CType(_AL.Item(i), MessageFieldParser).ParseField(msg)
                Else
                    For j As Integer = 0 To i - 1
                        If CType(_AL.Item(j), MessageFieldParser).FieldName = _
                           CType(_AL.Item(i), MessageFieldParser).DependentField Then
                            If CType(_AL.Item(j), MessageFieldParser).FieldValue = _
                               CType(_AL.Item(i), MessageFieldParser).DependentValue Then
                                CType(_AL.Item(i), MessageFieldParser).ParseField(msg)
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
                Dim o As MessageFieldParser = CType(_AL.Item(i), MessageFieldParser)
                Select Case o.WhatFieldType
                    Case MessageFieldParser.FieldType.FixedLengthField
                        Debug.WriteLine("Field " + o.FieldName + ", value " + o.FieldValue)
                    Case MessageFieldParser.FieldType.FixedLengthWithHeader
                        Debug.WriteLine("Field " + o.FieldName + ", value=" + o.FieldValue + ", header=" + o.HeaderValue)
                    Case MessageFieldParser.FieldType.VariableLengthWithHeader
                        Debug.WriteLine("Field " + o.FieldName + ", value=" + o.FieldValue + ", header=" + o.HeaderValue)
                        Debug.WriteLine("Determiner used " + o.DeterminerName)
                End Select
            Next
        End Sub

    End Class

End Namespace
