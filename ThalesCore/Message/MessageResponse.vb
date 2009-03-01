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
    ''' Used to construct a message response.
    ''' </summary>
    ''' <remarks>
    ''' Primitive class that can be used to construct a message response.
    ''' </remarks>
    Public Class MessageResponse

        Private _data As String = ""

        ''' <summary>
        ''' Returns the constructed message data.
        ''' </summary>
        ''' <remarks>
        ''' Returns the constructed message data.
        ''' </remarks>
        Public ReadOnly Property MessageData() As String
            Get
                Return _data
            End Get
        End Property

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Adds an element to the message.
        ''' </summary>
        ''' <remarks>
        ''' Appends a string element to the current message.
        ''' </remarks>
        Public Sub AddElement(ByVal s As String)
            _data += s
        End Sub

        ''' <summary>
        ''' Adds an element to the front of the message.
        ''' </summary>
        ''' <remarks>
        ''' Adds a string element to the beginning of the current message.
        ''' </remarks>
        Public Sub AddElementFront(ByVal s As String)
            _data = s + _data
        End Sub

    End Class

End Namespace
