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
    ''' This class depicts a string that represents a request message.
    ''' </summary>
    ''' <remarks>
    ''' Objects of this class can be used to hold incoming message data and perform
    ''' basic manipulation of the data.
    ''' </remarks>
    Public Class Message

        Private _data As String = ""
        Private _bData() As Byte
        Private _curIndex As Integer = 0

        ''' <summary>
        ''' The <b>complete</b> message data.
        ''' </summary>
        ''' <remarks>
        ''' This property returns all data present in the message.
        ''' </remarks>
        Public ReadOnly Property MessageData() As String
            Get
                Return _data
            End Get
        End Property

        ''' <summary>
        ''' Current positioning index.
        ''' </summary>
        ''' <remarks>
        ''' This property returns the current positioning index. The index value starts
        ''' at 0 and is advanced using the <see cref="Message.AdvanceIndex"/> 
        ''' method.
        ''' </remarks>
        Public ReadOnly Property CurrentIndex() As Integer
            Get
                Return _curIndex
            End Get
        End Property

        ''' <summary>
        ''' Default class constructor.
        ''' </summary>
        ''' <remarks>
        ''' This constructor initializes the object with a string representation of the
        ''' incoming message.
        ''' </remarks>
        Public Sub New(ByVal data As String)
            _bData = Utility.GetBytesFromString(data)
            _data = data
        End Sub

        ''' <summary>
        ''' Alternative class constructor.
        ''' </summary>
        ''' <remarks>
        ''' This constructor initializes the object with a byte representation of the
        ''' incoming message.
        ''' </remarks>
        Public Sub New(ByVal data() As Byte)
            ReDim _bData(data.GetLength(0) - 1)
            _data = Utility.GetStringFromBytes(data)
        End Sub

        ''' <summary>
        ''' Resets the positioning index.
        ''' </summary>
        ''' <remarks>
        ''' This method resets the positioning index to 0.
        ''' </remarks>
        Public Sub ResetIndex()
            _curIndex = 0
        End Sub

        ''' <summary>
        ''' Advances the positioning index.
        ''' </summary>
        ''' <remarks>
        ''' This method advances the positioning index by the specified number of bytes/characters.
        ''' </remarks>
        Public Sub AdvanceIndex(ByVal count As Integer)
            _curIndex += count
        End Sub

        ''' <summary>
        ''' Decreases the positioning index.
        ''' </summary>
        ''' <param name="count">Number of bytes/characters to backtrack.</param>
        ''' <remarks></remarks>
        Public Sub DecreaseIndex(ByVal count As Integer)
            _curIndex -= count
        End Sub

        ''' <summary>
        ''' Returns a substring of the message.
        ''' </summary>
        ''' <remarks>
        ''' This method returns a substring of the message, starting at the positioning index.
        ''' </remarks>
        Public Function GetSubstring(ByVal length As Integer) As String
            Return _data.Substring(_curIndex, length)
        End Function

        ''' <summary>
        ''' Returns the remaining bytes of the message.
        ''' </summary>
        ''' <remarks>
        ''' Returns the remaining bytes of the message.
        ''' </remarks>
        Public Function GetRemainingBytes() As Byte()
            Dim b() As Byte
            ReDim b(_data.Length - _curIndex - 1)
            Array.Copy(_bData, _curIndex, b, 0, b.GetLength(0))
            Return b
        End Function

        ''' <summary>
        ''' Returns the number of characters left in the message.
        ''' </summary>
        ''' <remarks>
        ''' This method returns the number of characters left in the message. This is a
        ''' number calculated by subtracting the positioning index from the total number of
        ''' characters of the message.
        ''' </remarks>
        Public Function CharsLeft() As Integer
            Return _data.Length - _curIndex
        End Function

        ''' <summary>
        ''' Returns the end sentinel and the trailer from the message and
        ''' removes it from the message.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTrailers() As String
            _bData = Utility.GetBytesFromString(Me.MessageData)
            Dim idx As Integer = _bData.GetLength(0) - 1
            While idx >= 0
                'Search for the end sentinel from the end.
                If _bData(idx) = &H19 Then
                    'Copy the end sentinel and trailer to a byte array.
                    Dim b() As Byte
                    ReDim b(_bData.GetLength(0) - idx - 1)
                    Array.Copy(_bData, idx, b, 0, b.GetLength(0))
                    'Copy the original message up to the sentinel to a byte array.
                    Dim bNew() As Byte
                    ReDim bNew(idx - 1)
                    Array.Copy(_bData, 0, bNew, 0, bNew.GetLength(0))
                    'Copy over the original message.
                    _bData = bNew
                    _data = Utility.GetStringFromBytes(_bData)
                    'Return the sentinel and trailer.
                    Return Utility.GetStringFromBytes(b)
                End If
                idx -= 1
            End While
            'If no end sentinel is found, do nothing.
            Return ""
        End Function

    End Class

End Namespace
