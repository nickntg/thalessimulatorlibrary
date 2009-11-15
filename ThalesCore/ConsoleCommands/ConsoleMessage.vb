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
    ''' Class used to hold information about a data exchange between the console and the simulator.
    ''' </summary>
    ''' <remarks>
    ''' Console commands are made up of a series of information entered by the user. For example, 
    ''' the GC command requires the user to enter the key length, key type and key scheme. During
    ''' these information exchanges, the simulator sends a message to the console that informs the
    ''' user of the data that is required and the user types in the data which is send to the 
    ''' simulator by the console.
    ''' 
    ''' Objects of this class are used to hold information about one such data exchange.
    ''' </remarks>
    Public Class ConsoleMessage

        ''' <summary>
        ''' Internal storage of the message displayed to the console.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _clientMessage As String

        ''' <summary>
        ''' Internal storage of the data entered by teh user.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _consoleMessage As String

        ''' <summary>
        ''' Internal storage for the flag that designates that this is a number of components.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _isNumberOfComponents As Boolean

        ''' <summary>
        ''' Internal storage for the flag that designates that this is a component.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _isComponent As Boolean

        ''' <summary>
        ''' Internal storage for the validator.
        ''' </summary>
        ''' <remarks></remarks>
        Protected _messageValidator As IConsoleDataValidator

        ''' <summary>
        ''' Get/set the message to send to the console, prompting the user for information.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ClientMessage() As String
            Get
                Return _clientMessage
            End Get
            Set(ByVal value As String)
                _clientMessage = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the data passed from the console to the simulator.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ConsoleMessage() As String
            Get
                Return _consoleMessage
            End Get
            Set(ByVal value As String)
                _consoleMessage = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set a flag that indicates whether this message refers to a number of components.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsNumberOfComponents() As Boolean
            Get
                Return _isNumberOfComponents
            End Get
            Set(ByVal value As Boolean)
                _isNumberOfComponents = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set a flag that indicates whether this message refers to a component.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsComponent() As Boolean
            Get
                Return _isComponent
            End Get
            Set(ByVal value As Boolean)
                _isComponent = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the validator that will verify the data passed to the simulator by the user.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>If this is not set, no validation is performed.</remarks>
        Public Property ConsoleMessageValidator() As IConsoleDataValidator
            Get
                Return _messageValidator
            End Get
            Set(ByVal value As IConsoleDataValidator)
                _messageValidator = value
            End Set
        End Property

        ''' <summary>
        ''' Creates an instance of this class.
        ''' </summary>
        ''' <param name="clientMessage">Message to console.</param>
        ''' <param name="consoleMessage">Data from console.</param>
        ''' <param name="messageValidator">Validator instance.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal clientMessage As String, ByVal consoleMessage As String, ByVal messageValidator As IConsoleDataValidator)
            _clientMessage = clientMessage
            _consoleMessage = consoleMessage
            _messageValidator = messageValidator
            _isNumberOfComponents = False
            _isComponent = False
        End Sub

        ''' <summary>
        ''' Creates an instance of this class with an option to define whether it refers to a number of components entry.
        ''' </summary>
        ''' <param name="clientMessage">Message to console.</param>
        ''' <param name="consoleMessage">Data from console.</param>
        ''' <param name="isNumberOfComponents">Flag that indicates whether this refers to a number of components entry.</param>
        ''' <param name="messageValidator">Validator instance.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal clientMessage As String, ByVal consoleMessage As String, ByVal isNumberOfComponents As Boolean, ByVal messageValidator As IConsoleDataValidator)
            _clientMessage = clientMessage
            _consoleMessage = consoleMessage
            _isNumberOfComponents = isNumberOfComponents
            _isComponent = False
            _messageValidator = messageValidator
        End Sub

        ''' <summary>
        ''' Creates an instance of this class with an option to define whether it refers
        ''' to a number of components entry or to a component entry.
        ''' </summary>
        ''' <param name="clientMessage">Message to console.</param>
        ''' <param name="consoleMessage">Data from console.</param>
        ''' <param name="isNumberOfComponents">Flag that indicates whether this refers to a number of components entry.</param>
        ''' ''' <param name="isComponent">Flag that indicates whether this refers to a component entry.</param>
        ''' <param name="messageValidator">Validator instance.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal clientMessage As String, ByVal consoleMessage As String, ByVal isNumberOfComponents As Boolean, ByVal isComponent As Boolean, ByVal messageValidator As IConsoleDataValidator)
            _clientMessage = clientMessage
            _consoleMessage = consoleMessage
            _isNumberOfComponents = isNumberOfComponents
            _isComponent = isComponent
            _messageValidator = messageValidator
        End Sub

    End Class

End Namespace