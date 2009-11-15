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

Namespace ConsoleCommands

    ''' <summary>
    ''' This class holds information about a console command.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ConsoleCommandClass

        ''' <summary>
        ''' Internal storage for the console command code.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _commandCode As String

        ''' <summary>
        ''' Internal storage for the console command description.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _commandDescription As String

        ''' <summary>
        ''' Internal storage for the console command type.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _commandType As Type

        ''' <summary>
        ''' Get/set the console command code.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommandCode() As String
            Get
                Return _commandCode
            End Get
            Set(ByVal value As String)
                _commandCode = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the console command description.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommandDescription() As String
            Get
                Return _commandDescription
            End Get
            Set(ByVal value As String)
                _commandDescription = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the type that implements the console command.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommandType() As Type
            Get
                Return _commandType
            End Get
            Set(ByVal value As Type)
                _commandType = value
            End Set
        End Property

        ''' <summary>
        ''' Default class constructor.
        ''' </summary>
        ''' <param name="commandCode">Console command code.</param>
        ''' <param name="commandDescription">Description of console command.</param>
        ''' <param name="commandType">Implementor type.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal commandCode As String, ByVal commandDescription As String, ByVal commandType As Type)
            _commandCode = commandCode
            _commandDescription = commandDescription
            _commandType = commandType
        End Sub

    End Class

End Namespace
