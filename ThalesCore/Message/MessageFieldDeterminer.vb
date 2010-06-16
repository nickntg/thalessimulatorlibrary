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
    ''' Class designed to help determine a message field type.
    ''' </summary>
    ''' <remarks>
    ''' The <b>MessageFieldDeterminer</b> class is used by objects that inherit from
    ''' <see cref="HostCommands.AHostCommand"/> in order to determine if a field of an incoming message
    ''' is of a particular type (for example, whether an encrypted key is represented
    ''' as 16 hex characters or is preceeded by a length indicator like X or Y).
    ''' <P>A field determiner is fed with a <see cref="MessageFieldDeterminer.HeaderValue"/> and a <see cref="MessageFieldDeterminer.FieldLength"/>.
    ''' If the field examined starts with the value indicated by <see cref="MessageFieldDeterminer.HeaderValue"/>, then the
    ''' field is assumed to match the determiner and, therefore, the rest of the field has a length
    ''' of <see cref="MessageFieldDeterminer.FieldLength"/> characters.</P>
    ''' <P>If the <see cref="MessageFieldDeterminer.HeaderValue"/> is an empty string, the determiner by default returns
    ''' a match.</P>
    ''' <P>An alternative matching method for a determiner is to look into the remaining message length
    ''' and decide the field's length. An example of this is the A0 Thales command, where the ZMK key length
    ''' can be 16 or 32 hexadecimal characters.</P>
    ''' </remarks>
    <Obsolete()> Public Class MessageFieldDeterminer

        Private _headerValue As String
        Private _fieldLength As Integer
        Private _determinerName As String
        Private _lookAheadLength As Integer

        ''' <summary>
        ''' Returns the value of the field header.
        ''' </summary>
        ''' <remarks>
        ''' Value of the field header.
        ''' </remarks>
        Public ReadOnly Property HeaderValue() As String
            Get
                Return _headerValue
            End Get
        End Property

        ''' <summary>
        ''' Returns the value of the field length.
        ''' </summary>
        ''' <remarks>
        ''' Returns the value of the field length if the field matches
        ''' this determiner.
        ''' </remarks>
        Public ReadOnly Property FieldLength() As Integer
            Get
                Return _fieldLength
            End Get
        End Property

        ''' <summary>
        ''' Determiner name.
        ''' </summary>
        ''' <remarks>
        ''' Returns the determiner name.
        ''' </remarks>
        Public ReadOnly Property DeterminerName() As String
            Get
                Return _determinerName
            End Get
        End Property

        ''' <summary>
        ''' Look-ahead length.
        ''' </summary>
        ''' <remarks>
        ''' If the remaining message length is greater to this value, the determiner
        ''' is assumed to match.
        ''' </remarks>
        Public ReadOnly Property LookAheadLength() As Integer
            Get
                Return _lookAheadLength
            End Get
        End Property

        ''' <summary>
        ''' Class constructor.
        ''' </summary>
        ''' <remarks>
        ''' Class constructor for determiners that use the header in order to determine a match.
        ''' </remarks>
        Public Sub New(ByVal determinerName As String, ByVal headerValue As String, ByVal fieldLength As Integer)
            _determinerName = determinerName
            _headerValue = headerValue
            _fieldLength = fieldLength
            _lookAheadLength = 0
        End Sub

        ''' <summary>
        ''' Class constructor.
        ''' </summary>
        ''' <remarks>
        ''' Class constructor for determiners that use the message remainder in order to determine a match.
        ''' </remarks>
        Public Sub New(ByVal determinerName As String, ByVal lookAheadLength As Integer, ByVal fieldLength As Integer)
            _determinerName = determinerName
            _lookAheadLength = lookAheadLength
            _fieldLength = fieldLength
            _headerValue = ""
        End Sub

        ''' <summary>
        ''' Determine if this determiner matches a message's contents.
        ''' </summary>
        ''' <remarks>
        ''' The contents of the data are assumed to start with the contents of the field.
        ''' </remarks>
        Public Function FieldMatches(ByVal data As String) As Boolean
            If _lookAheadLength = 0 Then
                If _headerValue = "" Then
                    Return True
                Else
                    Return data.StartsWith(_headerValue)
                End If
            Else
                If data.Length > _lookAheadLength Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Function

    End Class

End Namespace
