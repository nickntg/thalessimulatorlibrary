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
''' Represents a Racal error.
''' </summary>
''' <remarks>
''' This class is used to hold a Racal error code and its associated help text.
''' </remarks>
Public Class ThalesError

    Private _errorCode As String
    Private _errorHelp As String

    ''' <summary>
    ''' Object error code.
    ''' </summary>
    ''' <remarks>
    ''' Returns the Racal error code of this object.
    ''' </remarks>
    Public ReadOnly Property ErrorCode() As String
        Get
            Return _errorCode
        End Get
    End Property

    ''' <summary>
    ''' Object help.
    ''' </summary>
    ''' <remarks>
    ''' Returns a description of the error code associated with this object.
    ''' </remarks>
    Public ReadOnly Property ErrorHelp() As String
        Get
            Return _errorHelp
        End Get
    End Property

    ''' <summary>
    ''' ErrorCode constructor.
    ''' </summary>
    ''' <remarks>
    ''' This constructor instantiates the ErrorCode object with appropriate data.
    ''' </remarks>
    Public Sub New(ByVal ErrorCode As String, ByVal ErrorHelp As String)
        _errorCode = ErrorCode
        _errorHelp = ErrorHelp
    End Sub

End Class
